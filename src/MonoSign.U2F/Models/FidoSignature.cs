using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoSignatureConverter))]
	public class FidoSignature : IEquatable<FidoSignature>
	{
		private readonly byte[] _bytes;

		public FidoSignature(byte[] signatureBytes)
		{
			if (signatureBytes == null) throw new ArgumentNullException("signatureBytes");

			_bytes = signatureBytes;
		}

		public static FidoSignature FromWebSafeBase64(string signature)
		{
			if (signature == null) throw new ArgumentNullException("signature");

			return new FidoSignature(WebSafeBase64Converter.FromBase64String(signature));
		}

		public byte[] ToByteArray()
		{
			return _bytes;
		}

		public bool Equals(FidoSignature other)
		{
			if (other == null) return false;
			return ToWebSafeBase64() == other.ToWebSafeBase64();
		}

		public void Validate()
		{
			if (_bytes == null || _bytes.Length == 0)
				throw new InvalidOperationException("Signature must not be empty");
		}

		public string ToWebSafeBase64()
		{
			return WebSafeBase64Converter.ToBase64String(_bytes);
		}

		public override string ToString()
		{
			return ToWebSafeBase64();
		}
	}
}
