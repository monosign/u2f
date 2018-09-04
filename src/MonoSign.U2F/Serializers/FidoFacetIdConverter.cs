using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	public class FidoFacetIdConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
		    return new FidoFacetId(reader.Value.ToString());
		}

	    public override bool CanConvert(Type objectType)
		{
			return typeof(FidoFacetId).IsAssignableFrom(objectType);
		}
	}
}
