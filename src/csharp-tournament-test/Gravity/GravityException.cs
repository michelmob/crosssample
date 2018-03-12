using System;
using System.Runtime.Serialization;

namespace Gravity
{
    [Serializable]
    public class GravityException : Exception
    {
        public GravityException()
        {
            // No-op.
        }

        protected GravityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // No-op.
        }

        public GravityException(string message) : base(message)
        {
            // No-op.
        }

        public GravityException(string message, Exception innerException) : base(message, innerException)
        {
            // No-op.
        }
    }
}
