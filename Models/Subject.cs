﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExamationOnline.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; }

    public string Description { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<SubjectUser> SubjectUsers { get; set; } = new List<SubjectUser>();
}