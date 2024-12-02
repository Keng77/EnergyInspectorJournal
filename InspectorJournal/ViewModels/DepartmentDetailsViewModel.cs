using InspectorJournal.DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class DepartmentDetailsViewModel
    {
        // Коллекция проверок
        public IEnumerable<Inspection> Inspections { get; set; } = new List<Inspection>();

        public string Department { get; set; }

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
        public FilterDepartmentDetailsViewModel Filters { get; set; } = new FilterDepartmentDetailsViewModel();
        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }

        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }

}
