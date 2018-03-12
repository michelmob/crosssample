using System;
using System.Runtime.Serialization;

namespace Gravity.Storage
{
    [Serializable]
    public class StorageFileNotFoundException : StorageException
    {
        private const string FileNameKey = "FileName";

        private readonly string _fileName;

        public StorageFileNotFoundException()
        {
            // No-op.
        }

        protected StorageFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _fileName = info.GetString(FileNameKey);
        }

        public StorageFileNotFoundException(string message) : base(message)
        {
            // No-op.
        }

        public StorageFileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            // No-op.
        }

        public StorageFileNotFoundException(string message, string fileName) : this(message, fileName, null)
        {
            // No-op.
        }

        public StorageFileNotFoundException(string message, string fileName, Exception innerException) 
            : base(message, innerException)
        {
            _fileName = fileName;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(FileNameKey, _fileName);
        }
    }
}
