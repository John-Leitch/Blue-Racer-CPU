using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public static class TcpHelper
    {
        public static NetworkStream GetStream(string host, int port)
        {
            var client = new TcpClient();
            client.Connect(host, port);
            
            return client.GetStream();
        }
    }
}
