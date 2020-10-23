using ghostlight.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ghostlight.Shared.Enumerations;

namespace ghostlight.Server.Entities
{
    public class Customer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public DateTime BirthDate { get; set; }
        public string Notes { get; set; }

        public Gender Gender { get; set; }
        public bool Active { get; set; }


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateOn { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
