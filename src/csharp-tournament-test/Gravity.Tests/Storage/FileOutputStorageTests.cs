using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Storage;
using Moq;
using NUnit.Framework;

namespace Gravity.Tests.Storage
{
    public class FileOutputStorageTests
    {
        [Test]
        public void Storage_StoreFile_CombinesPath()
        {
            var fileStorageMock = new Mock<IFileStorage>();

            fileStorageMock.Setup(f => f.DirectorySeparatorChar).Returns('-');

            string actualPath = null;

            fileStorageMock
                .Setup(f => f.WriteAllAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask)
                .Callback<string, Stream>((s, _) => actualPath = s);

            var storage = new FileOutputStorage(fileStorageMock.Object);

            storage.StoreFileAsync("acc", "inst", "myFile", new MemoryStream());

            Assert.AreEqual("acc-inst-myFile", actualPath.Split("_").First());
        }

        [Test]
        public void Storage_ValidatesArguments()
        {
            Assert.AreEqual("awsAccountId", AssertThrows<ArgumentNullException>(null, null, null, null).ParamName);
            Assert.AreEqual("awsAccountId", AssertThrows<ArgumentException>("", null, null, null).ParamName);

            Assert.AreEqual("awsInstanceId", AssertThrows<ArgumentNullException>("a", null, null, null).ParamName);
            Assert.AreEqual("awsInstanceId", AssertThrows<ArgumentException>("a", "", null, null).ParamName);

            Assert.AreEqual("fileName", AssertThrows<ArgumentNullException>("a", "a", null, null).ParamName);
            Assert.AreEqual("fileName", AssertThrows<ArgumentException>("a", "a", "", null).ParamName);

            Assert.AreEqual("contents", AssertThrows<ArgumentNullException>("a", "a", "a", null).ParamName);
        }

        private static T AssertThrows<T>(string acc, string inst, string file, Stream content) where T : Exception
        {
            return Assert.Throws<T>(() => GetStorage().StoreFileAsync(acc, inst, file, content).Wait());
        }

        private static FileOutputStorage GetStorage()
        {
            return new FileOutputStorage(new Mock<IFileStorage>().Object);
        }
    }
}
