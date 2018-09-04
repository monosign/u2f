using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	public class FidoDeviceRegistration : IEquatable<FidoDeviceRegistration>, IValidate
	{
		public FidoKeyHandle KeyHandle { get; set; }

		public FidoPublicKey PublicKey { get; set; }

		public FidoAttestationCertificate Certificate { get; set; }

		public uint Counter { get; set; }

	    public FidoDeviceRegistration()
	    {
	    }

		public FidoDeviceRegistration(FidoKeyHandle keyHandle, FidoPublicKey publicKey, FidoAttestationCertificate certificate, uint counter)
		{
			if (keyHandle == null) throw new ArgumentNullException("keyHandle");
			if (publicKey == null) throw new ArgumentNullException("publicKey");
			if (certificate == null) throw new ArgumentNullException("certificate");

			KeyHandle = keyHandle;
			PublicKey = publicKey;
			Certificate = certificate;
			Counter = counter;
		}

		public void UpdateCounter(uint newCounter)
		{
			if (newCounter <= Counter)
			{
				throw new InvalidOperationException("Counter value too small!");
			}
			Counter = newCounter;
		}

		public static FidoDeviceRegistration FromJson(string json)
		{
			return JsonConvert.DeserializeObject<FidoDeviceRegistration>(json);
		}

		public string ToJson()
		{
			var json = JsonConvert.SerializeObject(this);
			return json;
		}

		public bool Equals(FidoDeviceRegistration other)
		{
			if (other == null) return false;

			return
				Counter == other.Counter &&
				Certificate.Equals(other.Certificate) &&
				KeyHandle.Equals(other.KeyHandle) &&
				PublicKey.Equals(other.PublicKey);
		}

	    public void Validate()
        {
            if (KeyHandle == null)
                throw new InvalidOperationException("Key handle must not be null");
            if (PublicKey == null)
                throw new InvalidOperationException("Public key must not be null");
            if (Certificate == null)
                throw new InvalidOperationException("Certificate data must not be null");

            KeyHandle.Validate();
            PublicKey.Validate();
            Certificate.Validate();
	    }
	}
}
