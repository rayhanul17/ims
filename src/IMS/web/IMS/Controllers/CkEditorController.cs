using IMS.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class CkEditorController : Controller
    {
        protected readonly IImageService _imageService;
        private readonly string _imagePath = "/UploadedFiles";

        public CkEditorController()
        {
            _imageService = new ImageService();
        }
        public async Task<ActionResult> File()
        {
            var funcNum = 0;
            int.TryParse(Request["CKEditorFuncNum"], out funcNum);

            if (Request.Files == null || Request.Files.Count < 1)
            {
                var response = new
                {
                    uploaded = 0,
                    error = new { message = "File not saved" }
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string fileName = await SaveAttatchedFile(_imagePath, Request);
                var url = _imagePath + "/" + fileName;

                var response = new
                {
                    uploaded = Request.Files.Count,
                    fileName = fileName,
                    url = url
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FileExplorer()
        {
            var path = Path.Combine(Server.MapPath("\\UploadedFiles\\"));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path);

            var fileNames = new List<string>();

            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }

            ViewBag.FileInfo = fileNames;
            ViewBag.Path = _imagePath;

            return View();
        }

        private async Task<string> SaveAttatchedFile(string filepath, HttpRequestBase Request)
        {
            string fileName = string.Empty;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var image = Request.Files[i];
                if (image != null && image.ContentLength > 0)
                {
                    string path = Server.MapPath("~/UploadedFiles/");

                    fileName = await _imageService.SaveImage(image, path);
                }
            }
            return fileName;
        }
    }
}
