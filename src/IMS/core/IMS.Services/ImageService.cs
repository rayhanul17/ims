using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Services
{
    #region Interface
    public interface IImageService
    {
        Task<string> SaveImage(HttpPostedFileBase file, string path);
        string GetImage(string path, string fileName);
    }
    #endregion
    public class ImageService : IImageService
    {
        #region Operational Function
        public string GetImage(string path, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return path + "NotFound.png";

            return path + fileName;
        }

        public async Task<string> SaveImage(HttpPostedFileBase file, string path)
        {
            if (file == null)
                return null;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var extension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + extension;
            await Task.Run(() => file.SaveAs(path + fileName));

            return fileName;
        }

        #endregion
    }
}
