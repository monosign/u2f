using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	public class FidoAuthenticateResponse : IValidate
	{
		public FidoClientData ClientData { get; set; }

		public FidoSignatureData SignatureData { get; set; }

		public FidoKeyHandle KeyHandle { get; set; }

        public static FidoAuthenticateResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<FidoAuthenticateResponse>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

		public FidoAuthenticateResponse()
		{
		}

		public FidoAuthenticateResponse(FidoClientData clientData, FidoSignatureData signatureData, FidoKeyHandle keyHandle)
		{
			ClientData = clientData;
			SignatureData = signatureData;
			KeyHandle = keyHandle;
		}

		public void Validate()
		{
            if (ClientData == null)
                throw new InvalidOperationException("Client data must not be null");
            if (KeyHandle == null)
                throw new InvalidOperationException("Key handle must not be null");
            if (SignatureData == null)
                throw new InvalidOperationException("Signature data must not be null");

            ClientData.Validate();
            KeyHandle.Validate();
            SignatureData.Validate();
		}
	}
}
