namespace MonoSign.U2F
{
	/// <summary>
	/// Interface for FIDO challenge generation
	/// </summary>
    public interface IGenerateFidoChallenge
    {
		/// <summary>
		/// Generate a random challenge
		/// </summary>
		byte[] GenerateChallenge();
	}
}
