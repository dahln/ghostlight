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
using template.Server.Utility;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using template.Shared.Enumerations;

namespace template.Server.Controllers
{
    [ApiController]
    public class DataController : ControllerBase
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("api/v1/customer")]
        [Authorize]
        async public Task<IActionResult> CustomerCreate([FromBody] Shared.Customer model)
        {
            string userId = User.GetUserId();

            if(string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Customer name is required");
            }

            template.Server.Entities.Customer customer = new template.Server.Entities.Customer()
            {
                Name = model.Name,
                Gender = model.Gender,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Postal = model.Postal,
                Notes = model.Notes,
                BirthDate = model.BirthDate,
                Active = model.Active,
                OwnerId = userId
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            Shared.Customer response = customer.ToSharedCustomer();

            return Ok(response);
        }

        [HttpGet]
        [Route("api/v1/customer/{id}")]
        [Authorize]
        async public Task<IActionResult> CustomerGetById(string id)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.OwnerId == userId && c.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            Shared.Customer response = customer.ToSharedCustomer();

            return Ok(response);
        }

        [HttpPut]
        [Route("api/v1/customer/{id}")]
        [Authorize]
        async public Task<IActionResult> CustomerUpdateById([FromBody] Shared.Customer model, string id)
        {
            string userId = User.GetUserId();

            if (string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Customer name is required");
            }

            var customer = await _db.Customers.Where(c => c.OwnerId == userId && c.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            customer.Name = model.Name;
            customer.Gender = model.Gender;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.City = model.City;
            customer.State = model.State;
            customer.Postal = model.Postal;
            customer.Notes = model.Notes;
            customer.Active = model.Active;
            customer.UpdateOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            Shared.Customer response = customer.ToSharedCustomer();

            return Ok(response);
        }

        [HttpDelete]
        [Route("api/v1/customer/{id}")]
        [Authorize]
        async public Task<IActionResult> CustomerDeleteById(string id)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.OwnerId == userId && c.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            return Ok();
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
                query = query.Where(i => i.Name.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Email.ToLower().ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Phone.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Address.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.State.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Postal.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Notes.ToLower().Contains(model.FilterText.ToLower()));
            }

            if(model.SortBy == nameof(Entities.Customer.Name))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Name)
                            : query.OrderByDescending(c => c.Name);
            }
            else if (model.SortBy == nameof(Entities.Customer.State))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.State)
                            : query.OrderByDescending(c => c.State);
            }
            else if (model.SortBy == nameof(Entities.Customer.Active))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Active)
                            : query.OrderByDescending(c => c.Active);
            }
            else if (model.SortBy == nameof(Entities.Customer.Gender))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Gender)
                            : query.OrderByDescending(c => c.Gender);
            }
            else
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Name)
                            : query.OrderByDescending(c => c.Name);
            }


            SearchResponse<Shared.CustomerSlim> response = new SearchResponse<Shared.CustomerSlim>();
            response.Total = await query.CountAsync();

            var dataResponse = await query.Skip(model.Page * model.PageSize)
                                        .Take(model.PageSize)
                                        .ToListAsync();

            response.Data = dataResponse.Select(i => i.ToSharedCustomerSlim()).ToList();

            return Ok(response);
        }


        [HttpGet]
        [Authorize]
        [Route("api/v1/seed/create/{number}")]
        async public Task<IActionResult> SeedCustomers(int number)
        {
            string userId = User.GetUserId();

            for (int a = 0; a < number; a++)
            {
                var customer = new Entities.Customer()
                {
                    Name = LoremNET.Lorem.Words(2),
                    Gender = (Gender)LoremNET.Lorem.Number(0, 2),
                    Email = LoremNET.Lorem.Email(),
                    Phone = LoremNET.Lorem.Number(1111111111, 9999999999).ToString(),
                    Address = $"{LoremNET.Lorem.Number(100, 10000).ToString()} {LoremNET.Lorem.Words(1)}",
                    City = LoremNET.Lorem.Words(1),
                    State = LoremNET.Lorem.Words(1),
                    Postal = LoremNET.Lorem.Number(11111, 99999).ToString(),
                    BirthDate = LoremNET.Lorem.DateTime(1923, 1, 1),
                    Notes = LoremNET.Lorem.Paragraph(5, 10, 10),
                    Active = LoremNET.Lorem.Number(0, 1) == 0 ? false : true,
                    CreatedOn = LoremNET.Lorem.DateTime(2020,1,1),
                    OwnerId = userId,
                };

                _db.Customers.Add(customer);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/seed/clear")]
        async public Task<IActionResult> SeedClear()
        {
            string userId = User.GetUserId();

            var clear = _db.Customers.Where(c => c.Id != null && c.OwnerId == userId);
            _db.Customers.RemoveRange(clear);

            await _db.SaveChangesAsync();

            return Ok();
        }
    }//End Controller
}//End Namespace