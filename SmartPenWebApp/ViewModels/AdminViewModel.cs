using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSignWebApp.ViewModels
{
    public class AdminViewModel
    {
        /*
         Data annotations, Used for validation
         ?Sever side validation?
         Required: Data annotations  
        */

        [Required]
        [MinLength(4, ErrorMessage = "Must be at least 4 character")]
        public string fName { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Must be at least 4 character")]
        public string lName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [MaxLength(1000, ErrorMessage = "Enter a short message, less than 1000 characters")]
        public string message { get; set; }
        
        /*Doesnt work, would have been nice
         public File document { get; set; }
         */

    }
}
