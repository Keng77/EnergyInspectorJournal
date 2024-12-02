using System.ComponentModel.DataAnnotations;
using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.ViewModels.AdditionalViewModels
{
    public class InspectorWorksItemViewModel
    {
        public Inspector Inspector { get; set; }
        public List<Inspection> Inspections { get; set; }
        public int NumberOfInspections { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public decimal MaxPenaltyAmount { get; set; }
    }

}
