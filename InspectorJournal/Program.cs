using InspectorJournal.Data;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.Middleware;
using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        // Внедрение зависимости для доступа к БД с использованием EF
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InspectionsDbContext>(options => options.UseSqlServer(connectionString));

        var connectionUsers = builder.Configuration.GetConnectionString("IdentityConnection");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionUsers));

        // Настройка Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        // Другие сервисы \\

        // Добавление кэширования
        services.AddMemoryCache();
        services.AddResponseCaching();
        services.AddControllers(options =>
        {
            options.CacheProfiles.Add("Default",
                new CacheProfile()
                {
                    Duration = 2 * 16 + 240
                });
        });
        services.AddDistributedMemoryCache();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".Journal.Session";
            options.IdleTimeout = TimeSpan.FromSeconds(3600);
            options.Cookie.IsEssential = true;
        });
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether
            //
            //
            // consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddHttpContextAccessor();
        services.AddControllersWithViews();
        services.AddRazorPages();

        var app = builder.Build();

        // Ожидаем, что в Development будем использовать страницу ошибок
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler("/Home/Error");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        // Включаем сессии
        app.UseSession();

        // Важно: вызов app.UseDbInitializer должен быть после app.UseRouting(), но до app.MapControllerRoute()
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        // Регистрация middleware для инициализации базы данных
        app.UseDbInitializer(); // Это вызывает вашу инициализацию базы данных

        // Маршруты
        // Перенаправление на страницу входа, если пользователь не аутентифицирован
        app.MapGet("/", async context =>
        {
            if (!context.User.Identity.IsAuthenticated)
                context.Response.Redirect("/Identity/Account/Login"); // Путь на страницу входа
            else
                context.Response.Redirect("/Home/Index"); // Путь на Index если пользователь аутентифицирован
        });

        // Настройка стандартного маршрута
        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}