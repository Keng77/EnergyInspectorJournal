using InspectorJournal.DataLayer.Data;
using InspectorJournal.Models;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InspectorJournal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly InspectionsDbContext _context;
        public HomeController(InspectionsDbContext db)
        {
            _context = db;
        }
        public IActionResult Index()
        {
            int numberRows = 15;
            var enterprises = _context.Enterprises.Take(numberRows).ToList();
            var inspectors = _context.Inspectors.Take(numberRows).ToList();
            var violationTypes = _context.ViolationTypes.Take(numberRows).ToList();
            List<FilterInspectionViewModel> inspections = [.. _context.Inspections
                .OrderBy(d => d.InspectionId)
                .Select(t => new FilterInspectionViewModel
                {
                    Inspector = t.Inspector.FullName,
                    InspectionId = t.InspectionId,
                    Enterprise = t.Enterprise.Name,
                    InspectionDate = t.InspectionDate,
                    ProtocolNumber = t.ProtocolNumber,
                    ViolationType = t.ViolationType.Name,
                    ResponsiblePerson = t.ResponsiblePerson,
                    PenaltyAmount = t.ViolationType.PenaltyAmount,
                    PaymentDeadline = t.PaymentDeadline,
                    CorrectionDeadline = t.CorrectionDeadline,
                    PaymentStatus = t.PaymentStatus,
                    CorrectionStatus = t.CorrectionStatus,
                })
                .Take(numberRows)];

            HomeViewModel homeViewModel = new()
            {
                Enterprises = enterprises,
                Inspectors = inspectors,
                ViolationTypes = violationTypes,
                Inspections = inspections
            };
            return View(homeViewModel);
        }
    }
}