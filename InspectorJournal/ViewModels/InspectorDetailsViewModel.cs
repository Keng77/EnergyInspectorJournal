using InspectorJournal.DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class InspectorDetailsViewModel
    {
        // Коллекция проверок
        public IEnumerable<Inspection> Inspections { get; set; } = new List<Inspection>();

        public int InspectorId { get; set; }
        [Display(Name = "Инспектор")]
        public string InspectorName { get; set; }

        [Display(Name = "Предприятие")]
        public string EnterpriseName { get; set; }

        [Display(Name = "Тип нарушения")]
        public string ViolationType { get; set; }

        [Display(Name = "Сумма задолженности")]
        public decimal PenaltyAmount { get; set; }

        [Display(Name = "Статус оплаты")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Статус исправления")]
        public string CorrectionStatus { get; set; }

        // Фильтры для страницы
        public FilterInspectorDetailsViewModel Filters { get; set; } = new FilterInspectorDetailsViewModel();
        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }

        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }


}
