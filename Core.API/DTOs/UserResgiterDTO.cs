using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.DTOs
{
    public class UserResgiterDTO
    {
        [Required(ErrorMessage = "pleas fill the username")]
        [MaxLength(255, ErrorMessage ="long user name !!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "pleas fill the email address")]
        [EmailAddress(ErrorMessage = "pleas enter the correct format for email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "pleas fill the phone number")]
        [Phone(ErrorMessage = "pleas enter the correct format for number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "pleas fill the password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "pleas fill the repassword")]
        [Compare("Password", ErrorMessage ="repassword is not match with password")]
        public string Repassword { get; set; }
    }
}
