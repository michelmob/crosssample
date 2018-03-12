using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Gravity.Storage.AmazonS3
{
    public class S3FileStorage : IFileStorage
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketName;

        // ReSharper disable once UnusedMember.Global (used by DI)
        public S3FileStorage(IOptions<S3Options> options) : this(options.Value)
        {
            // No-op.
        }

        public S3FileStorage(S3Options options) : this(GetS3Client(options), options.BucketName)
        {
            // No-op.
        }

        private static IAmazonS3 GetS3Client(S3Options options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.KeyId == null)
            {
                throw new ArgumentNullException(nameof(options.KeyId));
            }

            if (options.SecretKey == null)
            {
                throw new ArgumentNullException(nameof(options.SecretKey));
            }

            if (options.Region == null)
            {
                throw new ArgumentNullException(nameof(options.Region));
            }

            return new AmazonS3Client(options.KeyId, options.SecretKey, RegionEndpoint.GetBySystemName(options.Region));
        }

        public S3FileStorage(IAmazonS3 client, string bucketName)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _bucketName = ValidateName(bucketName);
        }

        public Task<string> ReadAllTextAsync(string name)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = ValidateName(name)
            };

            return WrapExceptions(() => _client.GetObjectAsync(request).ContinueWith(t =>
                new StreamReader(t.Result.ResponseStream, Encoding.UTF8).ReadToEnd()), name);
        }

        public Task WriteAllTextAsync(string name, string text)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text ?? throw new ArgumentNullException(nameof(text))));

            return WriteAllAsync(name, stream);
        }

        public Task WriteAllAsync(string name, Stream contents)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = ValidateName(name),
                InputStream = contents ?? throw new ArgumentNullException(nameof(contents))
            };

            return WrapExceptions(() => _client.PutObjectAsync(request), name);
        }

        public Task DeleteAsync(string name)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = ValidateName(name)
            };

            return WrapExceptions(() => _client.DeleteObjectAsync(request), name);
        }

        public Task<bool> ExistsAsync(string name)
        {
            ValidateName(name);

            return WrapExceptions(() =>
                _client.GetAllObjectKeysAsync(_bucketName, name, null)
                    .ContinueWith(t => t.Result.Contains(name)), _bucketName);
        }

        public Task<IList<string>> GetNames(string prefix)
        {
            // Prefix can be null or empty.
            return WrapExceptions(() => _client.GetAllObjectKeysAsync(_bucketName, prefix, null), prefix);
        }

        public char DirectorySeparatorChar => '\\';

        private static string ValidateName(string name, string argName = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(argName ?? nameof(name));
            }

            return name;
        }

        private static Task<T> WrapExceptions<T>(Func<Task<T>> func, string name)
        {
            // Wrap both sync and async exceptions.
            return Wrap(func, name).ContinueWith(t => Wrap(() => t.Result, name));
        }

        private static T Wrap<T>(Func<T> func, string name)
        {
            try
            {
                return func();
            }
            catch (AmazonServiceException e)
            {
                throw ConvertAmazonException(e, name);
            }
            catch (AggregateException aex)
            {
                aex.Flatten().Handle(e =>
                {
                    if (e is AmazonServiceException amazonEx)
                    {
                        throw ConvertAmazonException(amazonEx, name);
                    }

                    return false;
                });

                throw;
            }
        }

        private static Exception ConvertAmazonException(AmazonServiceException e, string name)
        {
            switch (e.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new StorageFileNotFoundException($"Could not find file '{name}'", e);

                default:
                    return new StorageException(
                        $"Failed to access file '{name}', examine inner exception for details.", e);
            }
        }
    }
}
