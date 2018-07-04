using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace SmartSignWebApp.PDF
{
    public class PDFCombine
    {
        public static PdfDocument CombinePdfPng(string pdfIn, string id, IHostingEnvironment _hostingEnvironment)
        {

            /* Get paths of documents to combine */
            string nCodedPdf = pdfIn;
            //string templateDocument = "NDAExample.pdf";
            // todo: string templateDocument = pdfIn;
            string pngImage = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img/pen/"+id+".png");
            // Create the output document
            
            PdfDocument outputDocument = new PdfDocument();

            // Show consecutive pages facing
            //outputDocument.PageLayout = PdfPageLayout.TwoPageLeft;
            
            XGraphics gfx;
            //XRect box;

            // Open the external documents as XPdfForm objects. Such objects are
            // treated like images. By default the first page of the document is
            // referenced by a new XPdfForm.
            using (var client = new WebClient())
            {
                client.DownloadFile(pdfIn, System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/input.pdf"));
            }


            //switched the order - ncode must be written first, document written on top

            //XPdfForm formNcode = XPdfForm.FromFile(fs);//ncode
            XPdfForm formNcode = XPdfForm.FromFile("input.pdf");//ncode

            //XPdfForm formPngImage = XPdfForm.FromFile(pngImage);
            Console.WriteLine("formNcode.PixelHeight: " + formNcode.PixelHeight);
            Console.WriteLine("formNcode.PixelWidth: " + formNcode.PixelWidth);
            

            
            //int count = formTemplate.PageCount;
            
            gfx = XGraphics.FromPdfPage(outputDocument.AddPage());
            gfx.DrawImage(formNcode, new XRect(0, 0, 595, 842));
            using (XImage sig = XImage.FromFile(pngImage)) { 
                gfx.DrawImage(sig, new XRect(0, 0, 595, 842));
            }
            //gfx = XGraphics.FromPdfPage(page1);
            gfx.Dispose();




            outputDocument.Save(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/ImageCombine.pdf"));
            outputDocument.Dispose();
            return outputDocument;
        }
        public static PdfDocument CombinePdfPng(string pdfIn, IHostingEnvironment _hostingEnvironment)
        {

            /* Get paths of documents to combine */
            string nCodedPdf = pdfIn;
            //string templateDocument = "NDAExample.pdf";
            // todo: string templateDocument = pdfIn;
            string pngImage = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img/pen/web.png");
            // Create the output document

            PdfDocument outputDocument = new PdfDocument();

            // Show consecutive pages facing
            //outputDocument.PageLayout = PdfPageLayout.TwoPageLeft;

            XGraphics gfx;
            //XRect box;

            // Open the external documents as XPdfForm objects. Such objects are
            // treated like images. By default the first page of the document is
            // referenced by a new XPdfForm.
            using (var client = new WebClient())
            {
                client.DownloadFile(pdfIn, System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/input.pdf"));
            }


            //switched the order - ncode must be written first, document written on top

            //XPdfForm formNcode = XPdfForm.FromFile(fs);//ncode
            XPdfForm formNcode = XPdfForm.FromFile("input.pdf");//ncode

            //XPdfForm formPngImage = XPdfForm.FromFile(pngImage);
            Console.WriteLine("formNcode.PixelHeight: " + formNcode.PixelHeight);
            Console.WriteLine("formNcode.PixelWidth: " + formNcode.PixelWidth);



            //int count = formTemplate.PageCount;

            gfx = XGraphics.FromPdfPage(outputDocument.AddPage());
            gfx.DrawImage(formNcode, new XRect(0, 0, 595, 842));
            //gfx.DrawImage(XImage.FromGdiPlusImage(sigBitmap), new XRect(0, 0, 595, 842));
            gfx.DrawImage(XImage.FromFile(pngImage), new XRect(0, 0, 595, 842));
            //gfx = XGraphics.FromPdfPage(page1);
            gfx.Dispose();




            outputDocument.Save(System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/ImageCombine.pdf"));
            outputDocument.Dispose();
            return outputDocument;
        }


        public static string CombinePdfPdf(Stream pdfIn, IHostingEnvironment _hostingEnvironment)
        {

            /* Get paths of documents to combine */
            string nCodePaper = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/Neosmartpen_A4_A_Type_plain_en.pdf");
            //string templateDocument = "NDAExample.pdf";
            // todo: string templateDocument = pdfIn;
            XPdfForm formTemplate = XPdfForm.FromStream(pdfIn);
            // Create the output document

            PdfDocument outputDocument = new PdfDocument();

            // Show consecutive pages facing
            //outputDocument.PageLayout = PdfPageLayout.TwoPageLeft;

            XGraphics gfx;
            //XRect box;

            // Open the external documents as XPdfForm objects. Such objects are
            // treated like images. By default the first page of the document is
            // referenced by a new XPdfForm.

            //switched the order - ncode must be written first, document written on top
            XPdfForm formNcode = XPdfForm.FromFile(nCodePaper);//ncode
            
            Console.WriteLine("formNcode.PixelHeight: " + formNcode.PixelHeight);
            Console.WriteLine("formNcode.PixelWidth: " + formNcode.PixelWidth);

            int count = formTemplate.PageCount;

            for (int idx = 0; idx < count; idx++)
            {
                // Add a new page to the output document
                PdfPage page1 = outputDocument.AddPage();

                // Get a graphics object for page1
                gfx = XGraphics.FromPdfPage(page1);

                // draw the ncode page to the output document
                if (formNcode.PageCount > idx)
                {

                    // Set page number (which is one-based) the first useable page from the ncode paper is page2
                    formNcode.PageNumber = idx + 2;

                    // Draw the page identified by the page number like an image
                    gfx.DrawImage(formNcode, new XRect(0, 0, 595, 842));

                }

                // Same as above for second page
                if (formTemplate.PageCount > idx)
                {

                    formTemplate.PageNumber = idx + 1;
                    gfx.DrawImage(formTemplate, new XRect(0, 0, 595, 842));

                }

            }
            // Save the document...
            string savePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "pdf/outputFile.pdf");
            outputDocument.Save(savePath);
            outputDocument.Dispose();


            return savePath;
        }


    }

}

