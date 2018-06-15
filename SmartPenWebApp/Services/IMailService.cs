using SmartSignWebApp.ViewModels;

namespace SmartSignWebApp.Services
{
    public interface IMailService
    {
        void SendMessage(string to, string subject, string body);
        void SendModel(AdminViewModel model);
    }
}