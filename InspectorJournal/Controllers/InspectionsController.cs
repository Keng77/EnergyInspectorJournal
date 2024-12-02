using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.Infrastructure;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SessionExtensions = InspectorJournal.Infrastructure.SessionExtensions;

namespace InspectorJournal.Controllers;

[Authorize]
[ResponseCache(CacheProfileName = "Default")]
public class InspectionsController : Controller
{
    private readonly InspectionsDbContext _context;
    private readonly int pageSize = 10; // количество элементов на странице

    public InspectionsController(InspectionsDbContext context, IConfiguration appConfig = null)
    {
        _context = context;
        if (appConfig != null) pageSize = int.Parse(appConfig["Parameters:PageSize"]);
    }

    // GET: Operations
    [SetToSession("Inspection")] //Фильтр действий для сохранение в сессию параметров отбора
    public IActionResult Index(FilterInspectionViewModel inspection, SortState sortOrder = SortState.No, int page = 1)
    {
        if ((inspection.Enterprise == null) & (inspection.ViolationType == null) & (inspection.PenaltyAmount == null))
            // Считывание данных из сессии
            if (HttpContext != null)
            {
                var sessionInspection = SessionExtensions.Get(HttpContext.Session, "Inspection");
                if (sessionInspection != null)
                    inspection = Transformations.DictionaryToObject<FilterInspectionViewModel>(sessionInspection);
            }

        // Сортировка и фильтрация данных
        IQueryable<Inspection> inspectionContext = _context.Inspections;
        inspectionContext = Sort_Search(inspectionContext, sortOrder, inspection.Enterprise ?? "",
            inspection.ViolationType ?? "", inspection.PenaltyAmount > 0 ? inspection.PenaltyAmount : 0);

        // Разбиение на страницы
        var count = inspectionContext.Count();
        inspectionContext = inspectionContext.Skip((page - 1) * pageSize).Take(pageSize);

        // Формирование модели для передачи представлению
        InspectionsViewModel inspections = new()
        {
            Inspections = inspectionContext,
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder),
            FilterInspectionViewModel = inspection
        };
        return View(inspections);
    }

    // GET: Inspections/Create
    public async Task<IActionResult> Create()
    {
        // Загружаем все объекты, которые нужны для выборок
        var enterprises = await _context.Enterprises.ToListAsync();
        var violationTypes = await _context.ViolationTypes.ToListAsync();
        var inspectors = await _context.Inspectors.ToListAsync();

        // Создаем новую модель проверки 
        var inspection = new Inspection();

        // Передаем в представление все необходимые данные
        ViewBag.EnterpriseId = new SelectList(enterprises, "EnterpriseId", "Name");
        ViewBag.ViolationTypeId = new SelectList(violationTypes, "ViolationTypeId", "Name");
        ViewBag.InspectorId = new SelectList(inspectors, "InspectorId", "FullName");

        return View(inspection);
    }


    // POST: Inspections/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        // Загружаем связанные объекты вручную, чтобы проверить их существование
        var enterprise = await _context.Enterprises.FindAsync(inspection.EnterpriseId);
        var inspector = await _context.Inspectors.FindAsync(inspection.InspectorId);
        var violationType = await _context.ViolationTypes.FindAsync(inspection.ViolationTypeId);

        // Проверка на существование связанных сущностей
        if (enterprise == null || inspector == null || violationType == null)
        {
            // Если сущности не найдены, добавляем ошибки в ModelState
            if (enterprise == null)
                ModelState.AddModelError(nameof(inspection.EnterpriseId), "Предприятие не найдено.");
            if (inspector == null)
                ModelState.AddModelError(nameof(inspection.InspectorId), "Инспектор не найден.");
            if (violationType == null)
                ModelState.AddModelError(nameof(inspection.ViolationTypeId), "Тип нарушения не найден.");

            // Заполняем ViewBag для повторного отображения данных
            ViewData["EnterpriseId"] =
                new SelectList(_context.Enterprises, "EnterpriseId", "Name", inspection.EnterpriseId);
            ViewData["ViolationTypeId"] = new SelectList(_context.ViolationTypes, "ViolationTypeId", "Name",
                inspection.ViolationTypeId);
            ViewData["InspectorId"] =
                new SelectList(_context.Inspectors, "InspectorId", "FullName", inspection.InspectorId);

            return View(inspection); // Возвращаем модель с ошибками
        }

        // Если все проверки прошли успешно, сохраняем данные
        inspection.Enterprise = enterprise;
        inspection.Inspector = inspector;
        inspection.ViolationType = violationType;
        _context.Add(inspection);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Inspections/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections.FindAsync(id);
        if (inspection == null) return NotFound();

        // Асинхронно загружаем данные
        var enterprises = await _context.Enterprises.ToListAsync();
        var violationTypes = await _context.ViolationTypes.ToListAsync();
        var inspectors = await _context.Inspectors.ToListAsync();

        // Проверяем, что данные не пусты, перед тем как создавать SelectList
        if (enterprises == null || !enterprises.Any())
            // Можно добавить сообщение об ошибке или обработку
            ModelState.AddModelError(string.Empty, "No enterprises found.");
        if (violationTypes == null || !violationTypes.Any())
            // Можно добавить сообщение об ошибке или обработку
            ModelState.AddModelError(string.Empty, "No violation types found.");
        if (inspectors == null || !inspectors.Any())
            // Можно добавить сообщение об ошибке или обработку
            ModelState.AddModelError(string.Empty, "No inspectors found.");

        // Передаем данные в ViewData
        ViewData["EnterpriseId"] = new SelectList(enterprises, "EnterpriseId", "Name");
        ViewData["ViolationTypeId"] = new SelectList(violationTypes, "ViolationTypeId", "Name");
        ViewData["InspectorId"] = new SelectList(inspectors, "InspectorId", "FullName");

        return View(inspection);
    }


    // POST: Inspections/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inspection inspection)
    {
        if (id != inspection.InspectionId) return NotFound();

        // Загружаем связанные объекты вручную, чтобы проверить их существование
        var enterprise = await _context.Enterprises.FindAsync(inspection.EnterpriseId);
        var inspector = await _context.Inspectors.FindAsync(inspection.InspectorId);
        var violationType = await _context.ViolationTypes.FindAsync(inspection.ViolationTypeId);

        // Проверка на существование связанных сущностей
        if (enterprise == null || inspector == null || violationType == null)
        {
            // Если сущности не найдены, добавляем ошибки в ModelState
            if (enterprise == null)
                ModelState.AddModelError(nameof(inspection.EnterpriseId), "Предприятие не найдено.");
            if (inspector == null)
                ModelState.AddModelError(nameof(inspection.InspectorId), "Инспектор не найден.");
            if (violationType == null)
                ModelState.AddModelError(nameof(inspection.ViolationTypeId), "Тип нарушения не найден.");

            // Заполняем ViewBag для повторного отображения данных
            ViewData["EnterpriseId"] =
                new SelectList(_context.Enterprises, "EnterpriseId", "Name", inspection.EnterpriseId);
            ViewData["ViolationTypeId"] = new SelectList(_context.ViolationTypes, "ViolationTypeId", "Name",
                inspection.ViolationTypeId);
            ViewData["InspectorId"] =
                new SelectList(_context.Inspectors, "InspectorId", "FullName", inspection.InspectorId);

            return View(inspection); // Возвращаем модель с ошибками
        }

        // Если все проверки прошли успешно, обновляем данные
        inspection.Enterprise = enterprise;
        inspection.Inspector = inspector;
        inspection.ViolationType = violationType;


        try
        {
            // Обновляем запись в базе данных
            _context.Update(inspection);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Inspections.Any(e => e.InspectionId == inspection.InspectionId))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }


    // GET: Inspections/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections
            .Include(o => o.Enterprise)
            .Include(o => o.ViolationType)
            .Include(o => o.Inspector)
            .SingleOrDefaultAsync(m => m.InspectionId == id);
        if (inspection == null) return NotFound();

        return View(inspection);
    }

    // GET: Inspections/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections
            .Include(o => o.Enterprise)
            .Include(o => o.ViolationType)
            .Include(o => o.Inspector)
            .SingleOrDefaultAsync(m => m.InspectionId == id);
        if (inspection == null) return NotFound();

        return View(inspection);
    }

    // POST: Inspections/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var inspection = await _context.Inspections.SingleOrDefaultAsync(m => m.InspectionId == id);
        _context.Inspections.Remove(inspection);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OperationExists(int id)
    {
        return _context.Inspections.Any(e => e.InspectionId == id);
    }

    private static IQueryable<Inspection> Sort_Search(IQueryable<Inspection> inspections, SortState sortOrder,
        string searchEnterpriseName, string searchViolationType, decimal searchPenaltyAmount)
    {
        // Применяем сортировку
        switch (sortOrder)
        {
            case SortState.EnterpriseNameAsc:
                inspections = inspections.OrderBy(s => s.Enterprise.Name);
                break;
            case SortState.EnterpriseNameDesc:
                inspections = inspections.OrderByDescending(s => s.Enterprise.Name);
                break;
            case SortState.InspectorNameAsc:
                inspections = inspections.OrderBy(s => s.Inspector.FullName);
                break;
            case SortState.InspectorNameDesc:
                inspections = inspections.OrderByDescending(s => s.Inspector.FullName);
                break;
            case SortState.ViolationTypeAsc:
                inspections = inspections.OrderBy(s => s.ViolationType.Name);
                break;
            case SortState.ViolationTypeDesc:
                inspections = inspections.OrderByDescending(s => s.ViolationType.Name);
                break;
            case SortState.PenaltyAmountAsc:
                inspections = inspections.OrderBy(s => s.PenaltyAmount);
                break;
            case SortState.PenaltyAmountDesc:
                inspections = inspections.OrderByDescending(s => s.PenaltyAmount);
                break;
        }

        // Применяем фильтры
        inspections = inspections.Include(o => o.Enterprise)
            .Include(o => o.ViolationType)
            .Include(o => o.Inspector)
            .Where(o =>
                (string.IsNullOrEmpty(searchEnterpriseName) || o.Enterprise.Name.Contains(searchEnterpriseName)) &&
                (string.IsNullOrEmpty(searchViolationType) || o.ViolationType.Name.Contains(searchViolationType)) &&
                (searchPenaltyAmount <= 0 || o.PenaltyAmount >= searchPenaltyAmount));

        return inspections;
    }
}