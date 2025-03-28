using System.ComponentModel.DataAnnotations;

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
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string SortColumn { get; set; }
        public bool IsAscending { get; set; }
    }

    public class QuestionCreateViewModel
    {
        [Required(ErrorMessage = "Question content is required")]
        [MinLength(10, ErrorMessage = "Question content must be at least 10 characters")]
        public string Content { get; set; }

        public string? Type { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public int? LectureID { get; set; }

        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
    }

    public class OptionViewModel
    {
        [Required(ErrorMessage = "Option content is required")]
        public string Content { get; set; }

        public bool? IsCorrect { get; set; } = false;
    }
}
