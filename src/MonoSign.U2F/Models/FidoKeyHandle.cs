using System;
using System.Linq;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoKeyHandleConverter))]
	public class FidoKeyHandle : IEquatable<FidoKeyHandle>, IValidate
	{
		private readonly byte[] _bytes;

		public FidoKeyHandle(byte[] keyHandleBytes)
		{
			if (keyHandleBytes == null) throw new ArgumentNullException("keyHandleBytes");

			_bytes = keyHandleBytes;
		}

		public static FidoKeyHandle FromWebSafeBase64(string keyHandle)
		{
			if (keyHandle == null) throw new ArgumentNullException("keyHandle");

			return new FidoKeyHandle(WebSafeBase64Converter.FromBase64String(keyHandle));
		}

		public byte[] ToByteArray()
		{
			return _bytes;
		}

		public bool Equals(FidoKeyHandle other)
		{
			if (other == null) return false;
			return ToWebSafeBase64() == other.ToWebSafeBase64();
		}

		public string ToWebSafeBase64()
		{
			return WebSafeBase64Converter.ToBase64String(_bytes);
		}

		public override string ToString()
		{
			return ToWebSafeBase64();
		}

        public void Validate()
        {
            if (_bytes == null || !_bytes.Any())
                throw new InvalidOperationException("KeyHandle is missing");
        }
	}
}
