using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.DTO
{
    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"(?!^[0-9]$)(?!^[a-zA-Z]$)^([a-zA-Z0-9]{6,15})$",
        ErrorMessage = "Password requires at least one digit, at least one alphabetic character. No special characters  and no spaces. At least 6 characters in length.")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"(?!^[0-9]$)(?!^[a-zA-Z]$)^([a-zA-Z0-9]{6,15})$",
        ErrorMessage = "Password requires at least one digit, at least one alphabetic character. No special characters  and no spaces. At least 6 characters in length.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [NotMapped]
        public string NewPassword2 { get; set; }
    }
}
