using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public ResponseTypes Type { get; set; }
    }
    public enum ResponseTypes
    {
        Success,
        Danger
    }
}