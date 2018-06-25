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


        public string docURL { get; set; }

        [Required (ErrorMessage = "The First Name field is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        public string fName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
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
