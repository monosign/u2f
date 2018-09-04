namespace MonoSign.U2F.Demo.Models
{
    public class RegisterDeviceModel
    {
        public string AppId { get; set; }
        public string Challenge { get; set; }
        public string RawRegisterResponse { get; set; }
    }
}