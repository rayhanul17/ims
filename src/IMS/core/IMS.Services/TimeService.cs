using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }
    public class TimeService : ITimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
