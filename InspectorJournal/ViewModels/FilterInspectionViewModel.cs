using System;
using System.ComponentModel.DataAnnotations;

namespace InspectorJournal.ViewModels
{
    public class FilterInspectionViewModel
    {
        //Код проверки
        public int InspectionId { get; set; }

        [Display(Name = "Инспектор")]
        public string Inspector { get; set; }

        [Display(Name = "Предприятие")]
        public string Enterprise { get; set; }

        [Display(Name = "Дата Проверки")]
        [DataType(DataType.Date)]
        public DateOnly InspectionDate { get; set; }

        [Display(Name = "Номер Протокола")]
        public string ProtocolNumber { get; set; }

        [Display(Name = "Тип Нарушения")]
        public string ViolationType { get; set; }

        [Display(Name = "Ответственный")]
        public string ResponsiblePerson { get; set; }

        [Display(Name = "Сумма задолженности")]
        public decimal PenaltyAmount { get; set; }

        [Display(Name = "Дедлайн Оплаты")]
        [DataType(DataType.Date)]
        public DateOnly PaymentDeadline { get; set; }

        [Display(Name = "Дедлайн исправления")]
        [DataType(DataType.Date)]
        public DateOnly CorrectionDeadline { get; set; }

        [Display(Name = "Статус Оплаты")]
        public string? PaymentStatus { get; set; }

        [Display(Name = "Статус Исправления")]
        public string? CorrectionStatus { get; set; }


    }
}
