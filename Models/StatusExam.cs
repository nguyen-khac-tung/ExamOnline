﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExamationOnline.Models;

public partial class StatusExam
{
    public int StatusId { get; set; }

    public string StatusName { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<UserExam> UserExams { get; set; } = new List<UserExam>();
}