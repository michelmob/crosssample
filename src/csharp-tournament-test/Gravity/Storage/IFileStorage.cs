using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gravity.Storage
{
    /// <summary>
    /// File storage, like AWS S3 or local/network disk. 
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Reads all text from specified file.
        /// Throws <see cref="StorageFileNotFoundException"/> when specified file does not exist.
        /// </summary>
        Task<string> ReadAllTextAsync(string name);

        /// <summary>
        /// Writes all text to the specified file.
        /// </summary>
        Task WriteAllTextAsync(string name, string text);

        /// <summary>
        /// Writes all data from a stream.
        /// </summary>
        Task WriteAllAsync(string name, Stream contents);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        Task DeleteAsync(string name);

        /// <summary>
        /// Determines whether specified file exists.
        /// </summary>
        Task<bool> ExistsAsync(string name);

        /// <summary>
        /// Gets the file names with specified prefix.
        /// Prefix can be null or empty.
        /// </summary>
        Task<IList<string>> GetNames(string prefix);

        /// <summary>
        /// Gets the directory separator character.
        /// </summary>
        char DirectorySeparatorChar { get; }
    }
}