using Microsoft.AspNetCore.Hosting;
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
            //logger.LogInformation("Trying to send mail with sendgrid...");
            //Execute(model, logger).Wait();
            //logger.LogInformation("... Finished Trying");
        }

        public void SendNewDocument(AdminViewModel model, IHostingEnvironment hostingEnvironment)
        {
            logger.LogInformation("Trying to send mail to signatory");
            SendSignatoryEmail(model, hostingEnvironment, logger).Wait();
            logger.LogInformation("Email sent to signatory");
            SendRequesterEmail(model, hostingEnvironment, logger).Wait();
            logger.LogInformation("Email sent to requester");
        }

        public void DocumentSigned(AdminViewModel model, IHostingEnvironment hostingEnvironment)
        {
            logger.LogInformation("Trying to send mail to signatory");
            SendRequesterEmailSigned(model, hostingEnvironment, logger).Wait();
            logger.LogInformation("Email sent to requester");
        }

        static async Task SendRequesterEmailSigned(AdminViewModel model, IHostingEnvironment hostingEnvironment, ILogger<SGMailService> logger)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("no-reply@DocSign", $"Doc Sign");
            var subject = $"Document signed by: {model.lName},{model.fName}";
            var to = new EmailAddress(model.notifyEmail, "Signatory");
            var plainTextContent = model.message;
            //var htmlContent = $"<strong>{model.message}</strong>";
            var htmlContent = $"" +
                $"<p>Document signed by: {model.lName},{model.fName}, </p>" +
                $"<p>View the status of the document <a href={"http://localhost:8888/app/Search/"}{model.Id} >here</a>" +
                $"</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Status Code: " + response.StatusCode.ToString());
            //"Your document link is: http://localhost:8888/app/Client/?" + model.Id;

        }


        static async Task SendSignatoryEmail(AdminViewModel model, IHostingEnvironment hostingEnvironment, ILogger<SGMailService> logger)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(model.notifyEmail, $"{model.notifyEmail}");
            var subject = $"Please Sign: {model.DocName}";
            var to = new EmailAddress(model.signatoryEmail, "Signatory");
            var plainTextContent = model.message;
            //var htmlContent = $"<strong>{model.message}</strong>";
            var htmlContent = $"" +
                $"<p>Hello {model.fName}, </p>" +
                $"<p>You are required to sign <a href={"http://localhost:8888/app/Client/"}{model.Id} >{model.DocName}</a>" +
                $"</p><p>{model.message}</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Status Code: " + response.StatusCode.ToString());
            //"Your document link is: http://localhost:8888/app/Client/?" + model.Id;

        }
        static async Task SendRequesterEmail(AdminViewModel model, IHostingEnvironment hostingEnvironment, ILogger<SGMailService> logger)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("no-reply@DocSign", $"Doc Sign");
            var subject = $"Request Created for: {model.lName},{model.fName}";
            var to = new EmailAddress(model.notifyEmail, "Signatory");
            var plainTextContent = model.message;
            //var htmlContent = $"<strong>{model.message}</strong>";
            var htmlContent = $"" +
                $"<p>Document request created for:{model.lName},{model.fName}, </p>" +
                $"<p>View the status of the document <a href={"http://localhost:8888/app/Search/"}{model.Id} >here</a>" +
                $"</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Status Code: " + response.StatusCode.ToString());
            //"Your document link is: http://localhost:8888/app/Client/?" + model.Id;

        }

        //static async Task Execute(AdminViewModel model, ILogger<SGMailService> logger)
        //{
        //    var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(model.notifyEmail, $"{model.fName} {model.lName}");
        //    var subject = $"Sending To:{model.signatoryEmail} from Sendgrid";
        //    var to = new EmailAddress(model.signatoryEmail, "Signatory");
        //    var plainTextContent = model.message;
        //    //var htmlContent = $"<strong>{model.message}</strong>";
        //    var htmlContent = $"" +
        //        $"<p>Hello {model.fName}, </p>" +
        //        $"<p>You are required to sign <a href={"http://localhost:8888/app/Client/?"}{model.Id} >{model.DocName}" +
        //        $"</p>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await client.SendEmailAsync(msg);
        //    logger.LogInformation("Status Code: "+response.StatusCode.ToString());
        //    //"Your document link is: http://localhost:8888/app/Client/?" + model.Id;

        //}

        public void SendMessage(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }

    }
}
