using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SmartSignWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSignWebApp.Services
{
    /*Once the null mail service class was setup, the IMailService interface was extracted*/
    public class NullMailService : IMailService
    {
        private readonly ILogger<NullMailService> logger;

        public NullMailService(ILogger<NullMailService> logger)
        {
            this.logger = logger;
        }

        public void DocumentSigned(AdminViewModel model, IHostingEnvironment hostingEnvironment)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string to, string subject, string body)
        {
            //Log the message for dummy mail service
            logger.LogInformation($"To: {to} Subject: {subject} Body: {body}");
        }

        public void SendModel(AdminViewModel model)
        {
            throw new NotImplementedException();
        }

        public void SendNewDocument(AdminViewModel model, IHostingEnvironment hostingEnvironment)
        {
            throw new NotImplementedException();
        }
    }
}
