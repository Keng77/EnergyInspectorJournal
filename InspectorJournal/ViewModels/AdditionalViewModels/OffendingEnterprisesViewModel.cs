using InspectorJournal.DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels.AdditionalViewModels
{
    public class OffendingEnterprisesViewModel
    {
        // Коллекция проверок
        public IEnumerable<Inspection> Inspections { get; set; } = new List<Inspection>();

        [Display(Name = "Предприятие")]
        public string Name { get; set; } = null!;

        [Display(Name = "Управляющий")]
        public string DirectorName { get; set; } = null!;

        [Display(Name = "Номер Управляющего")]
        public string DirectorPhone { get; set; } = null!;

        [Display(Name = "Тип нарушения")]
        public string ViolationType { get; set; } = null!;

        [Display(Name = "Статус Исправления")]
        public string? CorrectionStatus { get; set; }

        [Display(Name = "Статус Оплаты")]
        public string? PaymentStatus { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }

        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }

    }
}
