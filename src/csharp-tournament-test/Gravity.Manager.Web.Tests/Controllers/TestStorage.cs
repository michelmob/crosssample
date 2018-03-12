using System;
using System.IO;
using System.Threading.Tasks;
using Gravity.Storage;

namespace Gravity.Manager.Web.Tests.Controllers
{
    public class TestStorage : IOutputStorage
    {
        public const string ErrorText = "some error";

        public string Account { get; private set; }
        public string Instance { get; private set; }
        public string File { get; private set; }

        public Task StoreFileAsync(string awsAccountId, string awsInstanceId, string fileName, Stream contents)
        {
            if (fileName.Contains("error"))
            {
                return Task.FromException(new Exception(ErrorText));
            }

            Account = awsAccountId;
            Instance = awsInstanceId;
            File = fileName;

            return Task.CompletedTask;
        }
    }
}
