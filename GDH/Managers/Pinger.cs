using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GDH.Managers
{
    /// <summary>
    /// The pinger is the class that pings
    /// </summary>
    public class Pinger
    {
        private string _hostname;
        private int _port;

        public Pinger(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        /// <summary>
        /// Check if an server is avaiable with ping
        /// </summary>
        /// <returns>The avaiability of the server</returns>
        public bool PingCheck()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    if (!client.ConnectAsync(_hostname, _port).Wait(1000))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
