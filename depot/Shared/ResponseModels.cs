using depot.Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace depot.Shared.ResponseModels
{
    public class ResponseId
    {
        public string Id { get; set; }
    }

    public class ResponseFolderShort
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public bool IsAdministrator { get; set; } = false;
    }




    public class ResponseFolder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<ResponseDataType> DataeTypes { get; set; } = new List<ResponseDataType>();
        public List<ResponseFolderAuthorizedUser> AuthorizedUsers { get; set; } = new List<ResponseFolderAuthorizedUser>();
    }

    public class ResponseFolderAuthorizedUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsFolderAdmin { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public string ApplicationUserEmail { get; set; }
    }




    public class ResponseDataType
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<ResponseField> Fields { get; set; } = new List<ResponseField>();

        public List<ResponseInstance> Instances { get; set; } = new List<ResponseInstance>();
    }

    public class ResponseField
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public int Row { get; set; } = 1;
        public int Column { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;
        public string Options { get; set; }
        public bool Optional { get; set; } = true;
        public bool SearchShow { get; set; } = true;
        public int SearchOrder { get; set; } = 1;
    }

    public class ResponseInstance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public string CreatedByEmail { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
        public string UpdatedByEmail { get; set; }
    }


    public class InstanceSearchResponse
    {
        public List<ResponseInstance> Data { get; set; } = new List<ResponseInstance>();
        public int Total { get; set; }
    }

    public class AggregationTotal
    {
        public int Total { get; set; }
    }
}
