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
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Net;
using Newtonsoft.Json;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SmartSignWebApp.PDF;
using System.Drawing;

namespace SmartSignWebApp.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        ////public static PenConnector.PenConnector _penConnector { get; set; }
        private readonly PenConnector.PenConnector _penConnector;
        private readonly DocumentClient _client;

        private readonly string DatabaseName = Environment.GetEnvironmentVariable("DATABASEID");
        private readonly string CollectionName = Environment.GetEnvironmentVariable("COLLECTIONID");
        //private AdminViewModel model { get; set; }

        //hosting env for paths
        private IHostingEnvironment _hostingEnvironment;

        /* Use a constructor to inject the services needed
         * */
        public AppController(IMailService mailService, IHostingEnvironment environment, PenCommV1Callbacks penConnector, DocumentClient client)
        {
            _client = client;
            _penConnector = penConnector as PenConnector.PenConnector;
            _penConnector.ClearImage();
            _hostingEnvironment = environment;
            _mailService = mailService;
            //_penConnector = new PenConnector.PenConnector(_hostingEnvironment);


        }



        /* Actions: These actions are executed when the url path matches
         * The view returned is based on the cshtml page with the same name
         * as the action, in the views folder.
         */
        public IActionResult Index() {
            CloudBlobContainer container = GetCloudBlobContainer();
            ViewBag.Success = container.CreateIfNotExistsAsync().Result;
            System.Console.WriteLine(ViewBag.Success);
            ViewBag.Title = "Home ";
            return View();
        }

        public IActionResult Admin()
        {
            ViewBag.Title = "Request Signature";
            ViewBag.UploadStatus = "Upload";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminAsync(AdminViewModel model)
        {
            ViewBag.Title = "Request Signature";
            /*Validation:
             * ModelState - Uses data annotations to validate the model
             */
            if (ModelState.IsValid) {
                //Create record  
                model.isSigned = false;
                _mailService.SendNewDocument(
                        await DocumentDBRepository<AdminViewModel>.CreateDocumentIfNotExists(model), 
                        _hostingEnvironment);
                ;
                //await CreateDocumentIfNotExists(DatabaseName, CollectionName, model);
                //_mailService.SendNewDocument(model, _hostingEnvironment );
                ViewBag.UserMessage = "Document sent";
                ModelState.Clear();
            }
            ViewBag.UploadStatus = "Upload";
            return RedirectToAction("Index", "Db");
        }

        public async Task<IActionResult> Client(string id)
        {
            AdminViewModel model = await DocumentDBRepository<AdminViewModel>.GetItemAsync(id);
            ViewBag.DocName = model.DocName;
            ViewBag.Id = model.Id;
            if (!model.isSigned)
            {
                ViewBag.Title = model.fName+ " " +model.lName+" you are required to sign: " + model.DocName;
                ViewBag.pdfURL = GetBlobSasUri(model.DocGuid);
                return View();
            }
            else
            {
                ViewBag.Title = "Welcome back " + model.fName + ", you have signed " + model.DocName;
                ViewBag.pdfURL = GetBlobSasUri(model.SignedDocGuid);
                return View("DrawSignature");
            }
        }


        public async Task<IActionResult> DrawSignature()
        {

            var id = Request.Path.ToString().Substring(Request.Path.ToString().LastIndexOf('/') + 1);
            AdminViewModel model = await DocumentDBRepository<AdminViewModel>.GetItemAsync(id);

            // _penConnector.DrawSignature();           
            Bitmap tempBitmap = new Bitmap(800, 1131);
            Graphics g = Graphics.FromImage(tempBitmap);
            g.Clear(Color.Transparent);
            g.Dispose();
            //
            _penConnector.DrawSignature(model.Id, tempBitmap);


            PDFCombine.CombinePdfPng(GetBlobSasUri(model.DocGuid), model.Id, _hostingEnvironment);

            if (System.IO.File.Exists(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img/pen/"+model.Id+".png"))) {
                System.IO.File.Delete(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img/pen/" + model.Id + ".png"));
            }

            string uploadName = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/ImageCombine.pdf");
            FileStream fs = new FileStream(uploadName, FileMode.Open, FileAccess.Read);
            model.isSigned = true;
            model.SignedDocGuid = UploadBlob(fs, model.DocName);
            await DocumentDBRepository<AdminViewModel>.UpdateItemAsync(model.Id, model);
            ViewBag.Title = "Thank you for signing";
            ViewBag.Id = model.Id;
            ViewBag.pdfURL = GetBlobSasUri(model.SignedDocGuid);
            _mailService.DocumentSigned(model, _hostingEnvironment);
            return View();
        }

        public async Task<IActionResult> ConnectPen()
        {           
            _penConnector.connectPen();
            _penConnector.ClearImage();

            ViewBag.Title = "Connecting...";
            var id = Request.Path.ToString().Substring(Request.Path.ToString().LastIndexOf('/') + 1);
            AdminViewModel model = await DocumentDBRepository<AdminViewModel>.GetItemAsync(id);

            ViewBag.pdfURL = GetBlobSasUri(model.DocGuid);
            ViewBag.DocName = model.DocName;
            ViewBag.Id = model.Id;
            return View("Client");
            //return new EmptyResult();
        }


        public async Task<IActionResult> ClearImage()
        {

            if (_penConnector != null)
            {
                _penConnector.ClearSignature();
                _penConnector.ClearImage();
            }

            var id = Request.Path.ToString().Substring(Request.Path.ToString().LastIndexOf('/') + 1);
            AdminViewModel model = await DocumentDBRepository<AdminViewModel>.GetItemAsync(id);

            ViewBag.pdfURL = GetBlobSasUri(model.DocGuid);
            ViewBag.Id = model.Id;
            ViewBag.DocName = model.DocName;

            return View("Client");
            
        }


        //Start of file upload methods
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.UploadStatus = "Upload";
                ViewBag.Message = "No File Selected";
                return View("Admin");//RedirectToAction("Admin");
            }
            string pdfContentType = "application/pdf";
            if (file.ContentType != pdfContentType)
            {
                ViewBag.UploadStatus = "Upload";
                ViewBag.Message = "Wrong format, PDF only";
                return View("Admin");//RedirectToAction("Admin");
            }
            else {
                ViewBag.UploadStatus = "UploadSuccess";
                ViewBag.Message = "Upload Accepted";

               

                /*
                string fileName = Path.GetFileName(file.FileName);
                ViewBag.documentName = fileName;
                var blobName = UploadBlob(file);
                */

                string docName = Path.GetFileName(file.FileName);
                ViewBag.docName = docName;

                ViewBag.docGuid = UploadBlob(file, docName);

                //ViewBag.docGuid = UploadBlob(file, docName);
                Console.WriteLine("File name: " + ViewBag.docName);
                Console.WriteLine("Document ID: " + ViewBag.docGuid);
                /*
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/uploads/"+fileName
                           );

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                */

                return View("Admin");//RedirectToAction("Admin");
            }
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            //Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING"));
            //CloudConfigurationManager.GetSetting("<storageaccountname>_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pdf-container");
            return container;
        }

        public string UploadBlob(FileStream fs, string fileName)
        {
            var blobName = Guid.NewGuid().ToString() + fileName;
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            //string filePath = PDFCombine.CombinePdfPdf(fs, _hostingEnvironment);

            blob.UploadFromStreamAsync(fs).Wait();

            return blobName;
        }

        public string UploadBlob(IFormFile file, string fileName)
        {
            var blobName = Guid.NewGuid().ToString() + fileName;
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            //using (var fileStream = System.IO.File.OpenRead(@"<file-to-upload>"))
            using (var fileStream = file.OpenReadStream())
            {
                string filePath = PDFCombine.CombinePdfPdf(fileStream, _hostingEnvironment);
                
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    blob.UploadFromStreamAsync(fs).Wait();
                }
            }
            return blobName;
        }

        public string GetBlobSasUri(string blobname)
        {
            try
            {
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings.Get("StorageConnectionString"));

                //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = GetCloudBlobContainer();
                    // blobClient.GetContainerReference(containername);
                //Get a reference to a blob within the container.
                CloudBlockBlob blob = container.GetBlockBlobReference(blobname);

                //Set the expiry time and permissions for the blob.
                //In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
                //The shared access signature will be valid immediately.
                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

                //Generate the shared access signature on the blob, setting the constraints directly on the signature.
                string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

                //Return the URI string for the container, including the SAS token.
                return blob.Uri + sasBlobToken;
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message, e);
                throw;

            }

        }

    }
}
