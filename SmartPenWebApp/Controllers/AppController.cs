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

using Neosmartpen.Net.Protocol.v1;
using SmartSignWebApp.PenConnector;



namespace SmartSignWebApp.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        //public static PenConnector.PenConnector _penConnector { get; set; }
        private readonly PenConnector.PenConnector _penConnector;

        //hosting env for paths
        private IHostingEnvironment _hostingEnvironment;

        /* Use a constructor to inject the services needed
         * */
        public static int numInstances = 0;

        public AppController(IMailService mailService, IHostingEnvironment environment, PenCommV1Callbacks penConnector)
        {
            _penConnector = penConnector as PenConnector.PenConnector;
            _hostingEnvironment = environment;
            _mailService = mailService;
            //_penConnector = new PenConnector.PenConnector(_hostingEnvironment);

            System.Console.WriteLine("************Number of instances APPC " + (++numInstances));


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
            //_penConnector = new PenConnector.PenConnector(_hostingEnvironment);
            _penConnector.connectPen();
            _penConnector.ClearImage();

            ViewBag.Title = "Connecting...";
            return RedirectToAction("Client");
            //return new EmptyResult();
        }
        public IActionResult DrawSignature()
        {
            _penConnector.DrawSignature();
            
            ViewBag.Title = "Drawing Signature...";
            return RedirectToAction("Client");
        }
        public IActionResult ClearImage()
        {

            if (_penConnector != null)
            {
                _penConnector.ClearSignature();
                _penConnector.ClearImage();
            }

            ViewBag.Title = "Clearing Image...";
            return RedirectToAction("Client");
        }
        //Start of file upload methods
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "No File Selected";
                return View("Admin");//RedirectToAction("Admin");
            }
            string pdfContentType = "application/pdf";
            if (file.ContentType != pdfContentType)
            {
                ViewBag.Message = "Wrong format, PDF only";
                return View("Admin");//RedirectToAction("Admin");
            }
            else {
                ViewBag.Message = "Upload Accepted";
            }

            string fileName = Path.GetFileName(file.FileName);
            System.Console.WriteLine(fileName);
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/uploads/"+fileName
                       );

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return View("Admin");//RedirectToAction("Admin");
        }



    }
}
