using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.Infrastructure;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessionExtensions = InspectorJournal.Infrastructure.SessionExtensions;

namespace InspectorJournal.Controllers;

[Authorize]
[ResponseCache(CacheProfileName = "Default")]
public class OffendingEnterprisesController : Controller
{
    private readonly InspectionsDbContext _context;
    private readonly int pageSize = 10; // количество элементов на странице

    public OffendingEnterprisesController(InspectionsDbContext context, IConfiguration appConfig = null)
    {
        _context = context;
        if (appConfig != null) pageSize = int.Parse(appConfig["Parameters:PageSize"]);
    }

    // GET: OffendingEnterprises
    [SetToSession("OffendingEnterprise")]
    public IActionResult Index(string? enterpriseName, string? violationType, string? correctionStatus, string? paymentStatus, SortState sortOrder = SortState.No, int page = 1)
    {
        // Если фильтры не указаны, восстанавливаем из сессии
        if (string.IsNullOrEmpty(violationType) && string.IsNullOrEmpty(correctionStatus) && string.IsNullOrEmpty(paymentStatus) && string.IsNullOrEmpty(enterpriseName) && HttpContext != null)
        {
            var sessionData = SessionExtensions.Get(HttpContext.Session, "OffendingEnterprise");
            if (sessionData != null)
            {
                var filters = Transformations.DictionaryToObject<Dictionary<string, string>>(sessionData);
                violationType = filters.ContainsKey("ViolationType") ? filters["ViolationType"] : null;
                correctionStatus = filters.ContainsKey("CorrectionStatus") ? filters["CorrectionStatus"] : null;
                paymentStatus = filters.ContainsKey("PaymentStatus") ? filters["PaymentStatus"] : null;
                enterpriseName = filters.ContainsKey("EnterpriseName") ? filters["EnterpriseName"] : null;
            }
        }

        // Получение данных о проверках
        IQueryable<Inspection> inspections = _context.Inspections
            .Include(i => i.Enterprise)
            .Include(i => i.ViolationType);

        inspections = ApplyFiltersAndSorting(inspections, sortOrder, violationType, correctionStatus, paymentStatus, enterpriseName);

        // Разбиение на страницы
        var count = inspections.Count();
        inspections = inspections.Skip((page - 1) * pageSize).Take(pageSize);

        // Подготовка данных для представления
        var viewModel = new OffendingEnterprisesViewModel
        {
            Inspections = inspections.ToList(),
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder),
            ViolationType = violationType ?? "",
            CorrectionStatus = correctionStatus ?? "",
            PaymentStatus = paymentStatus ?? "",
            Name = enterpriseName ?? ""
        };

        return View("~/Views/OffendingEnterprises/Index.cshtml", viewModel);
    }


    private static IQueryable<Inspection> ApplyFiltersAndSorting(IQueryable<Inspection> inspections, SortState sortOrder, string? violationType, string? correctionStatus, string? paymentStatus, string? enterpriseName)
    {
        // Фильтрация
        inspections = inspections.Where(i =>
            (string.IsNullOrEmpty(violationType) || i.ViolationType.Name.Contains(violationType)) &&
            (string.IsNullOrEmpty(correctionStatus) || i.CorrectionStatus == correctionStatus) &&
            (string.IsNullOrEmpty(paymentStatus) || i.PaymentStatus == paymentStatus) &&  // Фильтрация по статусу оплаты
            (string.IsNullOrEmpty(enterpriseName) || i.Enterprise.Name.Contains(enterpriseName)));

        // Применение сортировки
        inspections = sortOrder switch
        {
            SortState.EnterpriseNameAsc => inspections.OrderBy(i => i.Enterprise.Name),
            SortState.EnterpriseNameDesc => inspections.OrderByDescending(i => i.Enterprise.Name),
            SortState.ViolationTypeAsc => inspections.OrderBy(i => i.ViolationType.Name),
            SortState.ViolationTypeDesc => inspections.OrderByDescending(i => i.ViolationType.Name),
            _ => inspections
        };

        return inspections;
    }

}
