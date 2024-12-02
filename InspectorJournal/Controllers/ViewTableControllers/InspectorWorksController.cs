using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.Infrastructure;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessionExtensions = InspectorJournal.Infrastructure.SessionExtensions;

[Authorize]
[ResponseCache(CacheProfileName = "Default")]
public class InspectorWorksController : Controller
{
    private readonly InspectionsDbContext _context;
    private readonly int pageSize = 10; // количество элементов на странице

    public InspectorWorksController(InspectionsDbContext context, IConfiguration appConfig = null)
    {
        _context = context;
        if (appConfig != null) pageSize = int.Parse(appConfig["Parameters:PageSize"]);
    }

    [SetToSession("InspectorWorks")]
    public IActionResult Index(FilterInspectorWorksViewModel filters, SortState sortOrder = SortState.No, int page = 1)
    {
        // Восстановление фильтров из сессии, если они не переданы
        if (filters.IsEmpty())
        {
            if (HttpContext != null)
            {
                var sessionFilters = SessionExtensions.Get(HttpContext.Session, "InspectorWorks");
                if (sessionFilters != null)
                {
                    filters = Transformations.DictionaryToObject<FilterInspectorWorksViewModel>(sessionFilters);
                }
            }
        }

        // Получение данных о проверках и применение фильтров
        IQueryable<Inspection> inspections = _context.Inspections
            .Include(i => i.Enterprise)
            .Include(i => i.ViolationType)
            .Include(i => i.Inspector);

       
        // Группируем данные по инспектору после фильтрации и сортировки
        var groupedInspections = inspections
            .GroupBy(i => i.Inspector)
            .Select(g => new InspectorWorksItemViewModel()
            {
                Inspector = g.Key,
                Inspections = g.ToList(),
                NumberOfInspections = g.Count(),
                TotalPenaltyAmount = g.Sum(i => i.PenaltyAmount),
                MaxPenaltyAmount = g.Max(i => i.PenaltyAmount)
            })
            .AsQueryable();


        // Применяем фильтры и сортировку к сгруппированным данным
        groupedInspections = ApplyGroupedFilters(groupedInspections, filters);
        groupedInspections = ApplyGroupedSorting(groupedInspections, sortOrder);

        // Подсчёт общего количества записей после фильтров
        var count = groupedInspections.Count();


        // Применяем пагинацию
        var pagedGroupInspections = groupedInspections
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Формирование модели для представления
        var viewModel = new InspectorWorksViewModel
        {
            Inspections = pagedGroupInspections.Select(g => new InspectorWorksItemViewModel
            {
                Inspector = g.Inspector,
                NumberOfInspections = g.NumberOfInspections,
                TotalPenaltyAmount = g.TotalPenaltyAmount,
                MaxPenaltyAmount = g.MaxPenaltyAmount
            }).ToList(),

            Filters = filters,
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder)
        };

