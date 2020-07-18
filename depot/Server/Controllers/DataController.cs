using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using depot.Server.Data;
using depot.Server.Entities;
using depot.Shared.RequestModels;
using depot.Shared.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using depot.Server.Models;
using depot.Server.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CRM.Server.Controllers
{
    [ApiController]
    public class DataController : ControllerBase
    {
        private ApplicationDbContext _db;
        private MongoDBContext _mongoDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public DataController(ApplicationDbContext dbContext, MongoDBContext mongoDBContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
            _mongoDBContext = mongoDBContext;
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/folder")]
        async public Task<IActionResult> FolderCreate([FromBody]FolderCreateEditRequestModel model)
        {
            string userId = User.GetUserId();
            
            Folder Folder = new Folder()
            {
                Name = model.Name,
            };
            _db.Folders.Add(Folder);

            //Create authorization for creating user
            FolderAuthorizedUser FolderAuthorizedUser = new FolderAuthorizedUser()
            {
                IsFolderAdmin = true,
                FolderId = Folder.Id,
                Folder = Folder,
                ApplicationUserId = userId
            };
            _db.FolderAuthorizedUsers.Add(FolderAuthorizedUser);
            
            await _db.SaveChangesAsync();

            ResponseId response = new ResponseId()
            {
                Id = Folder.Id
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/folder/{folderId}")]
        async public Task<IActionResult> GetFolderById(string folderId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            var response = await _db.Folders.Where(c => c.Id == folderId)
                                    .Include(o => o.AuthorizedUsers)
                                    .Select(o => new ResponseFolder()
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        AuthorizedUsers = o.AuthorizedUsers.Any(u => u.ApplicationUserId == userId && u.IsFolderAdmin == true) ? o.AuthorizedUsers
                                                            .Select(a => new ResponseFolderAuthorizedUser()
                                                            {
                                                                Id = a.Id,
                                                                ApplicationUserId = a.ApplicationUserId,
                                                                ApplicationUserEmail = a.ApplicationUser.Email,
                                                                IsFolderAdmin = a.IsFolderAdmin
                                                            }).ToList() : null
                                    }).FirstOrDefaultAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/folder/user/authorized")]
        async public Task<IActionResult> GetFoldersByAuthorization()
        {
            string userId = User.GetUserId();

            var Folders = await _db.FolderAuthorizedUsers.Where(o => o.ApplicationUserId == userId)
                                .Include(o => o.Folder)
                                .Select(o => new ResponseFolderShort()
                                {
                                    Id = o.Folder.Id,
                                    Name = o.Folder.Name,
                                    IsAdministrator = o.IsFolderAdmin
                                }).ToListAsync();

            return Ok(Folders);
        }


        [Authorize]
        [HttpPut]
        [Route("api/v1/folder/{folderId}")]
        async public Task<IActionResult> UpdateFolderChangeName([FromBody]FolderCreateEditRequestModel model, string folderId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var Folder = await _db.Folders.Where(o => o.Id == folderId).FirstOrDefaultAsync();
            Folder.Name = model.Name;

            await _db.SaveChangesAsync();

            return Ok();
        }

        

        [Authorize]
        [HttpPut]
        [Route("api/v1/folder/{folderId}/user/authorized")]
        async public Task<IActionResult> UpdateFolderSetUserAuthorized([FromBody]FolderAddAuthorizedEmailModel model, string folderId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var foundUserByEmail = await _userManager.FindByNameAsync(model.Email);
            if (foundUserByEmail == null)
                return BadRequest("User Not Found");

            var Folder = await _db.Folders.FirstOrDefaultAsync(o => o.Id == folderId);
            if (Folder == null)
                return BadRequest("Folder not found");

            FolderAuthorizedUser authorizedUser = new FolderAuthorizedUser()
            {
                FolderId = Folder.Id,
                Folder = Folder,
                ApplicationUser = foundUserByEmail,
                ApplicationUserId = foundUserByEmail.Id
            };

            _db.FolderAuthorizedUsers.Add(authorizedUser);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/folder/{folderId}/user/{applicationUserId}/authorized")]
        async public Task<IActionResult> UpdateFolderRemoveUserAuthorized(string folderId, string applicationUserId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var removeThese = _db.FolderAuthorizedUsers.Where(o => o.FolderId == folderId && o.ApplicationUserId == applicationUserId);
            _db.FolderAuthorizedUsers.RemoveRange(removeThese);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/folder/{folderId}/user/{applicationUserId}/authorized/toggle")]
        async public Task<IActionResult> UpdateFolderRemoveUserToggle([FromBody]FolderToggleAuthorizedModel model, string folderId, string applicationUserId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var updateThis = await _db.FolderAuthorizedUsers.Where(o => o.FolderId == folderId && o.ApplicationUserId == applicationUserId).FirstOrDefaultAsync();
            updateThis.IsFolderAdmin = model.Administrator;

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/folder/{folderId}")]
        async public Task<IActionResult> DeleteFolder(string folderId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var authorizations = _db.FolderAuthorizedUsers.Where(o => o.FolderId == folderId);
            _db.FolderAuthorizedUsers.RemoveRange(authorizations);

            var Folder = _db.Folders.Where(o => o.Id == folderId);
            _db.Folders.RemoveRange(Folder);

            await _db.SaveChangesAsync();

            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("FolderId", folderId)
                        });

            await _mongoDBContext.Instances.DeleteManyAsync(query);


            return Ok();
        }



        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/type")]
        async public Task<IActionResult> CreateFolderType([FromBody]ResponseDataType model, string folderId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            DataType newType = new DataType()
            {
                Name = model.Name,
                FolderId = folderId
            };
            
            _db.DataTypes.Add(newType);

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = newType.Id });
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}")]
        async public Task<IActionResult> UpdateFolderTypeName([FromBody]ResponseDataType model, string folderId, string dataTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var type = await _db.DataTypes.Where(d => d.FolderId == folderId && d.Id == dataTypeId).FirstOrDefaultAsync();
            type.Name = model.Name;
            
            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = type.Id });
        }


        [Authorize]
        [HttpDelete]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}")]
        async public Task<IActionResult> DeleteFolderTypeById(string folderId, string dataTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            //Delete the type
            var deleteThis = _db.DataTypes.Where(d => d.FolderId == folderId && d.Id == dataTypeId);
            _db.DataTypes.RemoveRange(deleteThis);

            await _db.SaveChangesAsync();

            var query = new BsonDocument("$and",
                            new BsonArray
                            {
                                new BsonDocument("FolderId", folderId),
                                new BsonDocument("TypeId", dataTypeId)
                            });
            await _mongoDBContext.Instances.DeleteManyAsync(query);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}")]
        [Route("api/v1/folder/{folderId}/type")]
        async public Task<IActionResult> GetFolderType(string folderId, string dataTypeId = null)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            if (dataTypeId == "menu")
            {
                var listResponse = await _db.DataTypes.Where(i => i.FolderId == folderId)
                                    .Select(i => new ResponseDataType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name
                                    }).ToListAsync();

                return Ok(listResponse);
            }

            if (dataTypeId != null)
            {
                var singleResponse = await _db.DataTypes.Where(i => i.Id == dataTypeId && i.FolderId == folderId)
                                    .Include(i => i.Fields)
                                    .Select(i => new ResponseDataType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name,
                                        Fields = i.Fields.Select(f => new ResponseField()
                                        {
                                            Id = f.Id,
                                            Name = f.Name,
                                            Row = f.Row,
                                            Column = f.Column,
                                            ColumnSpan = f.ColumnSpan,
                                            Type = f.Type,
                                            Optional = f.Optional,
                                            Options = f.Options,
                                            SearchOrder = f.SearchOrder,
                                            SearchShow = f.SearchShow
                                        }).ToList()
                                    }).FirstOrDefaultAsync();

                return Ok(singleResponse);
            }
            else
            {
                var listResponse = await _db.DataTypes.Where(i => i.FolderId == folderId)
                                    .Include(i => i.Fields)
                                    .Select(i => new ResponseDataType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name,
                                        Fields = i.Fields.Select(f => new ResponseField()
                                        {
                                            Id = f.Id,
                                            Name = f.Name,
                                            Row = f.Row,
                                            Column = f.Column,
                                            ColumnSpan = f.ColumnSpan,
                                            Type = f.Type,
                                            Optional = f.Optional,
                                            Options = f.Options,
                                            SearchOrder = f.SearchOrder,
                                            SearchShow = f.SearchShow
                                        }).ToList()
                                    }).ToListAsync();

                return Ok(listResponse);
            }
        }
      

        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/field")]
        async public Task<IActionResult> CreateFolderInstanceTypeField([FromBody]ResponseField model, string folderId, string dataTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            Field field = new Field()
            {
                Name = model.Name,
                Row = model.Row,
                Column = model.Column,
                ColumnSpan = model.ColumnSpan,
                Type = model.Type,
                Optional = model.Optional,
                Options = model.Options,
                SearchOrder = model.SearchOrder,
                SearchShow = model.SearchShow,
                DataTypeId = dataTypeId
            };
            _db.Fields.Add(field);

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = field.Id });
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/field/{fieldId}")]
        async public Task<IActionResult> UpdateFolderInstanceTypeField([FromBody]ResponseField model, string folderId, string dataTypeId, string fieldId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var field = await _db.Fields.Where(f => f.Id == fieldId &&
                                    f.DataTypeId == dataTypeId &&
                                    f.DataType.FolderId == folderId).FirstOrDefaultAsync();

            field.Name = model.Name;
            field.Row = model.Row;
            field.Column = model.Column;
            field.ColumnSpan = model.ColumnSpan;
            field.Type = model.Type;
            field.Optional = model.Optional;
            field.Options = model.Options;
            field.SearchOrder = model.SearchOrder;
            field.SearchShow = model.SearchShow;

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = field.Id });
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/field/{fieldId}")]
        async public Task<IActionResult> DeleteFolderInstanceTypeField(string folderId, string dataTypeId, string fieldId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId, true) == false)
                return BadRequest("Cannot Manage Folder");

            var field = _db.Fields.Where(f => f.Id == fieldId &&
                                    f.DataTypeId == dataTypeId &&
                                    f.DataType.FolderId == folderId);

            _db.Fields.RemoveRange(field);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/search")]
        async public Task<IActionResult> SearchFolderInstancesByType([FromBody]Search model, string folderId, string dataTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            if (folderId == null || dataTypeId == null || model.SortBy == null)
                return Ok(new InstanceSearchResponse());

            BsonDocument sort = default(BsonDocument);
            sort = new BsonDocument
            {
                { model.SortBy, model.SortDirection },
            };

            //Query used in data results and count results. Separate the query from the rest of the pipeline so it can be reused.
            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("FolderId", folderId),
                            new BsonDocument("TypeId", dataTypeId),
                            model.FilterText != null
                                ? new BsonDocument("$text", new BsonDocument {{ "$search", model.FilterText }, { "$caseSensitive", false } })
                                : new BsonDocument("_id", new BsonDocument("$ne", BsonNull.Value)),
                        });

            InstanceSearchResponse response = new InstanceSearchResponse();

            PipelineDefinition<Dictionary<string, string>, Dictionary<string, string>> pipelineData = new BsonDocument[]
            {
                new BsonDocument("$match", query),
                new BsonDocument("$sort", sort),
                new BsonDocument("$skip", model.Page * model.PageSize),
                new BsonDocument("$limit", model.PageSize),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", 0 },
                    })
            };
            //Search
            response.Data = await _mongoDBContext.Instances.Aggregate(pipelineData).ToListAsync();


            PipelineDefinition<Dictionary<string, string>, AggregationTotal> pipelineCountTotal = new BsonDocument[]
            {
                new BsonDocument("$match", query),
                new BsonDocument("$count", "Total")
            };
            AggregationTotal countResponse = await _mongoDBContext.Instances.Aggregate(pipelineCountTotal).FirstOrDefaultAsync();
            if (countResponse != null)
                response.Total = countResponse.Total;

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/instance")]
        async public Task<IActionResult> FolderCreateInstanceByType([FromBody]ResponseInstance model, string folderId, string dataTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            var user = await _userManager.FindByIdAsync(userId);

            model.InstanceData.Add("InstanceId", Guid.NewGuid().ToString());
            model.InstanceData.Add("FolderId", folderId);
            model.InstanceData.Add("TypeId", dataTypeId);
            model.InstanceData.Add("CreatedOn", model.LocalDateTime);
            model.InstanceData.Add("CreatedBy", user.Email);
            model.InstanceData.Add("UpdatedOn", null);
            model.InstanceData.Add("UpdatedBy", null);
            await _mongoDBContext.Instances.InsertOneAsync(model.InstanceData);

            return Ok(new ResponseId() { Id = model.InstanceData["InstanceId"] });
        }


        [Authorize]
        [HttpPut]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> FolderUpdateInstanceByType([FromBody]ResponseInstance model, string folderId, string dataTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            var user = await _userManager.FindByIdAsync(userId);

            var query = new BsonDocument("$and",
                       new BsonArray
                       {
                            new BsonDocument("FolderId", folderId),
                            new BsonDocument("TypeId", dataTypeId),
                            new BsonDocument("InstanceId", instanceId)
                       });

            model.InstanceData["UpdatedOn"] = model.LocalDateTime;
            model.InstanceData["UpdatedBy"] = user.Email;

            await _mongoDBContext.Instances.ReplaceOneAsync(query, model.InstanceData);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> FolderDeleteInstanceByType(string folderId, string dataTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("FolderId", folderId),
                            new BsonDocument("TypeId", dataTypeId),
                            new BsonDocument("InstanceId", instanceId)
                        });

            await _mongoDBContext.Instances.DeleteOneAsync(query);

            return Ok();
        }


        [Authorize]
        [HttpGet]
        [Route("api/v1/folder/{folderId}/type/{dataTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> FolderGetInstanceById(string folderId, string dataTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageFolder(userId, folderId) == false)
                return BadRequest("Cannot Manage Folder");

            //Query used in data results and count results. Separate the query from the rest of the pipeline so it can be reused.
            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("FolderId", folderId),
                            new BsonDocument("TypeId", dataTypeId),
                            new BsonDocument("InstanceId", instanceId)
                        });

            PipelineDefinition<Dictionary<string, string>, Dictionary<string, string>> pipelineData = new BsonDocument[]
            {
                new BsonDocument("$match", query),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", 0 },
                    })
            };

            //Search
            var data = await _mongoDBContext.Instances.Aggregate(pipelineData).FirstOrDefaultAsync();

            //Make sure the response object has all the fields defined
            DataType instanceType = await _db.DataTypes
                                                    .Include(i => i.Fields)
                                                    .Where(i => i.FolderId == folderId && i.Id == dataTypeId)
                                                    .FirstOrDefaultAsync();
            foreach (var field in instanceType.Fields)
            {
                if (data.ContainsKey(field.Id) == false)
                {
                    data.Add(field.Id, null);
                }
            }

            ResponseInstance response = new ResponseInstance()
            {
                Id = instanceId,
                InstanceData = data
            };

            return Ok(response);
        }
        
        async private Task<bool> CanManageFolder(string userId, string folderId, bool? requireAdmin = null)
        {
            if (requireAdmin == null)
            {
                return await _db.FolderAuthorizedUsers
                                .AnyAsync(o => o.FolderId == folderId &&
                                            o.ApplicationUserId == userId);
            }
            else if (requireAdmin != null)
            {
                return await _db.FolderAuthorizedUsers
                                .AnyAsync(o => o.FolderId == folderId &&
                                            o.ApplicationUserId == userId &&
                                            o.IsFolderAdmin == requireAdmin);
            }

            return false;
        }
    }
}