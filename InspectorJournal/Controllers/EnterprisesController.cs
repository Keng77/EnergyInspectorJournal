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
public class EnterprisesController : Controller
{
    private readonly InspectionsDbContext _context;
    private readonly int pageSize = 10; // количество элементов на странице

    public EnterprisesController(InspectionsDbContext context, IConfiguration appConfig = null)
    {
        _context = context;
        if (appConfig != null) pageSize = int.Parse(appConfig["Parameters:PageSize"]);
    }

    // GET: Enterprises
    [SetToSession("Enterprise")] // Фильтр действий для сохранения в сессию параметров отбора
    public IActionResult Index(string EnterpriseName = "", string EnterpriseAddress = "",
        SortState sortOrder = SortState.No, int page = 1)
    {
        // Если параметры фильтрации пустые, считываем их из сессии
        if (string.IsNullOrEmpty(EnterpriseName) && string.IsNullOrEmpty(EnterpriseAddress))
        {
            var sessionData = SessionExtensions.Get(HttpContext.Session, "Enterprise");
            if (sessionData != null)
            {
                var enterpriseViewModel = Transformations.DictionaryToObject<EnterprisesViewModel>(sessionData);
                EnterpriseName = enterpriseViewModel?.Name ?? "";
                EnterpriseAddress = enterpriseViewModel?.Address ?? "";
            }
        }

        // Сортировка и фильтрация данных
        IQueryable<Enterprise> enterpriseContext = _context.Enterprises;
        enterpriseContext = Sort_Search(enterpriseContext, sortOrder, EnterpriseName, EnterpriseAddress);

        // Разбиение на страницы
        var count = enterpriseContext.Count();
        var enterprises = enterpriseContext.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Формирование модели для передачи представлению
        EnterprisesViewModel viewModel = new()
        {
            Enterprises = enterprises,
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder),
            Name = EnterpriseName,
            Address = EnterpriseAddress
        };

        return View(viewModel);
    }


    // GET: Enterprises/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(m => m.EnterpriseId == id);
        if (enterprise == null) return NotFound();

        return View(enterprise);
    }

    // GET: Enterprises/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Enterprises/Create        
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EnterpriseId,Name,OwnershipType,Address,DirectorName,DirectorPhone")] Enterprise enterprise)
    {
        if (ModelState.IsValid)
        {
            _context.Add(enterprise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(enterprise);
    }

    // GET: Enterprises/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var enterprise = await _context.Enterprises.FindAsync(id);
        if (enterprise == null) return NotFound();
        return View(enterprise);
    }

    // POST: Enterprises/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("EnterpriseId,Name,OwnershipType,Address,DirectorName,DirectorPhone")] Enterprise enterprise)
    {
        if (id != enterprise.EnterpriseId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(enterprise);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(enterprise.EnterpriseId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(enterprise);
    }

    // GET: Enterprises/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(m => m.EnterpriseId == id);
        if (enterprise == null) return NotFound();

        return View(enterprise);
    }

    // POST: Enterprises/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enterprise = await _context.Enterprises.FindAsync(id);
        if (enterprise != null) _context.Enterprises.Remove(enterprise);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EnterpriseExists(int id)
    {
        return _context.Enterprises.Any(e => e.EnterpriseId == id);
    }

    private static IQueryable<Enterprise> Sort_Search(IQueryable<Enterprise> enterprises, SortState sortOrder,
        string searchEnterpriseName, string searchEnterpriseAddress)
    {
        // Применяем сортировку
        switch (sortOrder)
        {
            case SortState.EnterpriseNameAsc:
                enterprises = enterprises.OrderBy(s => s.Name);
                break;
            case SortState.EnterpriseNameDesc:
                enterprises = enterprises.OrderByDescending(s => s.Name);
                break;
        }

        // Применяем фильтры
        enterprises = enterprises.Where(o =>
            (string.IsNullOrEmpty(searchEnterpriseName) || o.Name.StartsWith(searchEnterpriseName)) &&
            (string.IsNullOrEmpty(searchEnterpriseAddress) || o.Address.Contains(searchEnterpriseAddress))
        );

        return enterprises;
    }
}