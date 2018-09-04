using System;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
	/// <summary>
	/// FIDO FacetId (see section 3 in FIDO specification for valid FacetIds)
	/// </summary>
	[JsonConverter(typeof(FidoFacetIdConverter))]
	public class FidoFacetId : IEquatable<FidoAppId>, IEquatable<FidoFacetId>
	{
		private readonly Uri _facetUri;

		public FidoFacetId(Uri facetId)
		{
			if (!facetId.IsAbsoluteUri)
				ThrowFormatException();
			_facetUri = facetId;

			ValidateUri(facetId);
		}

		public FidoFacetId(string facetId)
		{
			if (!Uri.TryCreate(facetId, UriKind.Absolute, out _facetUri))
				ThrowFormatException();

			ValidateUri(_facetUri);
		}

		private void ValidateUri(Uri uri)
		{
			var scheme = uri.Scheme.ToLowerInvariant();
			if (scheme != "http" && scheme != "https")
				ThrowFormatException();
		}

		public bool Equals(FidoAppId other)
		{
			if (other == null) return false;
			return ToString().StartsWith(other.ToString());
		}

		public bool Equals(FidoFacetId other)
		{
			if (other == null) return false;
			var localAuthority = Helpers.GetAuthority(_facetUri);
			var otherAuthority = Helpers.GetAuthority(other._facetUri);
			return localAuthority == otherAuthority;
		}

		private static void ThrowFormatException()
		{
			throw new FormatException("FIDO Facet ID must be a URL prefix (e.g. 'https://website.com')");
		}

		public override string ToString()
		{
			return Helpers.GetAuthority(_facetUri);
		}
	}
}
