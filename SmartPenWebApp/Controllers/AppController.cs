using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSignWebApp.Services;
using SmartSignWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSignWebApp.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;

        //hosting env for paths
        private IHostingEnvironment _hostingEnvironment;

        /* Use a constructor to inject the services needed
         * */

        public AppController(IMailService mailService, IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _mailService = mailService;
        }



        /* Actions: These actions are executed when the url path matches
         * The view returned is based on the cshtml page with the same name
         * as the action, in the views folder.
         */
        public IActionResult Index() {
            ViewBag.Title = "Home";
            return View();
        }

        public IActionResult Admin()
        {
            ViewBag.Title = "Admin";
            return View();
        }

        [HttpPost]
        public IActionResult Admin(AdminViewModel model)
        {
            ViewBag.Title = "Admin";
            /*Validation:
             * ModelState - Uses data annotations to validate the model
             */
            if (ModelState.IsValid) {
                //Create record              
                _mailService.SendModel(model);
                ViewBag.UserMessage = "Document sent";
                ModelState.Clear();
            }



            return View();
        }

        public IActionResult Client()
        {
            ViewBag.Title = "Client";
            return View();
        }

        public IActionResult Search()
        {
            ViewBag.Title = "Search";
            return View();
        }

        public IActionResult ConnectPen()
        {
            PenConnector.PenConnector penConnector = new PenConnector.PenConnector(_hostingEnvironment);
            penConnector.connectPen();
           
            ViewBag.Title = "Connecting...";
            return RedirectToAction("Client");
        }

        //Start of file upload methods
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/uploads",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Admin");
        }

        /*public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }*/

        //End of File upload methods
    }
}
