namespace InspectorJournal.ViewModels
{
    public enum SortState
    {
        No, // не сортировать
        EnterpriseNameAsc,          // по предприятию в алфавитном порядке
        EnterpriseNameDesc,        // по предприятию в обратном порядке
        InspectorNameAsc,         // по инспектору в алфавитном порядке
        InspectorNameDesc,       // по инспектору в обратном порядке
        ViolationTypeAsc,       // по типу нарушения в алфавитном порядке
        ViolationTypeDesc,     // по типу нарушения в обратном порядке
        PenaltyAmountAsc,     // по сумме задолженности по возрастанию
        PenaltyAmountDesc    // по сумме задолженности по убыванию



    }
    public class SortViewModel
    {
        public SortState EnterpriseNameSort { get; set; } // значение для сортировки по предприятию
        public SortState InspectorNameSort { get; set; } // значение для сортировки по инспектору
        public SortState ViolationTypeSort { get; set; }    // значение для сортировки по типу нарушения
        public SortState PenaltyAmountSort { get; set; }    // значение для сортировки по сумме задолженности
        public SortState CurrentState { get; set; }     // текущее значение сортировки

        public SortViewModel(SortState sortOrder)
        {
            EnterpriseNameSort = sortOrder == SortState.EnterpriseNameAsc ? SortState.EnterpriseNameDesc : SortState.EnterpriseNameAsc;
            InspectorNameSort  = sortOrder == SortState.InspectorNameAsc  ? SortState.InspectorNameDesc  : SortState.InspectorNameAsc;
            ViolationTypeSort  = sortOrder == SortState.ViolationTypeAsc  ? SortState.ViolationTypeDesc  : SortState.ViolationTypeAsc;
            PenaltyAmountSort  = sortOrder == SortState.PenaltyAmountAsc  ? SortState.PenaltyAmountDesc  : SortState.PenaltyAmountAsc;

            CurrentState = sortOrder;
        }



    }
}
