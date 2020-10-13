using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using template.Server.Data;
using template.Server.Entities;
using template.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using template.Server.Models;
using template.Server.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CRM.Server.Controllers
{
    [ApiController]
    public class DataController : ControllerBase
    {
        private ApplicationDbContext _db;
        //private MongoDBContext _mongoDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public DataController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }



        [HttpPost]
        [Route("api/v1/customer")]
        [Authorize]
        async public Task<IActionResult> AccountCreate([FromBody] CustomerRequest model)
        {
            string userId = User.GetUserId();

            Customer customer = new Customer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Postal = model.Postal,
                Notes = model.Notes,
                BirthDate = model.BirthDate,
                OwnerId = userId
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return Ok(customer);
        }

        [HttpGet]
        [Route("api/v1/customer/{id}")]
        [Authorize]
        async public Task<IActionResult> CustomerGetById(string id)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.OwnerId == id && c.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            return Ok(customer);
        }

        [HttpPut]
        [Route("api/v1/customer/{id}")]
        [Authorize]
        async public Task<IActionResult> CustomerUpdateById([FromBody] CustomerRequest model, string id)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.OwnerId == id && c.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest();

            customer.Name = model.Name;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.City = model.City;
            customer.State = model.State;
            customer.Postal = model.Postal;
            customer.Notes = model.Notes;

            await _db.SaveChangesAsync();

            return Ok(customer);
        }


        [Authorize]
        [HttpPost]
        [Route("api/v1/customers")]
        async public Task<IActionResult> CustomerSearch([FromBody] Search model)
        {
            string userId = User.GetUserId();

            var query = _db.Customers.Where(c => c.OwnerId == userId);

            if (!string.IsNullOrEmpty(model.FilterText))
            {
                query = query.Where(i => i.Name.Contains(model.FilterText) ||
                                        i.Email.Contains(model.FilterText) ||
                                        i.Phone.Contains(model.FilterText) ||
                                        i.Address.Contains(model.FilterText) ||
                                        i.State.Contains(model.FilterText) ||
                                        i.Postal.Contains(model.FilterText) ||
                                        i.Notes.Contains(model.FilterText));
            }

            CustomerSearchResponse response = new CustomerSearchResponse();
            response.Total = await query.CountAsync();

            var dataResponse = await query.Skip(model.Page * model.PageSize)
                                        .Take(model.PageSize)
                                        .ToListAsync();

            response.Data = dataResponse.Select(i => new CustomerSlimResponse()
            {
                Id = i.Id,
                Name = i.Name,
                Email = i.Email,
                Phone = i.Phone
            }).ToList();

            return Ok(response);
        }



    }//End Controller
}//End Namespace