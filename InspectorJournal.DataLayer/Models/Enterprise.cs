using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.DataLayer.Models
{
    public partial class Enterprise
    {
        [Key]
        [Display(Name = "Код Предприятия")]
        public int EnterpriseId { get; set; }

        [Required]
        [Display(Name = "Предприятие")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Тип Собственности")]
        public string OwnershipType { get; set; } = null!;

        [Required]
        [Display(Name = "Адрес")]
        public string Address { get; set; } = null!;

        [Required]
        [Display(Name = "Управляющий")]
        public string DirectorName { get; set; } = null!;

        [Required]
        [Display(Name = "Номер Управляющего")]
        public string DirectorPhone { get; set; } = null!;

        public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}
