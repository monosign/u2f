using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoSign.U2F
{
	public class FidoRegisterResponseSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var registerResponse = (FidoRegisterResponse)value;

			writer.WriteStartObject();

			writer.WritePropertyName("RegistrationData");
			serializer.Serialize(writer, registerResponse.RegistrationData);

			writer.WritePropertyName("ClientData");
			serializer.Serialize(writer, registerResponse.ClientData);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var properties = jsonObject.Properties().ToLookup(x => x.Name.ToLowerInvariant());

			var serializedRegistrationData = properties["registrationdata"].Single().Value.ToString();
			var serializedClientData = properties["clientdata"].Single().Value.ToString();

			return new FidoRegisterResponse
			{
				RegistrationData = FidoRegistrationData.FromWebSafeBase64(serializedRegistrationData),
				ClientData = FidoClientData.FromWebSafeBase64(serializedClientData)
			};
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(FidoRegisterResponseSerializer).IsAssignableFrom(objectType);
		}
	}
}
