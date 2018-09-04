using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoClientDataConverter))]
	public class FidoClientData : IValidate
	{
		[JsonProperty("typ")]
		public string Type { get; set; }

		[JsonProperty("challenge")]
		public string Challenge { get; set; }

		[JsonProperty("origin")]
		public string Origin { get; set; }

		private string _overriddenRawJsonValue;
		[JsonIgnore]
		public string RawJsonValue
		{
			get { return _overriddenRawJsonValue ?? JsonConvert.SerializeObject(this); }
		}

		public static FidoClientData FromWebSafeBase64(string base64)
		{
			var json = WebSafeBase64Converter.FromBase64String(base64);
			return FromJson(Encoding.UTF8.GetString(json, 0, json.Length));
		}

		public static FidoClientData FromJson(string json)
		{
			if (json == null) throw new ArgumentNullException("json");

			var element = JObject.Parse(json);
			if (element == null)
				throw new InvalidOperationException("Client data must be in JSON format");

			JToken type, challenge, orgin;
			if (!element.TryGetValue("typ", out type))
				throw new InvalidOperationException("Client data is missing 'typ' param");
			if (!element.TryGetValue("challenge", out challenge))
				throw new InvalidOperationException("Client data is missing 'challenge' param");

			var clientData = new FidoClientData
			{
				_overriddenRawJsonValue = json,
				Type = type.ToString(),
				Challenge = challenge.ToString()
			};

			if (element.TryGetValue("origin", out orgin))
				clientData.Origin = orgin.ToString();

			return clientData;
		}

	    public string ToWebSafeBase64()
	    {
	        return WebSafeBase64Converter.ToBase64String(ToJson());
	    }

	    public string ToJson()
	    {
            return "{\"challenge\":\"" +
                (Challenge ?? "").Replace("\"", "\\\"") + "\",\"origin\":\"" +
                (Origin ?? "").Replace("\"", "\\\"") + "\",\"typ\":\"" +
                (Type ?? "").Replace("\"", "\\\"") + "\"}";
	    }

		public void Validate()
		{
			if (String.IsNullOrEmpty(Type))
				throw new InvalidOperationException("'typ' is missing in client data");

			if (String.IsNullOrEmpty(Challenge))
				throw new InvalidOperationException("'challenge' is missing in client data");

			if (String.IsNullOrEmpty(Origin))
				throw new InvalidOperationException("'origin' is missing in client data");
		}
	}
}
