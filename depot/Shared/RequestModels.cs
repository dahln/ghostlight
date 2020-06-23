using System;
using System.Collections.Generic;
using System.Text;

namespace depot.Shared.RequestModels
{
    public class GroupCreateEditRequestModel
    {
        public string Name { get; set; }
    }

    public class GroupAddAuthorizedEmailModel
    {
        public string Email { get; set; }
    }

    public class GroupToggleAuthorizedModel
    {
        public bool Administrator { get; set; }
    }

    public class Search
    {
        public string FilterText { get; set; }
        public string SortBy { get; set; }
        public int SortDirection { get; set; } = 1;
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 25;
        public bool OnlyPrimary { get; set; } = false;
    }
}
