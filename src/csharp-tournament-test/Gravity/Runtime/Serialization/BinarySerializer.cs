using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Gravity.Runtime.Serialization
{
    /// <summary>
    ///     Default Binary Serializer
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        /// <summary>
        ///     Serialize generic type TInput to byte[]
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <returns></returns>
        public object Serialize<TInput>(TInput instance)
        {
            if (instance == null)
                return null;

            var serializer = new DataContractSerializer(typeof(TInput));

            byte[] buffer;
            using (var ms = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ms))
            {
                serializer.WriteObject(writer, instance);
                writer.Flush();
                buffer = ms.ToArray();
            }

            return buffer;
        }


        /// <summary>
        ///     Deserialize byte[] to generic type of TOutput
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public TOutput Deserialize<TOutput>(object data)
        {
            var deserializer = new DataContractSerializer(typeof(TOutput));

            using (var ms = new MemoryStream((byte[]) data))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max))
            {
                return (TOutput) deserializer.ReadObject(reader);
            }
        }
    }
}