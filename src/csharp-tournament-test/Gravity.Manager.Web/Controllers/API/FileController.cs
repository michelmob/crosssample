using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Diagnostics;
using Gravity.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers.API
{
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly IOutputStorage _storage;
        private readonly ILogger _logger;

        public FileController(IOutputStorage storage, ILogger logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = (logger ?? throw new ArgumentNullException(nameof(logger))).GetLogger(GetType());
        }

        [HttpPut("{awsAccountId}/{awsInstanceId}")]
        public async Task<IActionResult> Put(string awsAccountId, string awsInstanceId, IFormCollection form)
        {
            if (ValidatePutRequest(form) is ObjectResult err)
            {
                _logger.Warn($"Invalid {nameof(FileController)}.{nameof(Put)} request: {err.Value}");
                return err;
            }
            
            var tasks = form.Files.Select(f => StoreFileAsync(awsAccountId, awsInstanceId, f)).ToArray();

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Caught exception is the first one, there could be more: check all the tasks.
                LogErrors(form.Files, tasks);

                return StatusCode(StatusCodes.Status500InternalServerError, GetError(form.Files, tasks));
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        private ObjectResult ValidatePutRequest(IFormCollection form)
        {
            if (form?.Files == null || form.Files.Count == 0)
            {
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Request contains no files to upload.");
                }
            }

            foreach (var file in form.Files)
            {
                if (string.IsNullOrWhiteSpace(file.FileName))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File name can not be null or whitespace.");
                }

                if (file.Length == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File can not be empty: " + file.FileName);
                }
            }

            return null;
        }

        private async Task StoreFileAsync(string awsAccountId, string awsInstanceId, IFormFile file)
        {
            _logger.Info($"Uploading file from AWS account '{awsAccountId}', instance '{awsInstanceId}': {file.FileName}");
            
            using (var stream = file.OpenReadStream())
            {
                await _storage.StoreFileAsync(awsAccountId, awsInstanceId, file.FileName, stream);
            }
        }
        
        private static string GetError(IFormFileCollection files, Task[] tasks)
        {
            Debug.Assert(files.Count == tasks.Length);

            var failedFiles = files.Zip(tasks, (f, t) => t.IsFaulted ? f.FileName : null)
                .Where(f => f != null);
            
            return "Failed to upload files: " + string.Join(", ", failedFiles);
        }

        private void LogErrors(IFormFileCollection files, Task[] tasks)
        {
            Debug.Assert(files.Count == tasks.Length);

            for (var i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsFaulted)
                {
                    _logger.Error("Failed to store file: " + files[i].FileName, tasks[i].Exception);
                }
            }
        }
    }
}
