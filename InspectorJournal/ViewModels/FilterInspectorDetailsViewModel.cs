namespace InspectorJournal.ViewModels
{
    public class FilterInspectorDetailsViewModel
    {
        public string? EnterpriseName { get; set; }
        public string? ViolationType { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CorrectionStatus { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(EnterpriseName)
                   && string.IsNullOrEmpty(ViolationType)
                   && string.IsNullOrEmpty(PaymentStatus)
                   && string.IsNullOrEmpty(CorrectionStatus);
        }
    }
}
