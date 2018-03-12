using System.IO;
using System.Threading.Tasks;

namespace Gravity.Storage
{
    /// <summary>
    /// Discovery output storage.
    /// </summary>
    public interface IOutputStorage
    {
        /// <summary>
        /// Stores discovery output.
        /// </summary>
        Task StoreFileAsync(string awsAccountId, string awsInstanceId, string fileName, Stream contents);
    }
}
