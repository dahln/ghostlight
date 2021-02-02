using System;
using System.Collections.Generic;
using System.Text;
using ghostlight.Shared.Enumerations;

namespace ghostlight.Shared
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
        public string ImageBase64 { get; set; }

        public override int GetHashCode()
        {
            //Tuple can't hold 13. Break properties into batchs
            var firstBatch = Tuple.Create(Id, Name, Email, Phone, Address, City, State).GetHashCode();
            var secondBatch = Tuple.Create(Postal, BirthDate.ToString("yyyy/mm/dd"), Notes, Gender, Active, ImageBase64).GetHashCode();

            return firstBatch + secondBatch;
        }
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
