using System;

namespace IMS.Infrastructure.Utility
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }
}