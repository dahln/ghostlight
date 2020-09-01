using glubfish.Server.Models;
using glubfish.Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glubfish.Server.Entities
{
    public class Folder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<DataType> DataTypes { get; set; } = new List<DataType>();
        public List<FolderAuthorizedUser> AuthorizedUsers { get; set; } = new List<FolderAuthorizedUser>();
    }

    public class FolderAuthorizedUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsFolderAdmin { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string FolderId { get; set; }
        public Folder Folder { get; set; }
    }

    public class DataType
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();

        public string FolderId { get; set; }
        public Folder Folder { get; set; }

        public List<Instance> Instances { get; set; }
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
        
        public string DataTypeId { get; set; }
        public DataType DataType { get; set; }
    }

    public class Instance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Data { get; set; }

        public string DataTypeId { get; set; }
        public DataType DataType { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public string CreatedById{ get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.UtcNow;
        public string UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
    }
}
