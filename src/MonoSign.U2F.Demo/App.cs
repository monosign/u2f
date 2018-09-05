using System.Collections.Generic;
using System.Linq;

namespace MonoSign.U2F.Demo
{
    public static class App
    {
        public static List<User> Users { get; set; }
        public static User CurrentUser { get; set; }
        public static Dictionary<string, FidoStartedRegistration> Registrations { get; set; }

        static App()
        {
            Users = new List<User>();
            Registrations = new Dictionary<string, FidoStartedRegistration>();
        }

        public static void AddRegistration(string challenge, FidoStartedRegistration registration)
        {
            if (Registrations.ContainsKey(challenge))
                Registrations[challenge] = registration;
            else
                Registrations.Add(challenge, registration);
        }

        public static void AddDeviceRegistration(string deviceName, FidoDeviceRegistration deviceRegistration)
        {
            CurrentUser.Devices.Add(new Device{
                Name = deviceName,
                Identifier = deviceRegistration.KeyHandle.ToString(),
                Usage = 0,
                Data = deviceRegistration.ToJson()
            });
        }
    }
}