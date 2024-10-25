using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace ninePing
{
    class PingClass
    {
        // args[0] can be an IPaddress or host name. 
        public static string pingHost(string HostIP)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128, 
            // but change the fragmentation behavior.
            //options.DontFragment = true;


            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = GlobalConfig.Ping.PingTimeout;
            try
            {
                PingReply reply = pingSender.Send(HostIP, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return Convert.ToString(reply.RoundtripTime);
                }
                else
                {
                    return Convert.ToString(reply.Status);
                }
            }
            catch (Exception)
            {
                return "Can't Ping " + HostIP;
            }
            finally { }


        }
    }
}
