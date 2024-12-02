namespace InspectorJournal.ViewModels;

public enum SortState
{
    No, // не сортировать
    EnterpriseNameAsc, // по предприятию в алфавитном порядке
    EnterpriseNameDesc, // по предприятию в обратном порядке
    InspectorNameAsc, // по инспектору в алфавитном порядке
    InspectorNameDesc, // по инспектору в обратном порядке
    ViolationTypeAsc, // по типу нарушения в алфавитном порядке
    ViolationTypeDesc, // по типу нарушения в обратном порядке
    DepartmentAsc, // по департаменту в алфавитном порядке
    DepartmentDesc, // по департаменту в обратном порядке
    NumberOfInspectionsAsc, // по количеству проверок по возрастанию
    NumberOfInspectionsDesc, // по количеству проверок по убыванию
    TotalPenaltyAmountAsc, // по сумме штрафов по возрастанию
    TotalPenaltyAmountDesc, // по сумме штрафов по убыванию
    MaxPenaltyAmountAsc, // по максимальному штрафу по возрастанию
    MaxPenaltyAmountDesc, // по максимальному штрафу по убыванию
    PenaltyAmountAsc, // по сумме задолженности по возрастанию
    PenaltyAmountDesc // по сумме задолженности по убыванию
}

public class SortViewModel
{
    public SortState EnterpriseNameSort { get; set; } // значение для сортировки по предприятию
    public SortState InspectorNameSort { get; set; } // значение для сортировки по инспектору
    public SortState ViolationTypeSort { get; set; } // значение для сортировки по типу нарушения
    public SortState DepartmentSort { get; set; } // значение для сортировки по департаменту
    public SortState NumberOfInspectionsSort { get; set; } // значение для сортировки по количеству проверок
    public SortState TotalPenaltyAmountSort { get; set; } // значение для сортировки по сумме штрафов
    public SortState MaxPenaltyAmountSort { get; set; } // значение для сортировки по максимальному штрафу
    public SortState PenaltyAmountSort { get; set; } // значение для сортировки по сумме задолженности
    public SortState CurrentState { get; set; } // текущее значение сортировки

    public SortViewModel(SortState sortOrder)
    {
        EnterpriseNameSort = sortOrder == SortState.EnterpriseNameAsc
            ? SortState.EnterpriseNameDesc
            : SortState.EnterpriseNameAsc;
        InspectorNameSort = sortOrder == SortState.InspectorNameAsc
            ? SortState.InspectorNameDesc
            : SortState.InspectorNameAsc;
        ViolationTypeSort = sortOrder == SortState.ViolationTypeAsc
            ? SortState.ViolationTypeDesc
            : SortState.ViolationTypeAsc;
        DepartmentSort = sortOrder == SortState.DepartmentAsc
            ? SortState.DepartmentDesc
            : SortState.DepartmentAsc;
        NumberOfInspectionsSort = sortOrder == SortState.NumberOfInspectionsAsc
            ? SortState.NumberOfInspectionsDesc
            : SortState.NumberOfInspectionsAsc;
        TotalPenaltyAmountSort = sortOrder == SortState.TotalPenaltyAmountAsc
            ? SortState.TotalPenaltyAmountDesc
            : SortState.TotalPenaltyAmountAsc;
        MaxPenaltyAmountSort = sortOrder == SortState.MaxPenaltyAmountAsc
            ? SortState.MaxPenaltyAmountDesc
            : SortState.MaxPenaltyAmountAsc;
        PenaltyAmountSort = sortOrder == SortState.PenaltyAmountAsc
            ? SortState.PenaltyAmountDesc
            : SortState.PenaltyAmountAsc;

        CurrentState = sortOrder;
    }

    public string GetSortIndicator(SortState state)
    {
        return CurrentState == state ? "▲" :
            CurrentState == InvertState(state) ? "▼" : "";
    }

    private SortState InvertState(SortState state)
    {
        return state switch
        {
            SortState.EnterpriseNameAsc => SortState.EnterpriseNameDesc,
            SortState.EnterpriseNameDesc => SortState.EnterpriseNameAsc,
            SortState.InspectorNameAsc => SortState.InspectorNameDesc,
            SortState.InspectorNameDesc => SortState.InspectorNameAsc,
            SortState.ViolationTypeAsc => SortState.ViolationTypeDesc,
            SortState.ViolationTypeDesc => SortState.ViolationTypeAsc,
            SortState.DepartmentAsc => SortState.DepartmentDesc,
            SortState.DepartmentDesc => SortState.DepartmentAsc,
            SortState.NumberOfInspectionsAsc => SortState.NumberOfInspectionsDesc,
            SortState.NumberOfInspectionsDesc => SortState.NumberOfInspectionsAsc,
            SortState.TotalPenaltyAmountAsc => SortState.TotalPenaltyAmountDesc,
            SortState.TotalPenaltyAmountDesc => SortState.TotalPenaltyAmountAsc,
            SortState.MaxPenaltyAmountAsc => SortState.MaxPenaltyAmountDesc,
            SortState.MaxPenaltyAmountDesc => SortState.MaxPenaltyAmountAsc,
            SortState.PenaltyAmountAsc => SortState.PenaltyAmountDesc,
            SortState.PenaltyAmountDesc => SortState.PenaltyAmountAsc,
            _ => state
        };
    }
}