using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	/// <summary>
	/// FIDO AppId (see section 3 in FIDO specification for valid FacetIds)
	/// </summary>
	[JsonConverter(typeof(FidoAppIdConverter))]
	public class FidoAppId : IEquatable<FidoAppId>, IEquatable<FidoFacetId>
	{
		private readonly Uri _appUri;

		public FidoAppId(Uri appId)
		{
			if (!appId.IsAbsoluteUri)
				ThrowFormatException();
			_appUri = appId;

			ValidateUri(appId);
		}

		public FidoAppId(string facetId)
		{
			if (!Uri.TryCreate(facetId, UriKind.Absolute, out _appUri))
				ThrowFormatException();

			ValidateUri(_appUri);
		}

		private void ValidateUri(Uri uri)
		{
			var scheme = uri.Scheme.ToLowerInvariant();
			if (scheme != "http" && scheme != "https")
				ThrowFormatException();
		}

		public bool Equals(FidoFacetId other)
		{
			if (other == null) return false;
			return ToString().StartsWith(other.ToString());
		}

		public bool Equals(FidoAppId other)
		{
			if (other == null) return false;
			var localAuthority = Helpers.GetAuthority(_appUri);
			var otherAuthority = Helpers.GetAuthority(other._appUri);
			return localAuthority == otherAuthority;
		}

		private static void ThrowFormatException()
		{
			throw new FormatException("FIDO App ID must be a URL prefix (e.g. 'https://website.com')");
		}

		public override string ToString()
		{
			return Helpers.GetAuthority(_appUri);
		}
	}
}
