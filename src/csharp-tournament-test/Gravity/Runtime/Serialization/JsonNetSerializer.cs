using Newtonsoft.Json;

namespace Gravity.Runtime.Serialization
{
    /// <summary>
    /// Default JSON serializer
    /// </summary>
    public class JsonNetSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Converters = {new IpAddressJsonConverter()}
        };

        public object Serialize<TInput>(TInput instance) => 
            JsonConvert.SerializeObject(instance, Settings);

        public TOutput Deserialize<TOutput>(object data) => 
            JsonConvert.DeserializeObject<TOutput>((string) data, Settings);
    }
}