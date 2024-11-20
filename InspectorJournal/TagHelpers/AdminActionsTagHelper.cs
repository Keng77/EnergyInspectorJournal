using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace InspectorJournal.TagHelpers
{
    [HtmlTargetElement("admin-actions", Attributes = "item-id")]
    public class AdminActionsTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public AdminActionsTagHelper(IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        // ID элемента (например, InspectionId, EnterpriseId и т.д.)
        public int ItemId { get; set; }

        // Префикс, который используется для атрибута asp-route-id
        public string IdName { get; set; } = "id";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = _httpContextAccessor.HttpContext.User;

            // Если пользователь аутентифицирован и имеет нужные права
            if (!user.Identity.IsAuthenticated /*&& user.IsInRole("Admin")*/)
            {
                // Используем IUrlHelper, извлекаем ActionContext из HttpContext
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                // Используем TagBuilder для создания элементов <a>
                var editLink = new TagBuilder("a");
                editLink.Attributes["href"] = urlHelper.Action("Edit", new { id = ItemId });
                editLink.InnerHtml.Append("Редактировать");

                var detailsLink = new TagBuilder("a");
                detailsLink.Attributes["href"] = urlHelper.Action("Details", new { id = ItemId });
                detailsLink.InnerHtml.Append("Подробно");

                var deleteLink = new TagBuilder("a");
                deleteLink.Attributes["href"] = urlHelper.Action("Delete", new { id = ItemId });
                deleteLink.InnerHtml.Append("Удалить");

                // Оборачиваем все в <td> (или другую нужную разметку)
                output.TagName = "td";
                output.Content.AppendHtml(editLink);
                output.Content.AppendHtml(" | ");
                output.Content.AppendHtml(detailsLink);
                output.Content.AppendHtml(" | ");
                output.Content.AppendHtml(deleteLink);
            }
            else
            {
                output.SuppressOutput(); // Скрываем весь <admin-actions> если нет нужных прав
            }
        }
    }
}
