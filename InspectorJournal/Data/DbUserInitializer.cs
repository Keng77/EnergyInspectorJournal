using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;

namespace InspectorJournal.Data;

//Инициализация базы данных первой учетной записью и двумя ролями admin и user
public static class DbUserInitializer
{
    public static async Task Initialize(HttpContext context)
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        var adminEmail = "admin@gmail.com";
        var adminName = "admin@gmail.com";

        var password = "_Aa123456";
        if (await roleManager.FindByNameAsync("Admin") == null)
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (await roleManager.FindByNameAsync("User") == null) await roleManager.CreateAsync(new IdentityRole("User"));
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            ApplicationUser admin = new()
            {
                Email = adminEmail,
                UserName = adminName,
                RegistrationDate = DateTime.Now
            };
            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}