using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using System.Collections.Generic;

namespace InspectorJournal.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Enterprise> Enterprises { get; set; }

        public IEnumerable<Inspector> Inspectors { get; set; }

        public IEnumerable<ViolationType> ViolationTypes { get; set; }

        public IEnumerable<FilterInspectionViewModel> Inspections { get; set; }

    }

}
