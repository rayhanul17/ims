using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Services
{
    public interface IImageService
    {
        Task<string> SaveImage(HttpPostedFileBase file, string path);
        string GetImage(string path, string fileName);
    }

    public class ImageService : IImageService
    {
        public string GetImage(string path, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return path+"NotFound.png";
            
            return "\\UploadedFiles\\" + fileName;
        }

        public async Task<string> SaveImage(HttpPostedFileBase file, string path)
        {
            if (file == null)
                return null;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var extension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + extension;            
            await Task.Run(() => file.SaveAs(path+fileName));

            return fileName;
        }
    }

}
