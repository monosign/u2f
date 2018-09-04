using System;
using System.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoPublicKeyConverter))]
	public class FidoPublicKey : IEquatable<FidoPublicKey>, IValidate
	{
		private readonly byte[] _bytes;

		public FidoPublicKey(byte[] publicKeyBytes)
		{
			if (publicKeyBytes == null) throw new ArgumentNullException("publicKeyBytes");

			_bytes = publicKeyBytes;
		}

		public ICipherParameters PublicKey
		{
			get
			{
				var curve = SecNamedCurves.GetByOid(SecObjectIdentifiers.SecP256r1);
				var point = curve.Curve.DecodePoint(_bytes);
				var ecP = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

				return new ECPublicKeyParameters(point, ecP);
			}
		}

		public static FidoPublicKey FromWebSafeBase64(string publicKey)
		{
			if (publicKey == null) throw new ArgumentNullException("publicKey");

			return new FidoPublicKey(WebSafeBase64Converter.FromBase64String(publicKey));
		}

		public byte[] ToByteArray()
		{
			return _bytes;
		}

		public bool Equals(FidoPublicKey other)
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
                throw new InvalidOperationException("Public key is missing");

            if (PublicKey == null)
                throw new InvalidOperationException("Invalid public key");
        }
    }
}
