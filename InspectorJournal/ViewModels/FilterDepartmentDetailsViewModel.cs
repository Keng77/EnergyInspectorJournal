namespace InspectorJournal.ViewModels
{
    public class FilterDepartmentDetailsViewModel
    {
        public string? InspectorName { get; set; }
        public string? EnterpriseName { get; set; }
        public string? ViolationType { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CorrectionStatus { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(InspectorName)
                   && string.IsNullOrEmpty(EnterpriseName)
                   && string.IsNullOrEmpty(ViolationType)
                   && string.IsNullOrEmpty(PaymentStatus)
                   && string.IsNullOrEmpty(CorrectionStatus);
        }
    }
}
