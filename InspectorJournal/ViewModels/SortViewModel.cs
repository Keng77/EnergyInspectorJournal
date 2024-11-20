namespace InspectorJournal.ViewModels
{
    public enum SortState
    {
        No, // не сортировать
        EnterpriseNameAsc,       // по предприятию в алфавитном порядке
        EnterpriseNameDesc,     // по предприятию в обратном порядке
        ViolationTypeAsc,      // по типу нарушения в алфавитном порядке
        ViolationTypeDesc,    // по типу нарушения в обратном порядке
        PenaltyAmountAsc,    // по сумме задолженности по возрастанию
        PenaltyAmountDesc   // по сумме задолженности по убыванию


    }
    public class SortViewModel
    {
        public SortState EnterpriseNameSort { get; set; } // значение для сортировки по предприятию
        public SortState ViolationTypeSort { get; set; }    // значение для сортировки по типу нарушения
        public SortState PenaltyAmountSort { get; set; }    // значение для сортировки по сумме задолженности

        public SortState CurrentState { get; set; }     // текущее значение сортировки

        public SortViewModel(SortState sortOrder)
        {
            EnterpriseNameSort = sortOrder == SortState.EnterpriseNameAsc ? SortState.EnterpriseNameDesc : SortState.EnterpriseNameAsc;
            ViolationTypeSort = sortOrder == SortState.ViolationTypeAsc ? SortState.ViolationTypeDesc : SortState.ViolationTypeAsc;
            PenaltyAmountSort = sortOrder == SortState.PenaltyAmountAsc ? SortState.PenaltyAmountDesc : SortState.PenaltyAmountAsc;

            CurrentState = sortOrder;
        }



    }
}
