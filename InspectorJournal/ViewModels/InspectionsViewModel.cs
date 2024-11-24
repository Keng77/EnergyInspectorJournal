using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.ViewModels;

public class InspectionsViewModel
{
    public IEnumerable<Inspection> Inspections { get; set; }

    //�������� ��� ����������
    public FilterInspectionViewModel FilterInspectionViewModel { get; set; }

    //�������� ��� ��������� �� ���������
    public PageViewModel PageViewModel { get; set; }

    // ������� ����������
    public SortViewModel SortViewModel { get; set; }
}