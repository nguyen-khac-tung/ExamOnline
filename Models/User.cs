﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExamationOnline.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public bool? Gender { get; set; }

    public DateOnly? DoB { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public string Role { get; set; }

    public string Password { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<ClassUser> ClassUsers { get; set; } = new List<ClassUser>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<SubjectUser> SubjectUsers { get; set; } = new List<SubjectUser>();

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    public virtual ICollection<UserExam> UserExams { get; set; } = new List<UserExam>();
}