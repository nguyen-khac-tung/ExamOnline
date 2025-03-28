﻿using System.ComponentModel.DataAnnotations;

namespace ExamationOnline.ViewModels
{
    public class AccountLoginVM
    {
        [Required(ErrorMessage = "Email can not be empty.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password can not be empty.")]
        public string? Password { get; set; }
    }
}
