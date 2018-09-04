using System.Collections.Generic;
using System.Linq;

namespace MonoSign.U2F.Demo
{
    public static class App
    {
        public static List<User> Users { get; set; }
        public static User CurrentUser { get; set; }
        public static Dictionary<string, FidoStartedRegistration> Registrations { get; set; }
        public static Dictionary<string, FidoDeviceRegistration> DeviceRegistrations { get; set; }

        static App()
        {
            Users = new List<User>();
            Registrations = new Dictionary<string, FidoStartedRegistration>();
            DeviceRegistrations = new Dictionary<string, FidoDeviceRegistration>();
        }

        public static void AddRegistration(string challenge, FidoStartedRegistration registration)
        {
            if (Registrations.ContainsKey(challenge))
            {
                Registrations[challenge] = registration;
            }
            else
            {
                Registrations.Add(challenge, registration);
            }
        }

        public static void AddDeviceRegistration(string userName, FidoDeviceRegistration deviceRegistration)
        {
            if (DeviceRegistrations.ContainsKey(userName))
            {
                DeviceRegistrations[userName] = deviceRegistration;
            }
            else
            {
                DeviceRegistrations.Add(userName, deviceRegistration);
            }
        }
    }
}