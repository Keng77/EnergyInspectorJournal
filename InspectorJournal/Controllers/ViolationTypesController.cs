using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.Controllers
{
    public class ViolationTypesController : Controller
    {
        private readonly InspectionsDbContext _context;

        public ViolationTypesController(InspectionsDbContext context)
        {
            _context = context;
        }

        // GET: ViolationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ViolationTypes.ToListAsync());
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
