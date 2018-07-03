using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public bool isSigned { get; set; }
        [Required]
        public string DocGuid { get; set; }

        public string SignedDocGuid { get; set; }
        [Required]
        public string DocName { get; set; }

        [Required (ErrorMessage = "The First Name field is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        public string fName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        public string lName { get; set; }

        [Required]
        [EmailAddress]
        public string notifyEmail { get; set; }

        [Required]
        [EmailAddress]
        public string signatoryEmail { get; set; }
        [MaxLength(1000, ErrorMessage = "Enter a short message, less than 1000 characters")]
        public string message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /*Doesnt work, would have been nice
         public File document { get; set; }
         */

    }
}
