using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.DataLayer.Models
{
    public partial class ViolationType
    {
        [Key]
        [Display(Name = "Код Нарушения")]
        public int ViolationTypeId { get; set; }

        
        [Display(Name = "Тип нарушения")]
        public string Name { get; set; } = null!;

        
        [Display(Name = "Задолженность")]
        public decimal PenaltyAmount { get; set; }

        
        [Display(Name = "Дедлайн исправления")]
        public int CorrectionPeriodDays { get; set; }

        public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}
