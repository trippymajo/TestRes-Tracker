using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrtShared.ServiceCommunication
{
    public class RedisSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
    }
}
