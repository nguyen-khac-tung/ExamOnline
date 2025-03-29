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

    public class TakeExamViewModel
    {
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public int Duration { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ExamQuestionViewModel> Questions { get; set; } = new List<ExamQuestionViewModel>();
    }

    public class ExamQuestionViewModel
    {
        public string QuestionId { get; set; }
        public string Content { get; set; }
        public List<ExamOptionViewModel> Options { get; set; } = new List<ExamOptionViewModel>();
    }

    public class ExamOptionViewModel
    {
        public int OptionId { get; set; }
        public string Content { get; set; }
    }

    public class SubmitExamViewModel
    {
        public string ExamId { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }

    public class AnswerViewModel
    {
        public string QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
    }

    public class ExamResultViewModel
    {
        public string ExamId { get; set; }
        public string ExamName { get; set; }
        public int Duration { get; set; }
        public int TimeTaken { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool DisplayAnswers { get; set; }
        public List<AnswerResultViewModel> Answers { get; set; } = new List<AnswerResultViewModel>();
    }

    public class AnswerResultViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public int? SelectedOptionId { get; set; }
        public string SelectedOptionContent { get; set; }
        public int? CorrectOptionId { get; set; }
        public string CorrectOptionContent { get; set; }
        public bool IsCorrect { get; set; }
    }
}
