using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	public class FidoAttestationCertificateConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((FidoAttestationCertificate)value).ToWebSafeBase64());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
		    return FidoAttestationCertificate.FromWebSafeBase64(reader.Value.ToString());
		}

	    public override bool CanConvert(Type objectType)
		{
			return typeof(FidoAttestationCertificate).IsAssignableFrom(objectType);
		}
	}
}
