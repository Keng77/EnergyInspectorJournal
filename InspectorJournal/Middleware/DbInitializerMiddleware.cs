using InspectorJournal.DataLayer.Data;
using InspectorJournal.Data;
using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;

namespace InspectorJournal.Middleware;

public class DbInitializerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public Task Invoke(HttpContext context)
    {
        if (!(context.Session.Keys.Contains("starting")))
        {
            DbUserInitializer.Initialize(context).Wait();
            DbInitializer.Initialize(context.RequestServices.GetRequiredService<InspectionsDbContext>());
            context.Session.SetString("starting", "Yes");
        }

        // Вызов следующего делегата / компонента middleware в конвейере
        return _next.Invoke(context);
    }
}
