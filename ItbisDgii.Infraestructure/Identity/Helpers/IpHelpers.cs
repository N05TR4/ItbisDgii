using System.Net;
using System.Net.Sockets;

namespace ItbisDgii.Infraestructure.Identity.Helpers
{
    public class IpHelpers
    {
        public static string GetIpAdress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }
    }
}
