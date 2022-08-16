using Microsoft.AspNetCore.Identity;
using MvcMRDB.Areas.Identity.Data;

namespace MvcMRDB.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            string roleName = "Admin";
            
            IdentityResult roleResult;
            
            var roleExist = await RoleManager.RoleExistsAsync(roleName);
            
            // ensure that the role does not exist
            if (!roleExist)
            {
                //create the roles and seed them to the database: 
                roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
            }

            // find the user with the admin email 
            var _user = await UserManager.FindByEmailAsync("yogeshmail@email.com");

            // check if the user exists
            if (_user != null)
            {
                await UserManager.AddToRoleAsync(_user, "Admin");
            }
        }
    }
}
