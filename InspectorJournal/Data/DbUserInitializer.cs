using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;

namespace InspectorJournal.Data;
//Инициализация базы данных первой учетной записью и двумя ролями admin и user
public static class DbUserInitializer
{
    public static async Task Initialize(HttpContext context)
    {
        UserManager<ApplicationUser> userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager<IdentityRole> roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        string adminEmail = "admin@gmail.com";
        string adminName = "admin@gmail.com";

        string password = "_Aa123456";
        if (await roleManager.FindByNameAsync("Admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (await roleManager.FindByNameAsync("User") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            ApplicationUser admin = new()
            {
                Email = adminEmail,
                UserName = adminName,
                RegistrationDate = DateTime.Now
            };
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}