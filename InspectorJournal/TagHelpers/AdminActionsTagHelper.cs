using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace InspectorJournal.TagHelpers;

[HtmlTargetElement("admin-actions", Attributes = "item-id")]
public class AdminActionsTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor;

    public AdminActionsTagHelper(IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory,
        IActionContextAccessor actionContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _urlHelperFactory = urlHelperFactory;
        _actionContextAccessor = actionContextAccessor;
    }

    public int ItemId { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var actionContext = _actionContextAccessor.ActionContext;
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin"))
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            // Ссылка на редактирование с иконкой
            var editLink = new TagBuilder("a");
            editLink.Attributes["href"] = urlHelper.Action("Edit", new { id = ItemId });
            editLink.Attributes["class"] = "btn btn-outline-warning btn-sm"; // Классы для кнопки
            editLink.InnerHtml.AppendHtml("<i class='bi bi-pencil'></i>"); // Иконка редактирования

            // Ссылка на подробности с иконкой
            var detailsLink = new TagBuilder("a");
            detailsLink.Attributes["href"] = urlHelper.Action("Details", new { id = ItemId });
            detailsLink.Attributes["class"] = "btn btn-outline-info btn-sm"; // Классы для кнопки
            detailsLink.InnerHtml.AppendHtml("<i class='bi bi-eye'></i>"); // Иконка просмотра

            // Ссылка на удаление с иконкой
            var deleteLink = new TagBuilder("a");
            deleteLink.Attributes["href"] = urlHelper.Action("Delete", new { id = ItemId });
            deleteLink.Attributes["class"] = "btn btn-outline-danger btn-sm"; // Классы для кнопки
            deleteLink.InnerHtml.AppendHtml("<i class='bi bi-trash'></i>"); // Иконка удаления

            // Собираем содержимое
            output.TagName = "td"; // Ячейка таблицы
            output.Content.AppendHtml(editLink);
            output.Content.AppendHtml(" ");
            output.Content.AppendHtml(detailsLink);
            output.Content.AppendHtml(" ");
            output.Content.AppendHtml(deleteLink);
        }
        else
        {
            output.SuppressOutput(); // Если пользователь не администратор, не выводим содержимое
        }
    }
}
