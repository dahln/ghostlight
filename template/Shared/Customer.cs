using System;
using System.Collections.Generic;
using System.Text;
using template.Shared.Enumerations;

namespace template.Shared
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
    }

    public class CustomerSlim
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
    }
}
