using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.ViewModels;

public class HomeViewModel
{
    public IEnumerable<Enterprise> Enterprises { get; set; }

    public IEnumerable<Inspector> Inspectors { get; set; }

    public IEnumerable<ViolationType> ViolationTypes { get; set; }

    public IEnumerable<FilterInspectionViewModel> Inspections { get; set; }
}