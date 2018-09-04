using System;
using System.Collections.Generic;

namespace MonoSign.U2F.Demo
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Device> Devices { get; set; }
    }
}