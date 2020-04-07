using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class AppDbContextSeed
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                var defaultUser = new IdentityUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
                IdentityResult result = await userManager.CreateAsync(defaultUser, "123456Qq@");
                if (!result.Succeeded)
                    throw new DataException("Seed user failed");
            }

        }
    }
}
