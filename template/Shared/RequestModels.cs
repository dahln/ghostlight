using System;
using System.Collections.Generic;
using System.Text;

namespace template.Shared
{
    public class Search
    {
        public string FilterText { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 25;
    }

    public class CustomerRequest
    {
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
}
