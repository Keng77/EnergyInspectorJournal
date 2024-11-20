using InspectorJournal.Data;
using InspectorJournal.Middleware;
using InspectorJournal.Models;
using InspectorJournal.DataLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        // Внедрение зависимости для доступа к БД с использованием EF
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InspectionsDbContext>(options => options.UseSqlServer(connectionString));

        string connectionUsers = builder.Configuration.GetConnectionString("IdentityConnection");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionUsers));

        // Настройка Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

        // Другие сервисы \\

        services.AddDistributedMemoryCache();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".Journal.Session";
            options.IdleTimeout = TimeSpan.FromSeconds(3600);
            options.Cookie.IsEssential = true;
        });
        services.AddHttpContextAccessor();
        services.AddControllersWithViews();
        services.AddRazorPages();

        var app = builder.Build();

        // Ожидаем, что в Development будем использовать страницу ошибок
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        // Включаем сессии
        app.UseSession();

        // Важно: вызов app.UseDbInitializer должен быть после app.UseRouting(), но до app.MapControllerRoute()
        app.UseRouting();

        // Регистрация middleware для инициализации базы данных
        app.UseDbInitializer();  // Это вызывает вашу инициализацию базы данных

        // Маршруты
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}
