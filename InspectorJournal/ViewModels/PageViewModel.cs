namespace InspectorJournal.ViewModels;

//Класс для хранения информации о страницах разбиения
public class PageViewModel
{
    public int PageNumber { get; }
    public int TotalPages { get; }

    public PageViewModel(int count, int pageNumber = 1, int pageSize = 20)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}