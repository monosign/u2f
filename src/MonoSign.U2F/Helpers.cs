using System;
using Org.BouncyCastle.Crypto.Digests;

namespace MonoSign.U2F
{
    internal static class Helpers
    {
        public static byte[] Sha256(string text)
        {
            var bytes = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);

            var sha256 = new Sha256Digest();
            var hash = new byte[sha256.GetDigestSize()];
            sha256.BlockUpdate(bytes, 0, bytes.Length);
            sha256.DoFinal(hash, 0);
            return hash;
        }

        public static string GetAuthority(Uri uri)
        {
            var isDefaultPort =
                (uri.Scheme == "http" && uri.Port == 80) ||
                (uri.Scheme == "https" && uri.Port == 443);

            return uri.Scheme + "://" + uri.DnsSafeHost + (isDefaultPort ? "" : ":" + uri.Port);
        }
    }
}
