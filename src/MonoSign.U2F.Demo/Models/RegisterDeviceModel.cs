using System.Collections.Generic;

namespace MonoSign.U2F.Demo.Models
{
    public class RegisterDeviceModel
    {
        public string DeviceName { get; set; }
        public string AppId { get; set; }
        public string Challenge { get; set; }
        public string RawRegisterResponse { get; set; }
        public List<string> RegisteredKeys { get; set; }

        public RegisterDeviceModel()
        {
            RegisteredKeys = new List<string>();
        }
    }
}