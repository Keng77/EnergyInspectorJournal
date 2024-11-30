using System.ComponentModel.DataAnnotations;
using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.ViewModels;

public class InspectorsViewModel
{
    public IEnumerable<Inspector> Inspectors { get; set; }

    //Свойство для фильтрации
    //Код Инспектора
    [Display(Name = "Код Инспектора")]
    public int InspectorId { get; set; }

    [Display(Name = "Проверяющий")]
    public string FullName { get; set; } = null!;

    [Display(Name = "Департамент")]
    public string Department { get; set; } = null!;

    //Свойство для навигации по страницам
    public PageViewModel PageViewModel { get; set; }

    // Порядок сортировки
    public SortViewModel SortViewModel { get; set; }
}