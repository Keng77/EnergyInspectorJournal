using InspectorJournal.DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels.AdditionalViewModels
{
    public class InspectorWorksViewModel
    {
        public List<InspectorWorksItemViewModel> Inspections { get; set; }
        public FilterInspectorWorksViewModel Filters { get; set; }
        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }

        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
