using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class InspectionsViewModel
    {
        public IEnumerable<Inspection> Inspections { get; set; }
        //Свойство для фильтрации
        public FilterInspectionViewModel FilterInspectionViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }

    }
}
