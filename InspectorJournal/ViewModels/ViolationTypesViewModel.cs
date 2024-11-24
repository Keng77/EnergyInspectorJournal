using System.ComponentModel.DataAnnotations;
using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.ViewModels;

public class ViolationTypesViewModel
{
    public IEnumerable<ViolationType> ViolationTypes { get; set; }

    //Код Нарушения
    [Display(Name = "Код Нарушения")] public int ViolationTypeId { get; set; }

    [Display(Name = "Тип нарушения")] public string Name { get; set; } = null!;

    [Display(Name = "Задолженность")] public decimal PenaltyAmount { get; set; }

    [Display(Name = "Дедлайн исправления")]
    public int CorrectionPeriodDays { get; set; }

    //Свойство для навигации по страницам
    public PageViewModel PageViewModel { get; set; }

    // Порядок сортировки
    public SortViewModel SortViewModel { get; set; }
}