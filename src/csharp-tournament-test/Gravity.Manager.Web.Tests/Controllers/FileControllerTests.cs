using System.IO;
using System.Linq;
using Gravity.Diagnostics;
using Gravity.Manager.Web.Controllers.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Gravity.Manager.Web.Tests.Controllers
{
    public class FileControllerTests
    {
        [Test]
        public void FileController_Put_StoresFile()
        {
            var storage = new TestStorage();
            var controller = new FileController(storage, new TestLogger());

            var res = controller.Put("acc", "inst", new FormCollection(null, new FormFileCollection
            {
                new FormFile(new MemoryStream(new byte[5]), 0, 5, "_", "foo.bar")
            })).Result;

            var status = ((StatusCodeResult) res).StatusCode;
            Assert.AreEqual(StatusCodes.Status200OK, status);

            Assert.AreEqual("acc", storage.Account);
            Assert.AreEqual("inst", storage.Instance);
            Assert.AreEqual("foo.bar", storage.File);
        }

        [Test]
        public void FileController_PutEmptyFile_ReturnsBadRequestError()
        {
            var form = new FormCollection(null, new FormFileCollection
            {
                new FormFile(new MemoryStream(), 0, 0, "_", "foo.bar")
            });
            
            var res = PutForm(form);
            
            Assert.AreEqual(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.AreEqual("File can not be empty: foo.bar", res.Value);
        }

        [Test]
        public void FileController_PutNoNameFile_ReturnsBadRequestError()
        {
            var form = new FormCollection(null, new FormFileCollection
            {
                new FormFile(new MemoryStream(1), 0, 1, "_", ""),
                new FormFile(new MemoryStream(1), 0, 1, "_", "abc")
            });
            
            var res = PutForm(form);
            
            Assert.AreEqual(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.AreEqual("File name can not be null or whitespace.", res.Value);
        }

        [Test]
        public void FileController_PutNoFiles_ReturnsBadRequestError()
        {
            var form = new FormCollection(null, new FormFileCollection());
            
            var res = PutForm(form);
            
            Assert.AreEqual(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.AreEqual("Request contains no files to upload.", res.Value);
        }

        [Test]
        public void FileController_StorageThrowsErrorForSomeFiles_ReturnsErrorWithSummaryAndLogs()
        {
            var form = new FormCollection(null, new FormFileCollection
            {
                new FormFile(new MemoryStream(1), 0, 1, "_", "foo"),
                new FormFile(new MemoryStream(1), 0, 1, "_", "error1"),
                new FormFile(new MemoryStream(1), 0, 1, "_", "bar"),
                new FormFile(new MemoryStream(1), 0, 1, "_", "error2"),
            });

            var log = new TestLogger();
            
            var res = PutForm(form, log);
            
            // Check result.
            Assert.AreEqual(StatusCodes.Status500InternalServerError, res.StatusCode);
            Assert.AreEqual("Failed to upload files: error1, error2", res.Value);

            // Check log.
            var logs = log.Entries;
            Assert.AreEqual(6, log.Entries.Count);
            
            Assert.AreEqual("Uploading file from AWS account 'acc', instance 'inst': foo", logs[0].Message);
            Assert.AreEqual(LogLevel.Info, logs[0].Level);

            var errLogs = logs.Skip(4).ToArray();

            Assert.AreEqual("Failed to store file: error1", errLogs[0].Message);
            Assert.AreEqual("Failed to store file: error2", errLogs[1].Message);

            Assert.AreEqual(LogLevel.Error, errLogs.Select(x => x.Level).Distinct().Single());
            Assert.AreEqual(TestStorage.ErrorText, errLogs.Select(x => x.Exception.InnerException.Message).Distinct().Single());
            Assert.AreEqual(typeof(FileController).FullName, errLogs.Select(x => x.Category).Distinct().Single());
        }

        private static ObjectResult PutForm(IFormCollection form, ILogger logger = null)
        {
            var controller = new FileController(new TestStorage(), logger ?? new TestLogger());

            var res = controller.Put("acc", "inst", form).Result;

            return (ObjectResult) res;
        }
    }
}
