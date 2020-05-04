using depot.Server.Models;
using depot.Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Server.Entities
{
    public class Group
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<InstanceType> InstanceTypes { get; set; } = new List<InstanceType>();
        public List<GroupAuthorizedUser> AuthorizedUsers { get; set; } = new List<GroupAuthorizedUser>();
    }

    public class GroupAuthorizedUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsGroupAdmin { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string GroupId { get; set; }
        public Group Group { get; set; }
    }




    public class InstanceType
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();

        public string GroupId { get; set; }
        public Group Group { get; set; }
    }
    
    public class Field
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

        public string InstanceTypeId { get; set; }
        public InstanceType InstanceType { get; set; }
    }
}
