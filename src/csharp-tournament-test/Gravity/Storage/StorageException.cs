using System;
using System.Runtime.Serialization;

namespace Gravity.Storage
{
    [Serializable]
    public class StorageException : GravityException
    {
        public StorageException()
        {
            // No-op.
        }

        protected StorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // No-op.
        }

        public StorageException(string message) : base(message)
        {
            // No-op.
        }

        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
            // No-op.
        }
    }
}
