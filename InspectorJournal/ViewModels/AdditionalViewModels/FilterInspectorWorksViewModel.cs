using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels.AdditionalViewModels
{
    public class FilterInspectorWorksViewModel
    {
        [Display(Name = "Инспектор")]
        public string? InspectorName { get; set; }
        [Display(Name = "Департамент")]
        public string? Department { get; set; }

        [Display(Name = "Количество проверок")]
        public int NumberOfInspections { get; set; }

        [Display(Name = "Сумма штрафов")]
        public decimal TotalPenaltyAmount { get; set; }
        [Display(Name = "Максимальный штраф")]
        public decimal MaxPenaltyAmount { get; set; }

        // Метод проверки, пуст ли фильтр
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(InspectorName)
                   && string.IsNullOrEmpty(Department)
                   && NumberOfInspections == 0
                   && TotalPenaltyAmount == 0
                   && MaxPenaltyAmount == 0;
        }

    }
}