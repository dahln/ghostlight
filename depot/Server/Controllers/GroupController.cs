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
    public class GroupController : ControllerBase
    {
        private ApplicationDbContext _db;
        private MongoDBContext _mongoDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public GroupController(ApplicationDbContext dbContext, MongoDBContext mongoDBContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
            _mongoDBContext = mongoDBContext;
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group")]
        async public Task<IActionResult> GroupCreate([FromBody]GroupCreateEditRequestModel model)
        {
            string userId = User.GetUserId();
            
            Group Group = new Group()
            {
                Name = model.Name,
            };
            _db.Groups.Add(Group);

            //Create authorization for creating user
            GroupAuthorizedUser GroupAuthorizedUser = new GroupAuthorizedUser()
            {
                IsGroupAdmin = true,
                GroupId = Group.Id,
                Group = Group,
                ApplicationUserId = userId
            };
            _db.GroupAuthorizedUsers.Add(GroupAuthorizedUser);
            
            await _db.SaveChangesAsync();

            ResponseId response = new ResponseId()
            {
                Id = Group.Id
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}")]
        async public Task<IActionResult> GetGroupById(string GroupId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var response = await _db.Groups.Where(c => c.Id == GroupId)
                                    .Include(o => o.AuthorizedUsers)
                                    .Select(o => new ResponseGroup()
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        AuthorizedUsers = o.AuthorizedUsers.Any(u => u.ApplicationUserId == userId && u.IsGroupAdmin == true) ? o.AuthorizedUsers
                                                            .Select(a => new ResponseGroupAuthorizedUser()
                                                            {
                                                                Id = a.Id,
                                                                ApplicationUserId = a.ApplicationUserId,
                                                                ApplicationUserEmail = a.ApplicationUser.Email,
                                                                IsGroupAdmin = a.IsGroupAdmin
                                                            }).ToList() : null
                                    }).FirstOrDefaultAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/user/authorized")]
        async public Task<IActionResult> GetGroupsByAuthorization()
        {
            string userId = User.GetUserId();

            var Groups = await _db.GroupAuthorizedUsers.Where(o => o.ApplicationUserId == userId)
                                .Include(o => o.Group)
                                .Select(o => new ResponseGroupShort()
                                {
                                    Id = o.Group.Id,
                                    Name = o.Group.Name,
                                    IsAdministrator = o.IsGroupAdmin
                                }).ToListAsync();

            return Ok(Groups);
        }


        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}")]
        async public Task<IActionResult> UpdateGroupChangeName([FromBody]GroupCreateEditRequestModel model, string GroupId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var Group = await _db.Groups.Where(o => o.Id == GroupId).FirstOrDefaultAsync();
            Group.Name = model.Name;

            await _db.SaveChangesAsync();

            return Ok();
        }

        

        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/user/authorized")]
        async public Task<IActionResult> UpdateGroupSetUserAuthorized([FromBody]GroupAddAuthorizedEmailModel model, string GroupId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var foundUserByEmail = await _userManager.FindByNameAsync(model.Email);
            if (foundUserByEmail == null)
                return BadRequest("User Not Found");

            var Group = await _db.Groups.FirstOrDefaultAsync(o => o.Id == GroupId);
            if (Group == null)
                return BadRequest("Group not found");

            GroupAuthorizedUser authorizedUser = new GroupAuthorizedUser()
            {
                GroupId = Group.Id,
                Group = Group,
                ApplicationUser = foundUserByEmail,
                ApplicationUserId = foundUserByEmail.Id
            };

            _db.GroupAuthorizedUsers.Add(authorizedUser);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/user/{applicationUserId}/authorized")]
        async public Task<IActionResult> UpdateGroupRemoveUserAuthorized(string GroupId, string applicationUserId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var removeThese = _db.GroupAuthorizedUsers.Where(o => o.GroupId == GroupId && o.ApplicationUserId == applicationUserId);
            _db.GroupAuthorizedUsers.RemoveRange(removeThese);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/user/{applicationUserId}/authorized/toggle")]
        async public Task<IActionResult> UpdateGroupRemoveUserToggle([FromBody]GroupToggleAuthorizedModel model, string GroupId, string applicationUserId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var updateThis = await _db.GroupAuthorizedUsers.Where(o => o.GroupId == GroupId && o.ApplicationUserId == applicationUserId).FirstOrDefaultAsync();
            updateThis.IsGroupAdmin = model.Administrator;

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}")]
        async public Task<IActionResult> DeleteGroup(string GroupId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var authorizations = _db.GroupAuthorizedUsers.Where(o => o.GroupId == GroupId);
            _db.GroupAuthorizedUsers.RemoveRange(authorizations);

            var Group = _db.Groups.Where(o => o.Id == GroupId);
            _db.Groups.RemoveRange(Group);

            var links = _db.InstanceLinks.Where(l => l.GroupId == GroupId);
            _db.InstanceLinks.RemoveRange(links);

            await _db.SaveChangesAsync();

            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("GroupId", GroupId)
                        });

            await _mongoDBContext.Instances.DeleteManyAsync(query);


            return Ok();
        }



        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type")]
        async public Task<IActionResult> CreateGroupType([FromBody]ResponseInstanceType model, string GroupId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            InstanceType newType = new InstanceType()
            {
                Name = model.Name,
                GroupId = GroupId
            };
            
            _db.InstanceTypes.Add(newType);

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = newType.Id });
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}")]
        async public Task<IActionResult> UpdateGroupTypeName([FromBody]ResponseInstanceType model, string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var type = await _db.InstanceTypes.Where(d => d.GroupId == GroupId && d.Id == instanceTypeId).FirstOrDefaultAsync();
            type.Name = model.Name;
            
            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = type.Id });
        }


        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}")]
        async public Task<IActionResult> DeleteGroupTypeById(string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            //Delete the type
            var deleteThis = _db.InstanceTypes.Where(d => d.GroupId == GroupId && d.Id == instanceTypeId);
            _db.InstanceTypes.RemoveRange(deleteThis);
            
            //Delete links of instances associated with this type.
            var links = _db.InstanceLinks.Where(l => l.GroupId == GroupId && (l.LinkId1_TypeId == instanceTypeId || l.LinkId2_TypeId == instanceTypeId));
            _db.InstanceLinks.RemoveRange(links);

            await _db.SaveChangesAsync();

            var query = new BsonDocument("$and",
                            new BsonArray
                            {
                                new BsonDocument("GroupId", GroupId),
                                new BsonDocument("TypeId", instanceTypeId)
                            });
            await _mongoDBContext.Instances.DeleteManyAsync(query);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}")]
        [Route("api/v1/Group/{GroupId}/type")]
        async public Task<IActionResult> GetGroupType(string GroupId, string instanceTypeId = null)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            if (instanceTypeId == "menu")
            {
                var listResponse = await _db.InstanceTypes.Where(i => i.GroupId == GroupId)
                                    .Select(i => new ResponseInstanceType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name
                                    }).ToListAsync();

                return Ok(listResponse);
            }

            if (instanceTypeId != null)
            {
                var singleResponse = await _db.InstanceTypes.Where(i => i.Id == instanceTypeId && i.GroupId == GroupId)
                                    .Include(i => i.Fields)
                                    .Select(i => new ResponseInstanceType()
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
                                            SearchShow = f.SearchShow,
                                            Primary = f.Primary
                                        }).ToList()
                                    }).FirstOrDefaultAsync();

                return Ok(singleResponse);
            }
            else
            {
                var listResponse = await _db.InstanceTypes.Where(i => i.GroupId == GroupId)
                                    .Include(i => i.Fields)
                                    .Select(i => new ResponseInstanceType()
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
                                            SearchShow = f.SearchShow,
                                            Primary = f.Primary
                                        }).ToList()
                                    }).ToListAsync();

                return Ok(listResponse);
            }
        }
      

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/field")]
        async public Task<IActionResult> CreateGroupInstanceTypeField([FromBody]ResponseField model, string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var existingPrimary = await _db.Fields.AnyAsync(f => f.InstanceTypeId == instanceTypeId
                                                                && f.InstanceType.GroupId == GroupId
                                                                && f.Primary == true);
            if (existingPrimary == true && model.Primary == true)
                return BadRequest("There is already a primary field on this type - unselect 'Primary' to create this field.");


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
                InstanceTypeId = instanceTypeId,
                Primary = model.Primary
            };
            _db.Fields.Add(field);

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = field.Id });
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/field/{fieldId}")]
        async public Task<IActionResult> UpdateGroupInstanceTypeField([FromBody]ResponseField model, string GroupId, string instanceTypeId, string fieldId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var existingPrimary = await _db.Fields.FirstOrDefaultAsync(f => f.InstanceTypeId == instanceTypeId 
                                                                && f.InstanceType.GroupId == GroupId
                                                                && f.Primary == true);

            //If a primary does NOT exist and the fieldId is NOT the current primary, then throw the error
            if (existingPrimary != null && fieldId != existingPrimary.Id)
                return BadRequest("This type already has a 'primary' field. Unselect the existing primary before selecting a new primary.");

            var field = await _db.Fields.Where(f => f.Id == fieldId &&
                                    f.InstanceTypeId == instanceTypeId &&
                                    f.InstanceType.GroupId == GroupId).FirstOrDefaultAsync();

            field.Name = model.Name;
            field.Row = model.Row;
            field.Column = model.Column;
            field.ColumnSpan = model.ColumnSpan;
            field.Type = model.Type;
            field.Optional = model.Optional;
            field.Options = model.Options;
            field.SearchOrder = model.SearchOrder;
            field.SearchShow = model.SearchShow;
            field.Primary = model.Primary;

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = field.Id });
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/field/{fieldId}")]
        async public Task<IActionResult> DeleteGroupInstanceTypeField(string GroupId, string instanceTypeId, string fieldId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId, true) == false)
                return BadRequest("Cannot manage group");

            var field = _db.Fields.Where(f => f.Id == fieldId &&
                                    f.InstanceTypeId == instanceTypeId &&
                                    f.InstanceType.GroupId == GroupId);

            _db.Fields.RemoveRange(field);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/search")]
        async public Task<IActionResult> SearchGroupInstancesByType([FromBody]Search model, string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            if (GroupId == null || instanceTypeId == null || model.SortBy == null)
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
                            new BsonDocument("GroupId", GroupId),
                            new BsonDocument("TypeId", instanceTypeId),
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

            if(model.OnlyPrimary)
            {
                var primaryField = await _db.Fields.Where(f => f.InstanceTypeId == instanceTypeId && f.Primary == true).FirstOrDefaultAsync();
                for(int a = 0; a < response.Data.Count; a++)
                {
                    response.Data[a] = response.Data[a].Where(f => f.Key == "InstanceId" || f.Key == primaryField.Id).ToDictionary(t => t.Key, t => t.Value);
                }
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance")]
        async public Task<IActionResult> GroupCreateInstanceByType([FromBody]ResponseInstance model, string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            model.InstanceData.Add("InstanceId", Guid.NewGuid().ToString());
            model.InstanceData.Add("GroupId", GroupId);
            model.InstanceData.Add("TypeId", instanceTypeId);
            await _mongoDBContext.Instances.InsertOneAsync(model.InstanceData);

            return Ok(new ResponseId() { Id = model.InstanceData["InstanceId"] });
        }


        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> GroupUpdateInstanceByType([FromBody]ResponseInstance model, string GroupId, string instanceTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var query = new BsonDocument("$and",
                       new BsonArray
                       {
                            new BsonDocument("GroupId", GroupId),
                            new BsonDocument("TypeId", instanceTypeId),
                            new BsonDocument("InstanceId", instanceId)
                       });

            await _mongoDBContext.Instances.ReplaceOneAsync(query, model.InstanceData);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> GroupDeleteInstanceByType(string GroupId, string instanceTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("GroupId", GroupId),
                            new BsonDocument("TypeId", instanceTypeId),
                            new BsonDocument("InstanceId", instanceId)
                        });

            await _mongoDBContext.Instances.DeleteOneAsync(query);

            var links = _db.InstanceLinks.Where(l => l.GroupId == GroupId && (l.LinkId1 == instanceId || l.LinkId2 == instanceId));
            _db.InstanceLinks.RemoveRange(links);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/link/{instanceId}")]
        async public Task<IActionResult> GetLinksForInstanceId(string GroupId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var links = await _db.InstanceLinks
                                .Where(i => i.LinkId1 == instanceId || i.LinkId2 == instanceId)
                                .Select(i => new LinkedInstanceResponse()
                                {
                                    Id = i.Id,
                                    GroupId = i.GroupId,
                                    LinkId1 = i.LinkId1,
                                    LinkId2 = i.LinkId2
                                })
                                .ToListAsync();

            return Ok(links);
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/link/type/{linkId1_TypeId}/instance/{linkId1}/type/{linkId2_TypeId}/instance/{linkId2}")]
        async public Task<IActionResult> LinkInstanceByInstanceIds(string GroupId, string linkId1_TypeId, string linkId1, string linkId2_TypeId, string linkId2)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var type1Fields = await _db.InstanceTypes.Where(t => t.Id == linkId1_TypeId).SelectMany(t => t.Fields).ToListAsync();
            var type2Fields = await _db.InstanceTypes.Where(t => t.Id == linkId2_TypeId).SelectMany(t => t.Fields).ToListAsync();

            if (type1Fields.Any(f => f.Primary == true) || type2Fields.Any(f => f.Primary == true))
            {

                InstanceLink link = new InstanceLink()
                {
                    GroupId = GroupId,
                    LinkId1 = linkId1,
                    LinkId2 = linkId2,
                    LinkId1_TypeId = linkId1_TypeId,
                    LinkId2_TypeId = linkId2_TypeId
                };

                _db.InstanceLinks.Add(link);
                await _db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest("Both linking types must have a primary field selected");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/link/{linkId}")]
        async public Task<IActionResult> UnLinkInstanceByInstanceIds(string GroupId, string linkId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            var deleteThis = _db.InstanceLinks.Where(l => l.Id == linkId);

            _db.RemoveRange(deleteThis);
            await _db.SaveChangesAsync();

            return Ok();
        }


        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> GroupGetInstanceById(string GroupId, string instanceTypeId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            //Query used in data results and count results. Separate the query from the rest of the pipeline so it can be reused.
            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("GroupId", GroupId),
                            new BsonDocument("TypeId", instanceTypeId),
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
            InstanceType instanceType = await _db.InstanceTypes
                                                    .Include(i => i.Fields)
                                                    .Where(i => i.GroupId == GroupId && i.Id == instanceTypeId)
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

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/instance/{instanceId}/primary")]
        async public Task<IActionResult> GroupGetInstancePrimaryValueById(string GroupId, string instanceId)
        {
            string userId = User.GetUserId();
            if (await CanManageGroup(userId, GroupId) == false)
                return BadRequest("Cannot manage group");

            //Query used in data results and count results. Separate the query from the rest of the pipeline so it can be reused.
            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("GroupId", GroupId),
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
            InstanceType instanceType = await _db.InstanceTypes
                                                    .Include(i => i.Fields)
                                                    .Where(i => i.GroupId == GroupId && i.Id == data["TypeId"])
                                                    .FirstOrDefaultAsync();

            var primary = instanceType.Fields.FirstOrDefault(f => f.Primary == true);

            ResponsePrimaryValue response = new ResponsePrimaryValue()
            {
                Id = instanceId,
                DataType = data["TypeId"],
                DataTypeName = instanceType.Name,
                Value = data.Where(f => f.Key == primary.Id).FirstOrDefault().Value
            };

            return Ok(response);
        }

        async private Task<bool> CanManageGroup(string userId, string groupId, bool? requireAdmin = null)
        {
            if (requireAdmin == null)
            {
                return await _db.GroupAuthorizedUsers
                                .AnyAsync(o => o.GroupId == groupId &&
                                            o.ApplicationUserId == userId);
            }
            else if (requireAdmin != null)
            {
                return await _db.GroupAuthorizedUsers
                                .AnyAsync(o => o.GroupId == groupId &&
                                            o.ApplicationUserId == userId &&
                                            o.IsGroupAdmin == requireAdmin);
            }

            return false;
        }
    }
}