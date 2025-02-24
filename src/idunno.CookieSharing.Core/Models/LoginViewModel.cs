﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace idunno.CookieSharing.Core
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
