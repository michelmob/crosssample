using System;
using System.IO;
using System.Threading.Tasks;

namespace Gravity.Storage
{
    public class FileOutputStorage : IOutputStorage
    {
        private readonly IFileStorage _fileStorage;

        public FileOutputStorage(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        public Task StoreFileAsync(string awsAccountId, string awsInstanceId, string fileName, Stream contents)
        {
            Validate(awsAccountId, nameof(awsAccountId));
            Validate(awsInstanceId, nameof(awsInstanceId));
            Validate(fileName, nameof(fileName));

            contents = contents ?? throw new ArgumentNullException(nameof(contents));
            
            var sep = _fileStorage.DirectorySeparatorChar;
            
            // Add ISO 8601 timestamp.
            var timeStamp = DateTime.UtcNow.ToString("o");
            
            var fullPath = $"{awsAccountId}{sep}{awsInstanceId}{sep}{fileName}_{timeStamp}";

            return _fileStorage.WriteAllAsync(fullPath, contents);
        }

        private static void Validate(string value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{name}' can not be empty or whitespace.", name);
            }
        }
    }
}