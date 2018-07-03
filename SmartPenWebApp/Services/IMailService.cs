using Microsoft.AspNetCore.Hosting;
using SmartSignWebApp.ViewModels;

namespace SmartSignWebApp.Services
{
    public interface IMailService
    {
        void SendMessage(string to, string subject, string body);
        void SendModel(AdminViewModel model);
        void SendNewDocument(AdminViewModel model, IHostingEnvironment hostingEnvironment);
        void DocumentSigned(AdminViewModel model, IHostingEnvironment hostingEnvironment);
    }
}