using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using template.Server.Entities;
using template.Shared;

namespace template.Server.Helpers
{
    static public class ToCustomerSlim
    {
        static public CustomerSlimResponse Convert(Customer model)
        {
            var customer = new CustomerSlimResponse()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone
            };

            return customer;
        }
    }
}
