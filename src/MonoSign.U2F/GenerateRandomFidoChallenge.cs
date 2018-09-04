using Org.BouncyCastle.Security;

namespace MonoSign.U2F
{
	/// <summary>
	/// FIDO challenge generation using a cryptographically secure RNG
	/// </summary>
	public class GenerateRandomFidoChallenge : IGenerateFidoChallenge
	{
		private static readonly SecureRandom SecureRandom = new SecureRandom();

		/// <summary>
		/// Generate a random challenge
		/// </summary>
		public byte[] GenerateChallenge()
		{
			var randomBytes = new byte[32];
			SecureRandom.NextBytes(randomBytes);
			return randomBytes;
		}
	}
}
