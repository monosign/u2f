namespace MonoSign.U2F.Demo.Models
{
    public class AuthenticateDeviceModel
    {
        public string AppId { get; set; }
        public string Challenge { get; set; }
        public string RawAuthenticateResponse { get; set; }
        public string KeyHandle { get; set; }
    }
}