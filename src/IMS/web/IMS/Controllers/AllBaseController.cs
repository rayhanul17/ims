using IMS.Models;
using log4net;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class AllBaseController : Controller
    {
        protected static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void ViewResponse(string message, ResponseTypes type)
        {
            TempData.Put<ResponseModel>("ResponseMessage", new ResponseModel
            {
                Message = message,
                Type = type
            });
        }
    }
}