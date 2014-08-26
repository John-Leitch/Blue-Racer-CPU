using Components.Aphid.Interpreter;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using Components.Aphid.Parser;
using System.Net;

namespace Components.Aphid.Library
{
    public class Datagram
    {
        [AphidProperty("localPort")]
        public int LocalPort { get; set; }

        [AphidProperty("remotePort")]
        public int RemotePort { get; set; }

        [AphidProperty("remoteHost")]
        public string RemoteHost { get; set; }

        [AphidProperty("data")]
        public byte[] Data { get; set; }
    }

    [AphidLibraryAttribute("udp")]
    public class UdpLibrary
    {
        private static List<UdpClient> _clients = new List<UdpClient>();

        [AphidInteropFunction("__udp.create")]
        public static UdpClient UdpCreate()
        {
            var client = new UdpClient();

            lock (_clients)
            {
                _clients.Add(client);
            }

            return client;
        }

        private static Dictionary<string, IPAddress> _ipv4DnsCache = new Dictionary<string, IPAddress>();

        private static IPAddress GetIPV4Address(string host)
        {
            IPAddress address;

            if (!_ipv4DnsCache.TryGetValue(host, out address))
            {
                address = Dns
                    .GetHostAddresses("localhost")
                    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                _ipv4DnsCache.Add(host, address);
            }

            return address;
        }

        [AphidInteropFunction("__udp.send", UnwrapParameters = false)]
        public static AphidObject UdpSend(AphidObject clientObj, AphidObject datagramObj, AphidObject hostObj, AphidObject portObj)
        {
            var host = (string)hostObj.Value;
            var address = GetIPV4Address(host);
            var port = (decimal)portObj.Value;
            var ep = new IPEndPoint(address, (int)port);
            var buffer = AphidByteConverter.ToBytes(datagramObj);
            var client = (UdpClient)clientObj.Value;
            client.Send(buffer, buffer.Length, ep);

            var datagram = new Datagram()
            {
                LocalPort = ep.Port,
                RemotePort = (int)port,                
                RemoteHost = host,
                Data = buffer,
            };

            return AphidObject.ConvertFrom(datagram);
        }

        [AphidInteropFunction("__udp.recv")]
        public static AphidObject UdpReceive(UdpClient client, decimal port)
        {
            var ep = new IPEndPoint(IPAddress.Any, (int)port);
            var buffer = client.Receive(ref ep);

            var datagram = new Datagram()
            {
                LocalPort = (int)port,
                RemotePort = ep.Port,
                RemoteHost = ep.Address.ToString(),
                Data = buffer,
            };

            return AphidObject.ConvertFrom(datagram);
        }

        [AphidInteropFunction("__udp.close")]
        public static void UdpClose(UdpClient client)
        {
            lock (_clients)
            {
                client.Close();
                _clients.Remove(client);
            }
        }
    }
}

