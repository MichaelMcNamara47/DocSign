using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using SmartSignWebApp.ViewModels;

namespace SmartSignWebApp.Controllers
{
    public class DbController : Controller
    {
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<AdminViewModel>.GetItemsAsync(
                d => true
                );
            ViewBag.Title = "Document List";
            return View(items);
        }
        [ActionName("Create")]
        public IActionResult Create()
        {
            return RedirectToAction("Admin", "App");
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(AdminViewModel item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<AdminViewModel>.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return null;
            }

            AdminViewModel item = await DocumentDBRepository<AdminViewModel>.GetItemAsync(id);
            if (item == null)
            {
                //return HttpNotFound();
                return null;
            }

            return View(item);
        }

    }
}