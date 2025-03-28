namespace ExamationOnline.Areas.Lecture.ViewModels
{
    public class ExamListViewModel
    {
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public int? TotalQuestion { get; set; }
        public DateTime? CreateDate { get; set; }
        public string StatusName { get; set; }
        public int? StatusId { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ExamListingViewModel
    {
        public List<ExamListViewModel> Exams { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string SortColumn { get; set; }
        public bool IsAscending { get; set; }
    }
}
