using System;

namespace MonoSign.U2F
{
	public class FidoStartedRegistration
	{
		public FidoAppId AppId { get; set; }
		public string Challenge { get; set; }

	    public FidoStartedRegistration()
	    {
	    }

		public FidoStartedRegistration(FidoAppId appId, string challenge)
		{
			if (appId == null) throw new ArgumentNullException("appId");
			if (challenge == null) throw new ArgumentNullException("challenge");

			AppId = appId;
			Challenge = challenge;
		}
	}
}
