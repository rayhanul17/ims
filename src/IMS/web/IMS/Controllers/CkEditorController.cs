using System.Collections.Generic;
using System.IO;
using System.Web;
using System;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class CkEditorController : Controller
    {
        private readonly string _imagePath = "~/UploadedFiles/";

        [ValidateInput(false)]
        public ActionResult File()
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
                string fileName = string.Empty;
                SaveAttatchedFile(_imagePath, Request, ref fileName);
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

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult FileExplorer()
        {
            var path = Path.Combine(Server.MapPath(_imagePath));

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

        private void SaveAttatchedFile(string filepath, HttpRequestBase Request, ref string fileName)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    string targetPath = Server.MapPath(filepath);
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    fileName = Guid.NewGuid() + fileName;
                    string fileSavePath = Path.Combine(targetPath, fileName);
                    file.SaveAs(fileSavePath);
                }
            }
        }
    }
}
