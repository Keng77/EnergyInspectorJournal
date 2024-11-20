using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectorJournal.DataLayer.Models
{
    public partial class Inspection
    {
        [Key]
        [Display(Name = "Код проверки")]
        public int InspectionId { get; set; }

        [ForeignKey("Inspector")]
        [Display(Name = "Код Испектора")]
        public int InspectorId { get; set; }

        [ForeignKey("Enterprise")]
        [Display(Name = "Код Предприятия")]
        public int EnterpriseId { get; set; }

        [ForeignKey("ViolationType")]
        [Display(Name = "Код Нарушения")]
        public int ViolationTypeId { get; set; }

        [Display(Name = "Дата Проверки")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Дата проверки обязательна для заполнения.")]
        public DateOnly InspectionDate { get; set; }

        [Display(Name = "Номер Протокола")]
        [StringLength(50, ErrorMessage = "Номер протокола не может быть длиннее 50 символов.")]
        public string ProtocolNumber { get; set; }

        [Display(Name = "Ответственный")]
        [StringLength(100, ErrorMessage = "Имя ответственного не может быть длиннее 100 символов.")]
        public string ResponsiblePerson { get; set; }

        [Required(ErrorMessage = "Задолженность обязательна для заполнения.")]
        [Range(0, double.MaxValue, ErrorMessage = "Задолженность должна быть положительным числом.")]
        [Display(Name = "Задолженность")]
        public decimal PenaltyAmount { get; set; }

        [Display(Name = "Дедлайн Оплаты")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Дедлайн оплаты обязательна для заполнения.")]
        public DateOnly PaymentDeadline { get; set; }

        [Display(Name = "Дедлайн исправления")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Дедлайн исправления обязательна для заполнения.")]
        public DateOnly CorrectionDeadline { get; set; }

        [Required(ErrorMessage = "Статус оплаты обязателен.")]
        [Display(Name = "Статус Оплаты")]
        public string? PaymentStatus { get; set; }

        [Required(ErrorMessage = "Статус исправления обязателен.")]
        [Display(Name = "Статус Исправления")]
        public string? CorrectionStatus { get; set; }

        public virtual Enterprise Enterprise { get; set; }

        public virtual Inspector Inspector { get; set; }

        public virtual ViolationType ViolationType { get; set; }
    }
}
