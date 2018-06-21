using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SmartSignWebApp.Controllers
{
    public class PenController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private PenConnector.PenConnector _penConnector;


        public PenController(IHostingEnvironment environment)
        {
            this._hostingEnvironment = environment;
            _penConnector = new PenConnector.PenConnector(_hostingEnvironment);
            _penConnector.ClearImage();
        }
        public IActionResult ConnectPen()
        {
            _penConnector.connectPen();

            ViewBag.Title = "Connecting...";
            return RedirectToAction("Client","App");
        }

        public IActionResult DisconnectPen() {

            if (_penConnector != null) {

                _penConnector.disconnected();

            }

            ViewBag.Title = "Disconnecting...";
            return RedirectToAction("Client", "App");
        }

        public IActionResult ClearImage()
        {

            if (_penConnector != null)
            {
                _penConnector.ClearImage();
            }

            ViewBag.Title = "Clearing Image...";
            return RedirectToAction("Client", "App");
        }
        
    }
}
