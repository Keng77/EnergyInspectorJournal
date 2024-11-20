using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.Infrastructure;

namespace InspectorJournal.Controllers
{
    public class InspectorsController : Controller
    {
        private readonly InspectionsDbContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public InspectorsController(InspectionsDbContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Inspectors
        [SetToSession("Inspector")] // Фильтр действий для сохранения параметров в сессии
        public IActionResult Index(string InspectorName = "", SortState sortOrder = SortState.No, int page = 1)
        {
            // Если фильтр пустой, восстанавливаем данные из сессии
            if (string.IsNullOrWhiteSpace(InspectorName))
            {
                var sessionData = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Inspector");
                if (sessionData != null)
                {
                    var inspectorViewModel = Transformations.DictionaryToObject<InspectorsViewModel>(sessionData);
                    InspectorName = inspectorViewModel?.FullName ?? "";
                }
            }

            // Сортировка и фильтрация
            IQueryable<Inspector> inspectorsContext = _context.Inspectors;
            inspectorsContext = Sort_Search(inspectorsContext, sortOrder, InspectorName);

            // Пагинация
            var count = inspectorsContext.Count();
            var inspectors = inspectorsContext.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Формирование ViewModel
            InspectorsViewModel viewModel = new()
            {
                Inspectors = inspectors,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FullName = InspectorName
            };

            return View(viewModel);
        }


        // GET: Inspectors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspector = await _context.Inspectors
                .FirstOrDefaultAsync(m => m.InspectorId == id);
            if (inspector == null)
            {
                return NotFound();
            }

            return View(inspector);
        }

        // GET: Inspectors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inspectors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InspectorId,FullName,Department")] Inspector inspector)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inspector);
        }

        // GET: Inspectors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspector = await _context.Inspectors.FindAsync(id);
            if (inspector == null)
            {
                return NotFound();
            }
            return View(inspector);
        }

        // POST: Inspectors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InspectorId,FullName,Department")] Inspector inspector)
        {
            if (id != inspector.InspectorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectorExists(inspector.InspectorId))
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
            return View(inspector);
        }

        // GET: Inspectors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspector = await _context.Inspectors
                .FirstOrDefaultAsync(m => m.InspectorId == id);
            if (inspector == null)
            {
                return NotFound();
            }

            return View(inspector);
        }

        // POST: Inspectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspector = await _context.Inspectors.FindAsync(id);
            if (inspector != null)
            {
                _context.Inspectors.Remove(inspector);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectorExists(int id)
        {
            return _context.Inspectors.Any(e => e.InspectorId == id);
        }

        private static IQueryable<Inspector> Sort_Search(IQueryable<Inspector> inspectors, SortState sortOrder, string searchInspectorName)
        {
            // Применяем сортировку
            switch (sortOrder)
            {
                case SortState.InspectorNameAsc:
                    inspectors = inspectors.OrderBy(s => s.FullName);
                    break;
                case SortState.InspectorNameDesc:
                    inspectors = inspectors.OrderByDescending(s => s.FullName);
                    break;
            }

            // Применяем фильтры
            inspectors = inspectors.Where(o =>
                                         (string.IsNullOrEmpty(searchInspectorName) || o.FullName.Contains(searchInspectorName)));

            return inspectors;
        }
    }
}