        return View("~/Views/InspectorWorks/Index.cshtml", viewModel);
    }




    [SetToSession("InspectorDetails")]
    public IActionResult InspectorDetails(int inspectorId, FilterInspectorDetailsViewModel filters, SortState sortOrder = SortState.No, int page = 1)
    {
        // Восстановление фильтров из сессии, если они не переданы
        if (filters.IsEmpty())
        {
            if (HttpContext != null)
            {
                var sessionFilters = SessionExtensions.Get(HttpContext.Session, "InspectorDetails");
                if (sessionFilters != null)
                {
                    filters = Transformations.DictionaryToObject<FilterInspectorDetailsViewModel>(sessionFilters);
                }
            }
        }
        // Получение имени инспектора
        var inspectorName = _context.Inspectors
            .Where(i => i.InspectorId == inspectorId)
            .Select(i => i.FullName) // Предполагаем, что у инспектора есть поле FullName
            .FirstOrDefault();

        if (string.IsNullOrEmpty(inspectorName))
        {
            return NotFound("Инспектор не найден.");
        }

        // Получение данных о проверках
        IQueryable<Inspection> inspections = _context.Inspections
            .Where(i => i.InspectorId == inspectorId)  // Фильтруем по ID инспектора
            .Include(i => i.Enterprise)
            .Include(i => i.ViolationType)
            .Include(i => i.Inspector);

        inspections = ApplySorting(inspections, sortOrder);
        inspections = ApplyInspectorDetailsFilters(inspections, filters);
        
        // Подсчёт общего количества записей для пагинации
        var count = inspections.Count();

        // Применение пагинации
        var pagedInspections = inspections.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Формирование модели для представления
        var viewModel = new InspectorDetailsViewModel
        {
            Inspections = pagedInspections,
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder),
            Filters = filters,
            InspectorId = inspectorId,  // Передаем ID инспектора
            InspectorName = inspectorName
        };

        return View("~/Views/InspectorWorks/InspectorDetails.cshtml", viewModel);
    }



    [SetToSession("DepartmentDetails")]
    public IActionResult DepartmentDetails(string department, FilterDepartmentDetailsViewModel filters, SortState sortOrder = SortState.No, int page = 1)
    {
        // Восстановление фильтров из сессии, если они не переданы
        if (filters.IsEmpty())
        {
            if (HttpContext != null)
            {
                var sessionFilters = SessionExtensions.Get(HttpContext.Session, "DepartmentDetails");
                if (sessionFilters != null)
                {
                    filters = Transformations.DictionaryToObject<FilterDepartmentDetailsViewModel>(sessionFilters);
                }
            }
        }

        // Получение данных о проверках
        IQueryable<Inspection> inspections = _context.Inspections
            .Where(i => i.Inspector.Department == department)  // Фильтруем по департаменту
            .Include(i => i.Enterprise)
            .Include(i => i.ViolationType)
            .Include(i => i.Inspector);

        // Применяем фильтры и сортировку
        inspections = ApplySorting(inspections, sortOrder);
        inspections = ApplyDepartmentDetailsFilters(inspections, filters);
        

        // Подсчёт общего количества записей для пагинации
        var count = inspections.Count();

        // Применение пагинации
        var pagedInspections = inspections
            .Skip((page - 1) * pageSize)  // Пропускаем записи с учётом текущей страницы
            .Take(pageSize)              // Берём записи для текущей страницы
            .ToList();

        // Формирование модели для представления
        var viewModel = new DepartmentDetailsViewModel
        {
            Inspections = pagedInspections,
            PageViewModel = new PageViewModel(count, page, pageSize), // Информация о пагинации
            SortViewModel = new SortViewModel(sortOrder), // Информация о сортировке
            Filters = filters, // Фильтры для отображения
            Department = department // Департамент для отображения
        };

        return View("~/Views/InspectorWorks/DepartmentDetails.cshtml", viewModel);
    }




    private IQueryable<Inspection> ApplyInspectorWorksFilters(IQueryable<Inspection> inspections, FilterInspectorWorksViewModel filters)
    {
        if (!string.IsNullOrEmpty(filters.InspectorName))
        {
            inspections = inspections.Where(i => i.Inspector.FullName.Contains(filters.InspectorName));
        }
        if (!string.IsNullOrEmpty(filters.Department))
        {
            inspections = inspections.Where(i => i.Inspector.Department.Contains(filters.Department));
        }
        return inspections;
    }


    private IQueryable<Inspection> ApplyInspectorDetailsFilters(IQueryable<Inspection> inspections, FilterInspectorDetailsViewModel filters)
    {
        if (!string.IsNullOrEmpty(filters.EnterpriseName))
        {
            inspections = inspections.Where(i => i.Enterprise.Name.Contains(filters.EnterpriseName));
        }
        if (!string.IsNullOrEmpty(filters.ViolationType))
        {
            inspections = inspections.Where(i => i.ViolationType.Name.Contains(filters.ViolationType));
        }
        if (!string.IsNullOrEmpty(filters.PaymentStatus))
        {
            inspections = inspections.Where(i => i.PaymentStatus == filters.PaymentStatus);
        }
        if (!string.IsNullOrEmpty(filters.CorrectionStatus))
        {
            inspections = inspections.Where(i => i.CorrectionStatus == filters.CorrectionStatus);
        }
        return inspections;
    }


    private IQueryable<Inspection> ApplyDepartmentDetailsFilters(IQueryable<Inspection> inspections, FilterDepartmentDetailsViewModel filters)
    {
        if (!string.IsNullOrEmpty(filters.InspectorName))
        {
            inspections = inspections.Where(i => i.Inspector.FullName.Contains(filters.InspectorName));
        }
        if (!string.IsNullOrEmpty(filters.EnterpriseName))
        {
            inspections = inspections.Where(i => i.Enterprise.Name.Contains(filters.EnterpriseName));
        }
        if (!string.IsNullOrEmpty(filters.ViolationType))
        {
            inspections = inspections.Where(i => i.ViolationType.Name.Contains(filters.ViolationType));
        }
        if (!string.IsNullOrEmpty(filters.PaymentStatus))
        {
            inspections = inspections.Where(i => i.PaymentStatus == filters.PaymentStatus);
        }
        if (!string.IsNullOrEmpty(filters.CorrectionStatus))
        {
            inspections = inspections.Where(i => i.CorrectionStatus == filters.CorrectionStatus);
        }
        return inspections;
    }


    // Приватный метод для применения сортировки
    private IQueryable<Inspection> ApplySorting(IQueryable<Inspection> inspections, SortState sortOrder)
    {
        switch (sortOrder)
        {
            case SortState.InspectorNameAsc:
                return inspections.OrderBy(i => i.Inspector.FullName);
            case SortState.InspectorNameDesc:
                return inspections.OrderByDescending(i => i.Inspector.FullName);

            case SortState.EnterpriseNameAsc:
                return inspections.OrderBy(i => i.Enterprise.Name);
            case SortState.EnterpriseNameDesc:
                return inspections.OrderByDescending(i => i.Enterprise.Name);

            case SortState.TotalPenaltyAmountAsc:
                return inspections.OrderBy(i => i.PenaltyAmount);
            case SortState.TotalPenaltyAmountDesc:
                return inspections.OrderByDescending(i => i.PenaltyAmount);

            case SortState.MaxPenaltyAmountAsc:
                return inspections.OrderBy(i => i.PenaltyAmount);
            case SortState.MaxPenaltyAmountDesc:
                return inspections.OrderByDescending(i => i.PenaltyAmount);

            case SortState.DepartmentAsc:
                return inspections.OrderBy(i => i.Inspector.Department);
            case SortState.DepartmentDesc:
                return inspections.OrderByDescending(i => i.Inspector.Department);

            case SortState.NumberOfInspectionsAsc:
                return inspections.OrderBy(i => i.Inspector.FullName);  
            case SortState.NumberOfInspectionsDesc:
                return inspections.OrderByDescending(i => i.Inspector.FullName); 

            default:
                return inspections.OrderBy(i => i.InspectorId);
        }
    }

    private IQueryable<InspectorWorksItemViewModel> ApplyGroupedSorting(
        IQueryable<InspectorWorksItemViewModel> groupedInspections, SortState sortOrder)
    {
        return sortOrder switch
        {
            SortState.InspectorNameAsc => groupedInspections.OrderBy(g => g.Inspector.FullName),
            SortState.InspectorNameDesc => groupedInspections.OrderByDescending(g => g.Inspector.FullName),
            SortState.DepartmentAsc => groupedInspections.OrderBy(g => g.Inspector.Department),
            SortState.DepartmentDesc => groupedInspections.OrderByDescending(g => g.Inspector.Department),
            SortState.NumberOfInspectionsAsc => groupedInspections.OrderBy(g => g.NumberOfInspections),
            SortState.NumberOfInspectionsDesc => groupedInspections.OrderByDescending(g => g.NumberOfInspections),
            SortState.TotalPenaltyAmountAsc => groupedInspections.OrderBy(g => g.TotalPenaltyAmount),
            SortState.TotalPenaltyAmountDesc => groupedInspections.OrderByDescending(g => g.TotalPenaltyAmount),
            SortState.MaxPenaltyAmountAsc => groupedInspections.OrderBy(g => g.MaxPenaltyAmount),
            SortState.MaxPenaltyAmountDesc => groupedInspections.OrderByDescending(g => g.MaxPenaltyAmount),
            _ => groupedInspections // Default order
        };
    }

    private IQueryable<InspectorWorksItemViewModel> ApplyGroupedFilters(
        IQueryable<InspectorWorksItemViewModel> groupedInspections, FilterInspectorWorksViewModel filters)
    {
        if (!string.IsNullOrEmpty(filters.InspectorName))
        {
            groupedInspections = groupedInspections
                .Where(g => g.Inspector.FullName.Contains(filters.InspectorName));
        }
        if (!string.IsNullOrEmpty(filters.Department))
        {
            groupedInspections = groupedInspections
                .Where(g => g.Inspector.Department.Contains(filters.Department));
        }
        return groupedInspections;
    }





}
