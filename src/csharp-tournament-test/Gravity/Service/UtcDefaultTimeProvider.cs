using System;

namespace Gravity.Service
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}