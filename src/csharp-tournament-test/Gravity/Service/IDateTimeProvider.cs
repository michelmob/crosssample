using System;

namespace Gravity.Service
{
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}