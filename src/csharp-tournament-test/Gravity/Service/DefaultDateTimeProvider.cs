using System;

namespace Gravity.Service
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}