﻿using System;
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
        async public Task<IActionResult> GroupCreate(string GroupId)
        {
            string userId = User.GetUserId();

            var response = await _db.Groups.Where(c => c.Id == GroupId)
                                    .Include(o => o.AuthorizedUsers)
                                    .Select(o => new ResponseGroup()
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        AuthorizedUsers = o.AuthorizedUsers
                                                            .Select(a => new ResponseGroupAuthorizedUser()
                                                            {
                                                                Id = a.Id,
                                                                ApplicationUserId = a.ApplicationUserId,
                                                                ApplicationUserEmail = a.ApplicationUser.Email,
                                                                IsGroupAdmin = a.IsGroupAdmin
                                                            }).ToList()
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
                                    Name = o.Group.Name
                                }).ToListAsync();

            return Ok(Groups);
        }


        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}")]
        async public Task<IActionResult> UpdateGroupChangeName([FromBody]GroupCreateEditRequestModel model, string GroupId)
        {
            string userId = User.GetUserId();

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

            var removeThese = _db.GroupAuthorizedUsers.Where(o => o.GroupId == GroupId && o.ApplicationUserId == applicationUserId);
            _db.GroupAuthorizedUsers.RemoveRange(removeThese);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("api/v1/Group/{GroupId}/user/{applicationUserId}/authorized/toggle")]
        async public Task<IActionResult> UpdateGroupRemoveUserAuthorized([FromBody]GroupToggleAuthorizedModel model, string GroupId, string applicationUserId)
        {
            string userId = User.GetUserId();

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

            var authorizations = _db.GroupAuthorizedUsers.Where(o => o.GroupId == GroupId);
            _db.GroupAuthorizedUsers.RemoveRange(authorizations);

            var Group = _db.Groups.Where(o => o.Id == GroupId);
            _db.Groups.RemoveRange(Group);

            await _db.SaveChangesAsync();

            return Ok();
        }



        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type")]
        async public Task<IActionResult> CreateGroupType([FromBody]ResponseInstanceType model, string GroupId)
        {
            string userId = User.GetUserId();

            InstanceType newType = new InstanceType()
            {
                Name = model.Name,
                GroupId = GroupId
            };

            InstanceAuthorizedUser instanceAuthorizedUser = new InstanceAuthorizedUser()
            {
                ApplicationUserId = userId,
                CanRead = true,
                CanWrite = true,
                InstanceType = newType,
                InstanceTypeId = newType.Id
            };

            _db.InstanceTypes.Add(newType);
            _db.InstanceAuthorizedUsers.Add(instanceAuthorizedUser);

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = newType.Id });
        }

        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}")]
        [Route("api/v1/Group/{GroupId}/type")]
        async public Task<IActionResult> GetGroupType(string GroupId, string instanceTypeId = null)
        {
            string userId = User.GetUserId();

            if(instanceTypeId == "menu")
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
                                    .Include(i => i.AuthorizedUsers)
                                    .Select(i => new ResponseInstanceType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name,
                                        AuthorizedUsers = i.AuthorizedUsers.Select(a => new ResponseInstanceAuthorizedUser()
                                        {
                                            Id = a.Id,
                                            ApplicationUserEmail = a.ApplicationUser.Email,
                                            ApplicationUserId = a.ApplicationUserId,
                                            CanRead = a.CanRead,
                                            CanWrite = a.CanWrite
                                        }).ToList(),
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
                var listResponse = await _db.InstanceTypes.Where(i => i.GroupId == GroupId)
                                    .Include(i => i.Fields)
                                    .Include(i => i.AuthorizedUsers)
                                    .Select(i => new ResponseInstanceType()
                                    {
                                        Id = i.Id,
                                        Name = i.Name,
                                        AuthorizedUsers = i.AuthorizedUsers.Select(a => new ResponseInstanceAuthorizedUser()
                                        {
                                            Id = a.Id,
                                            ApplicationUserEmail = a.ApplicationUser.Email,
                                            ApplicationUserId = a.ApplicationUserId,
                                            CanRead = a.CanRead,
                                            CanWrite = a.CanWrite
                                        }).ToList(),
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
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/field")]
        async public Task<IActionResult> CreateGroupInstanceTypeField([FromBody]ResponseField model, string GroupId, string instanceTypeId)
        {
            string userId = User.GetUserId();

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
                InstanceTypeId = instanceTypeId
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

            await _db.SaveChangesAsync();

            return Ok(new ResponseId() { Id = field.Id });
        }

        [Authorize]
        [HttpDelete]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/field/{fieldId}")]
        async public Task<IActionResult> DeleteGroupInstanceTypeField(string GroupId, string instanceTypeId, string fieldId)
        {
            string userId = User.GetUserId();

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

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance")]
        async public Task<IActionResult> GroupCreateInstanceByType([FromBody]ResponseInstance model, string GroupId, string instanceTypeId)
        {
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
            var query = new BsonDocument("$and",
                        new BsonArray
                        {
                            new BsonDocument("GroupId", GroupId),
                            new BsonDocument("TypeId", instanceTypeId),
                            new BsonDocument("InstanceId", instanceId)
                        });

            await _mongoDBContext.Instances.DeleteOneAsync(query);

            return Ok();
        }


        [Authorize]
        [HttpGet]
        [Route("api/v1/Group/{GroupId}/type/{instanceTypeId}/instance/{instanceId}")]
        async public Task<IActionResult> GroupGetInstanceById(string GroupId, string instanceTypeId, string instanceId)
        {
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
    }
}