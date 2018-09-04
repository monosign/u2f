using System;
using System.IO;
using Newtonsoft.Json;

namespace MonoSign.U2F
{
    [JsonConverter(typeof(FidoSignatureDataConverter))]
	public class FidoSignatureData : IValidate
	{
		public byte UserPresence { get; set; }

		public uint Counter { get; set; }

		public FidoSignature Signature { get; set; }

        public FidoSignatureData()
        {
        }

		public FidoSignatureData(byte userPresence, uint counter, FidoSignature signature)
		{
			UserPresence = userPresence;
			Counter = counter;
			Signature = signature;
		}

		public static FidoSignatureData FromWebSafeBase64(string webSafeBase64)
		{
		    return FromBytes(WebSafeBase64Converter.FromBase64String(webSafeBase64));
		}

        public static FidoSignatureData FromBytes(byte[] rawRegistrationData)
		{
		    if (rawRegistrationData == null) throw new ArgumentNullException("rawRegistrationData");

            using (var mem = new MemoryStream(rawRegistrationData))
			{
				return FromStream(mem);
			}
		}

        private static FidoSignatureData FromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var binaryReader = new BinaryReader(stream))
			{
				var userPresence = binaryReader.ReadByte();
				var counterBytes = binaryReader.ReadBytes(4);

				if (BitConverter.IsLittleEndian)
					Array.Reverse(counterBytes);

				var counter = BitConverter.ToUInt32(counterBytes, 0);

				var size = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
				var signatureBytes = binaryReader.ReadBytes((int)size);

				return new FidoSignatureData(
					userPresence,
					counter,
					new FidoSignature(signatureBytes));
			}
        }

        public string ToWebSafeBase64()
        {
            return WebSafeBase64Converter.ToBase64String(ToBytes());
        }

        public byte[] ToBytes()
		{
			using (var mem = new MemoryStream())
			{
				ToStream(mem);
				return mem.ToArray();
			}
		}

		public void ToStream(Stream stream)
		{
			using (var binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.Write(UserPresence);
				
				var counterBytes = BitConverter.GetBytes(Counter);

				if (BitConverter.IsLittleEndian)
					Array.Reverse(counterBytes);

				binaryWriter.Write(counterBytes);
				binaryWriter.Write(Signature.ToByteArray());
			}
		}

        public void Validate()
        {
            if (Signature == null)
                throw new InvalidOperationException("Signature must not be null");

            Signature.Validate();
        }
	}
}
