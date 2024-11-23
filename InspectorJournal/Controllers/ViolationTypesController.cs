using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.ViewModels;
using Microsoft.IdentityModel.Tokens;
using InspectorJournal.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace InspectorJournal.Controllers
{
    [Authorize]
    public class ViolationTypesController : Controller
    {
        private readonly InspectionsDbContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public ViolationTypesController(InspectionsDbContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: ViolationTypes
        [SetToSession("ViolationType")] // Фильтр действий для сохранения в сессию параметров отбора
        public async Task<IActionResult> Index(string ViolationTypeName = "",  SortState sortOrder = SortState.No, int page = 1)
        {
            // Если параметры фильтрации пустые, считываем их из сессии
            if (string.IsNullOrEmpty(ViolationTypeName))
            {
                var sessionData = Infrastructure.SessionExtensions.Get(HttpContext.Session, "ViolationType");
                if (sessionData != null)
                {
                    var violationTypesViewModel = Transformations.DictionaryToObject<ViolationTypesViewModel>(sessionData);
                    ViolationTypeName = violationTypesViewModel?.Name ?? "";
                }
            }

            // Сортировка и фильтрация данных
            IQueryable<ViolationType> violationTypesContext = _context.ViolationTypes;
            violationTypesContext = Sort_Search(violationTypesContext, sortOrder, ViolationTypeName);

            // Разбиение на страницы
            var count = violationTypesContext.Count();
            var violations = violationTypesContext.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Формирование модели для передачи представлению
            ViolationTypesViewModel viewModel = new()
            {
                ViolationTypes = violations,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                Name = ViolationTypeName,
            };

            return View(viewModel);
        }

        // GET: ViolationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var violationType = await _context.ViolationTypes
                .FirstOrDefaultAsync(m => m.ViolationTypeId == id);
            if (violationType == null)
            {
                return NotFound();
            }

            return View(violationType);
        }

        // GET: ViolationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ViolationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViolationTypeId,Name,PenaltyAmount,CorrectionPeriodDays")] ViolationType violationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(violationType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(violationType);
        }

        // GET: ViolationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var violationType = await _context.ViolationTypes.FindAsync(id);
            if (violationType == null)
            {
                return NotFound();
            }
            return View(violationType);
        }

        // POST: ViolationTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ViolationTypeId,Name,PenaltyAmount,CorrectionPeriodDays")] ViolationType violationType)
        {
            if (id != violationType.ViolationTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(violationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViolationTypeExists(violationType.ViolationTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(violationType);
        }

        // GET: ViolationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var violationType = await _context.ViolationTypes
                .FirstOrDefaultAsync(m => m.ViolationTypeId == id);
            if (violationType == null)
            {
                return NotFound();
            }

            return View(violationType);
        }

        // POST: ViolationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var violationType = await _context.ViolationTypes.FindAsync(id);
            if (violationType != null)
            {
                _context.ViolationTypes.Remove(violationType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViolationTypeExists(int id)
        {
            return _context.ViolationTypes.Any(e => e.ViolationTypeId == id);
        }

        private static IQueryable<ViolationType> Sort_Search(IQueryable<ViolationType> violationTypes, SortState sortOrder, string searchviolationTypenName)
        {
            // Применяем сортировку
            switch (sortOrder)
            {
                case SortState.ViolationTypeAsc:
                    violationTypes = violationTypes.OrderBy(s => s.Name);
                    break;
                case SortState.ViolationTypeDesc:
                    violationTypes = violationTypes.OrderByDescending(s => s.Name);
                    break;
            }

            // Применяем фильтры
            violationTypes = violationTypes.Where(o =>
                                                 (string.IsNullOrEmpty(searchviolationTypenName) || o.Name.Contains(searchviolationTypenName))
                                                 );

            return violationTypes;
        }
    }
}
