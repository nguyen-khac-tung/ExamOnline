namespace ExamationOnline.Areas.Student.ViewModels
{
    public class StudentExamListViewModel
    {
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public int? TotalQuestion { get; set; }
        public bool HasTaken { get; set; }
    }

    public class StudentExamListingViewModel
    {
        public List<StudentExamListViewModel> Exams { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string SortColumn { get; set; }
        public bool IsAscending { get; set; }
    }
}
