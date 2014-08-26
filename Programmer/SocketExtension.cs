using System;
using Microsoft.SPOT;
using System.Net.Sockets;

namespace Programmer
{
    public static class SocketExtension
    {
        public static int Send(this Socket socket, byte b)
        {
            return socket.Send(new[] { b });
        }

        public static byte[] Receive(this Socket socket, int length)
        {
            var buffer = new byte[length];
            var totalReceived = 0;

            while (totalReceived != length)
            {
                var tmp = new byte[length - totalReceived];
                var received = socket.Receive(tmp);
                Array.Copy(tmp, 0, buffer, totalReceived, received);
                totalReceived += received;
            }

            return buffer;
        }
    }
}
