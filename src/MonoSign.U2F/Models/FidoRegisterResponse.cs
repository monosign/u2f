using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	public class FidoRegisterResponse : IValidate
	{
		public FidoRegistrationData RegistrationData { get; set; }

		public FidoClientData ClientData { get; set; }

		public static FidoRegisterResponse FromJson(string json)
		{
			return JsonConvert.DeserializeObject<FidoRegisterResponse>(json);
		}

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

		public void Validate()
		{
			if (RegistrationData == null)
				throw new InvalidOperationException("Registration data is missing in registration response");

            if (ClientData == null)
                throw new InvalidOperationException("Client data is missing in registration response");

		    RegistrationData.Validate();
            ClientData.Validate();
		}
	}
}
