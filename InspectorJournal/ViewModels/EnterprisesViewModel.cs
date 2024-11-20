using InspectorJournal.ViewModels;
using InspectorJournal.DataLayer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class EnterprisesViewModel
    {
        public IEnumerable<Enterprise> Enterprises { get; set; }
        //Код Предприятия
        [Display(Name = "Код Предприятия")]
        public int EnterpriseId { get; set; }

        [Display(Name = "Предприятие")]
        public string Name { get; set; } = null!;

        [Display(Name = "Тип Собственности")]
        public string OwnershipType { get; set; } = null!;

        [Display(Name = "Адресс")]
        public string Address { get; set; } = null!;

        [Display(Name = "Управляющий")]
        public string DirectorName { get; set; } = null!;

        [Display(Name = "Номер Управляющего")]
        public string DirectorPhone { get; set; } = null!;

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SortViewModel SortViewModel { get; set; }
    }
}
