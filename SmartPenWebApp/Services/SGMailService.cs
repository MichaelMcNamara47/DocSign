using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartSignWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSignWebApp.Services
{
    public class SGMailService : IMailService

    {
        private readonly ILogger<SGMailService> logger;

        public SGMailService(ILogger<SGMailService> logger)
        {
            this.logger = logger;
        }

        public void SendModel(AdminViewModel model)
        {
            logger.LogInformation("Trying to send mail with sendgrid...");
            Execute(model, logger).Wait();
            logger.LogInformation("... Finished Trying");
        }

        static async Task Execute(AdminViewModel model, ILogger<SGMailService> logger)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(model.email, $"{model.fName} {model.lName}");
            var subject = $"Sending To:{model.email} from Sendgrid";
            var to = new EmailAddress("michael.mcnamara@storm.ie", "Signatory");
            var plainTextContent = model.message;
            //var htmlContent = $"<strong>{model.message}</strong>";
            var htmlContent = $"" +
                $"<p>Hello {model.fName}, </p>" +
                $"<p>You are required to sign <a href={"http://localhost:8888/app/Client/?"}{model.Id} >{model.DocName}" +
                $"</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Status Code: "+response.StatusCode.ToString());
            //"Your document link is: http://localhost:8888/app/Client/?" + model.Id;

        }

        public void SendMessage(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
