using System.Collections.Generic;

namespace MonoSign.U2F
{
	public interface IFidoUniversalTwoFactor
	{
		FidoStartedRegistration StartRegistration(FidoAppId appId);

		FidoDeviceRegistration FinishRegistration(
			FidoStartedRegistration startedRegistration,
			string jsonDeviceResponse,
			IEnumerable<FidoFacetId> trustedFacetIds);

		FidoStartedAuthentication StartAuthentication(
			FidoAppId appId, FidoDeviceRegistration deviceRegistration);

	    uint FinishAuthentication(FidoStartedAuthentication startedAuthentication,
	        string rawAuthResponse,
	        FidoDeviceRegistration deviceRegistration,
	        IEnumerable<FidoFacetId> trustedFacetIds);
	}
}