﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ExamationOnline.Models;

public partial class UserExam
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string ExamId { get; set; }

    public int? TimeTaken { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? CorrectAnswer { get; set; }

    public int? StatusId { get; set; }

    public virtual Exam Exam { get; set; }

    public virtual StatusExam Status { get; set; }

    public virtual User User { get; set; }
}