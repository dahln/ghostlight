using template.Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace template.Shared
{
    public class ResponseId
    {
        public string Id { get; set; }
    }

    public class CustomerResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public DateTime BirthDate { get; set; }
        public string Notes { get; set; }
    }

    public class CustomerSlimResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CustomerSearchResponse
    {
        public List<CustomerSlimResponse> Data { get; set; } = new List<CustomerSlimResponse>();
        public int Total { get; set; }
    }
}
