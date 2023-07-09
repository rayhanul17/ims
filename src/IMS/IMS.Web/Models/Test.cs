using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models
{
    public class Test : ITest
    {
        public void Print(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}