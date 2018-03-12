using System;
using Gravity.Service;

namespace Gravity.Manager.Data.EF.Tests
{
    public class FixedDateTimeProvider : IDateTimeProvider
    {
        public static DateTime DateTime { get; } = new DateTime(2001, 1, 1);
        
        public DateTime Now() => DateTime;
    }
}