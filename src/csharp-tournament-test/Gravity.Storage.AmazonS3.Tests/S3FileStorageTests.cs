using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using NUnit.Framework;

namespace Gravity.Storage.AmazonS3.Tests
{
    public class S3FileStorageTests
    {
        [Test]
        public void Storage_MissingFile_ThrowsStorageFileNotFoundException()
        {
            var clientMock = new Mock<IAmazonS3>();
            clientMock
                .Setup(c => c.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<GetObjectResponse>(
                    new AmazonS3Exception("bar", ErrorType.Sender, "NoSuchKey", "1", HttpStatusCode.NotFound)));

            var storage = new S3FileStorage(clientMock.Object, "bucket1");

            var ex = Assert.ThrowsAsync<StorageFileNotFoundException>(
                async () => await storage.ReadAllTextAsync("foo"));

            Assert.IsInstanceOf<AmazonS3Exception>(ex.InnerException);
            Assert.AreEqual("bar", ex.InnerException.Message);
        }

        [Test]
        public async Task Storage_ReturnsStoredFile()
        {
            var clientMock = new Mock<IAmazonS3>();
            Stream stream = null;

            clientMock
                .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .Callback<PutObjectRequest, CancellationToken>((req, _) => stream = req.InputStream)
                .Returns(Task.FromResult(new PutObjectResponse()));

            clientMock
                .Setup(c => c.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new GetObjectResponse
                {
                    ResponseStream = stream
                }));

            var storage = new S3FileStorage(clientMock.Object, "bucket1");

            const string text = "fooBar\nbaz";
            const string fileName = "myFile";

            await storage.WriteAllTextAsync(fileName, text);
            var actualText = await storage.ReadAllTextAsync(fileName);

            Assert.AreEqual(text, actualText);
        }

        [Test]
        public async Task Storage_Delete_DelegatesToS3()
        {
            var clientMock = new Mock<IAmazonS3>();
            string actualFileName = null;
            string actualBucket = null;

            clientMock
                .Setup(c => c.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
                .Callback<DeleteObjectRequest, CancellationToken>((req, _) =>
                {
                    actualFileName = req.Key;
                    actualBucket = req.BucketName;
                })
                .Returns(Task.FromResult(new DeleteObjectResponse()));

            const string bucket = "fooBucket";
            const string fileName = "myFile";
            
            var storage = new S3FileStorage(clientMock.Object, bucket);
            await storage.DeleteAsync(fileName);

            Assert.AreEqual(bucket, actualBucket);
            Assert.AreEqual(fileName, actualFileName);
        }

        [Test]
        public async Task Storage_Exists_DelegatesToGetKeys()
        {
            var clientMock = new Mock<IAmazonS3>();
            
            string actualPrefix = null;
            string actualBucket = null;

            const string bucket = "fooBucket";
            const string fileName = "myFile";

            clientMock
                .Setup(c => c.GetAllObjectKeysAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>()))
                .Callback<string, string, IDictionary<string, object>>((buck, pref, _) =>
                {
                    actualBucket = buck;
                    actualPrefix = pref;
                })
                .Returns(Task.FromResult<IList<string>>(new[] {fileName}));

            var storage = new S3FileStorage(clientMock.Object, bucket);

            Assert.True(await storage.ExistsAsync(fileName));
            Assert.AreEqual(bucket, actualBucket);
            Assert.AreEqual(fileName, actualPrefix);
        }

        [Test]
        public async Task Storage_GetNames_DelegatesToGetKeys()
        {
            var clientMock = new Mock<IAmazonS3>();
            
            string actualPrefix = null;
            string actualBucket = null;

            const string bucket = "fooBucket";
            const string fileName = "myFile";

            clientMock
                .Setup(c => c.GetAllObjectKeysAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>()))
                .Callback<string, string, IDictionary<string, object>>((buck, pref, _) =>
                {
                    actualBucket = buck;
                    actualPrefix = pref;
                })
                .Returns(Task.FromResult<IList<string>>(new[] {fileName}));

            var storage = new S3FileStorage(clientMock.Object, bucket);

            var actualNames = await storage.GetNames(fileName.Substring(1));
            Assert.AreEqual(fileName, actualNames.Single());
            Assert.AreEqual(bucket, actualBucket);
            Assert.AreEqual(fileName.Substring(1), actualPrefix);
        }
    }
}
