using ExamationOnline.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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

    public class ExamViewModel
    {
        public string? ExamId { get; set; }

        [Required(ErrorMessage = "Exam name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Exam name must be between 5 and 100 characters")]
        public string ExamName { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive integer")]
        public int Duration { get; set; }

        public int? ClassId { get; set; }

        public int? SubjectId { get; set; }

        public bool IsDisplayAnswer { get; set; } = false;

        public int? StatusId { get; set; }

        public int? LectureId { get; set; }

        // For dropdown lists
        public List<Class>? ClassList { get; set; }
        public List<Subject>? SubjectList { get; set; }

        // Additional properties to track state
        public bool? IsExamTaken { get; set; } // Has any student taken the exam?
    }

    public class AddQuestionViewModel
    {
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public List<QuestionListViewModel>? AvailableQuestions { get; set; }
        public List<string> SelectedQuestionIds { get; set; } = new List<string>();
    }
}
