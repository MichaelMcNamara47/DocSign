using Microsoft.AspNetCore.Mvc;
using SmartSignWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSignWebApp.Controllers
{
    public class AppController : Controller
    {
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
                //Send email
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
    }
}
