using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using depot.Server.Data;
using depot.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace depot.Server.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly MongoDBContext _mongoDBContext;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            ApplicationDbContext applicationDbContext,
            MongoDBContext mongoDBContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _mongoDBContext = mongoDBContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            //Cleanup
            var folderAuthorizedUsers = _applicationDbContext.FolderAuthorizedUsers.Where(u => u.ApplicationUserId == user.Id);
            foreach(var folderAuthorizedUser in folderAuthorizedUsers)
            {
                var count = _applicationDbContext.FolderAuthorizedUsers.Count(f => f.FolderId == folderAuthorizedUser.FolderId);
                if(count == 1)
                {
                    var query = new BsonDocument("$and",
                    new BsonArray
                    {
                        new BsonDocument("FolderId", folderAuthorizedUser.FolderId)
                    });

                    await _mongoDBContext.Instances.DeleteManyAsync(query);

                    var folder = await _applicationDbContext.Folders.FirstOrDefaultAsync(f => f.Id == folderAuthorizedUser.FolderId);
                    _applicationDbContext.Folders.Remove(folder);
                }
                else
                {
                    _applicationDbContext.FolderAuthorizedUsers.Remove(folderAuthorizedUser);
                }
            }
            await _applicationDbContext.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
