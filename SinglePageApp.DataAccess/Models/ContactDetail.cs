using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SinglePageApp.Models
{
    public class ContactDetail
    {
        public string ContactId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string Avatar { get; set; }
    }
}