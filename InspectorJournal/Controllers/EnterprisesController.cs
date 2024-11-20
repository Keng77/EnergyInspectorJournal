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
    public class EnterprisesController : Controller
    {
        private readonly InspectionsDbContext _context;

        public EnterprisesController(InspectionsDbContext context)
        {
            _context = context;
        }

        // GET: Enterprises
        public async Task<IActionResult> Index()
        {
            return View(await _context.Enterprises.ToListAsync());
        }

        // GET: Enterprises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enterprise = await _context.Enterprises
                .FirstOrDefaultAsync(m => m.EnterpriseId == id);
            if (enterprise == null)
            {
                return NotFound();
            }

            return View(enterprise);
        }

        // GET: Enterprises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Enterprises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnterpriseId,Name,OwnershipType,Address,DirectorName,DirectorPhone")] Enterprise enterprise)
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
            if (id == null)
            {
                return NotFound();
            }

            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }
            return View(enterprise);
        }

        // POST: Enterprises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnterpriseId,Name,OwnershipType,Address,DirectorName,DirectorPhone")] Enterprise enterprise)
        {
            if (id != enterprise.EnterpriseId)
            {
                return NotFound();
            }

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
            return View(enterprise);
        }

        // GET: Enterprises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enterprise = await _context.Enterprises
                .FirstOrDefaultAsync(m => m.EnterpriseId == id);
            if (enterprise == null)
            {
                return NotFound();
            }

            return View(enterprise);
        }

        // POST: Enterprises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise != null)
            {
                _context.Enterprises.Remove(enterprise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnterpriseExists(int id)
        {
            return _context.Enterprises.Any(e => e.EnterpriseId == id);
        }
    }
}
