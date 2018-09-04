using System;
using System.Text;

namespace MonoSign.U2F
{
	/// <summary>
	/// BASE64 converter that uses only web-safe characters
	/// </summary>
	public static class WebSafeBase64Converter
	{
		/// <summary>
		/// Converts a byte array to a web-safe base64 string
		/// </summary>
		/// <param name="byteArray">byte array to convert</param>
		/// <returns>web safe base64 encoded string</returns>
		public static string ToBase64String(byte[] byteArray)
		{
			if (byteArray == null) return null;

			var result = Convert.ToBase64String(byteArray)
				.TrimEnd('=')
				.Replace('+', '-')
				.Replace('/', '_');
			return result;
		}

		/// <summary>
		/// Converts a byte array to a web-safe base64 string
		/// </summary>
		/// <param name="value">string value to convert</param>
		/// <returns>web safe base64 encoded string</returns>
		public static string ToBase64String(string value)
		{
			if (value == null) return null;

			var byteArray = Encoding.UTF8.GetBytes(value);
			return ToBase64String(byteArray);
		}

		/// <summary>
		/// Decode a web-safe base64 string to a byte array
		/// </summary>
		/// <param name="webSafeBase64">web safe base64 encoded string</param>
		/// <returns>byte array</returns>
		public static byte[] FromBase64String(string webSafeBase64)
		{
			if (webSafeBase64 == null) return null;

			webSafeBase64 = webSafeBase64
				.Trim()
				.Replace('-', '+')
				.Replace('_', '/');

			var mod4 = webSafeBase64.Length % 4;
			if (mod4 > 0)
			{
				webSafeBase64 += new string('=', 4 - mod4);
			}

			return Convert.FromBase64String(webSafeBase64);
		}
	}
}
