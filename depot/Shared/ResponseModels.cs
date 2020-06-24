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

    public class ResponseGroupShort
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public bool IsAdministrator { get; set; } = false;
    }




    public class ResponseGroup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<ResponseInstanceType> InstanceTypes { get; set; } = new List<ResponseInstanceType>();
        public List<ResponseGroupAuthorizedUser> AuthorizedUsers { get; set; } = new List<ResponseGroupAuthorizedUser>();
    }

    public class ResponseGroupAuthorizedUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsGroupAdmin { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public string ApplicationUserEmail { get; set; }
    }




    public class ResponseInstanceType
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
        public bool SearchShow { get; set; } = false;
        public int SearchOrder { get; set; } = 1;
        public bool Primary { get; set; } = false;
    }


    public class ResponseInstance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Dictionary<string, string> InstanceData { get; set; } = new Dictionary<string, string>();
    }

    public class ResponsePrimaryValue
    { 
        public string Id { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }

    public class InstanceSearchResponse
    {
        public List<Dictionary<string, string>> Data { get; set; } = new List<Dictionary<string, string>>();
        public int Total { get; set; }
    }

    public class AggregationTotal
    {
        public int Total { get; set; }
    }

    public class LinkedInstanceResponse
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string LinkId1 { get; set; }
        public string LinkId2 { get; set; }
    }
}
