using depot.Server.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<FolderAuthorizedUser> FolderAuthorizedUsers { get; set; } = new List<FolderAuthorizedUser>();
    }
}
