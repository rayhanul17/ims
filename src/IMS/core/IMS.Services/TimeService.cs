using System;

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
