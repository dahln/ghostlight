using System;
using System.Collections.Generic;
using System.Text;

namespace depot.Shared.RequestModels
{
    public class FolderCreateEditRequestModel
    {
        public string Name { get; set; }
    }

    public class FolderAddAuthorizedEmailModel
    {
        public string Email { get; set; }
    }

    public class FolderToggleAuthorizedModel
    {
        public bool Administrator { get; set; }
    }

    public class Search
    {
        public string FilterText { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 25;
    }
}
