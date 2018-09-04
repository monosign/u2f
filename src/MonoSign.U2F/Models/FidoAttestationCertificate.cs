using System;
using System.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.X509;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoAttestationCertificateConverter))]
	public class FidoAttestationCertificate : IEquatable<FidoAttestationCertificate>, IValidate
	{
		public byte[] RawData { get; private set; }
		public X509Certificate Certificate { get; private set; }

		public FidoAttestationCertificate(byte[] attestationCertificateBytes)
		{
			if (attestationCertificateBytes == null) throw new ArgumentNullException("attestationCertificateBytes");

			Certificate = new X509CertificateParser().ReadCertificate(attestationCertificateBytes);
			RawData = attestationCertificateBytes;
		}

		public static FidoAttestationCertificate FromWebSafeBase64(string attestationCertificate)
		{
			if (attestationCertificate == null) throw new ArgumentNullException("attestationCertificate");

			return new FidoAttestationCertificate(WebSafeBase64Converter.FromBase64String(attestationCertificate));
		}

		public bool Equals(FidoAttestationCertificate other)
		{
			if (other == null) return false;
			return RawData.SequenceEqual(other.RawData);
		}

		public string ToWebSafeBase64()
		{
			return WebSafeBase64Converter.ToBase64String(RawData);
		}

		public override string ToString()
		{
			return ToWebSafeBase64();
		}

        public void Validate()
        {
            if (RawData == null || !RawData.Any())
                throw new InvalidOperationException("Attestation certificate is missing");

            if (Certificate == null)
                throw new InvalidOperationException("Invalid attestation certificate");
        }
    }
}
