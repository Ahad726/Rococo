using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rococo.DataAccess.Data;
using Rococo.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Rococo.DataSeed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;
        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.applicationDbContext = applicationDbContext;
        }
        public async Task InitializeAsync()
        {
            // Add pending migrations if exists
            if (applicationDbContext.Database.GetPendingMigrations().Count() > 0)
            {
                applicationDbContext.Database.Migrate();
            }

            if (applicationDbContext.Database.GetAppliedMigrations().Count() > 0)
            {
                applicationDbContext.Database.Migrate();
            }

            // Return if Admin role exists
            if (applicationDbContext.Roles.Any(x => x.Name == "Admin"))
            {
               // return;
            }

            // Create Admin and manager Role
            roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();

            //Create user 
            userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            }, "Abc@123").GetAwaiter().GetResult();

            // Assign role to admin user
            await userManager.AddToRoleAsync(await userManager.FindByEmailAsync("admin@gmail.com"), "Admin");
        }


    }
}
