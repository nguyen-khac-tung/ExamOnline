namespace ExamationOnline.Areas.Lecture.ViewModels
{
    //Create viewmodel classes related to question here
    public class QuestionListViewModel
    {
        public string QuestionId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? HasAnswer { get; set; }
        public bool? IsActive { get; set; }
        public bool CanDelete { get; set; }
    }

    public class QuestionListingViewModel
    {
        public List<QuestionListViewModel> Questions { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string SortColumn { get; set; }
        public bool IsAscending { get; set; }
    }
}
