using System;

namespace MonoSign.U2F
{
	public class FidoStartedAuthentication
	{
		public FidoAppId AppId { get; set; }
		public string Challenge { get; set; }
		public FidoKeyHandle KeyHandle { get; set; }

	    public FidoStartedAuthentication()
	    {
	    }

		public FidoStartedAuthentication(FidoAppId appId, string challenge, FidoKeyHandle keyHandle)
		{
			if (appId == null) throw new ArgumentNullException("appId");
			if (challenge == null) throw new ArgumentNullException("challenge");
			if (keyHandle == null) throw new ArgumentNullException("keyHandle");

			AppId = appId;
			Challenge = challenge;
			KeyHandle = keyHandle;
		}
	}
}
