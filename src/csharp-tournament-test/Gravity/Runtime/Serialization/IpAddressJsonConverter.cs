﻿using System;
using System.Net;
using Newtonsoft.Json;

namespace Gravity.Runtime.Serialization
{
    public class IpAddressJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }
    }
}