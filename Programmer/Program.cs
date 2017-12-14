using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Net.NetworkInformation;
using Processor;

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never,
        Collapsed,
        RootHidden
    }
}

namespace Programmer
{
    public class Program
    {
        private static CpuProgrammer _programmer = new CpuProgrammer();

        private static void Interpret(byte[] buffer, Socket client)
        {
            if (buffer.Length != 5)
            {
                return;
            }

            var operand = BitConverter.ToUInt32(buffer, 1);
            
            switch ((ProgrammerCommand)buffer[0])
            {
                case ProgrammerCommand.Break:
                    _programmer.Break();
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.Continue:
                    _programmer.Continue();
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.Reset:
                    _programmer.Reset();
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.Restart:
                    _programmer.Restart();
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.Read:                    
                    var data = _programmer.Read4(operand);
                    client.Send(BitConverter.GetBytes(data));
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.Write:
                    //var buffer2 = new byte[4];
                    //client.Receive(buffer2);
                    var buffer2 = client.Receive(4);
                    data = BitConverter.ToUInt32(buffer2, 0);
                    _programmer.Write4(operand, data);
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.GetContext:
                    _programmer.GetContext().WriteTo(client);
                    client.Send(buffer[0]);                    
                    break;

                case ProgrammerCommand.GetPageFlags:
                    client.Send(new[] { _programmer.GetPageFlags(operand) });
                    client.Send(buffer[0]);
                    break;

                case ProgrammerCommand.SetPageFlags:
                    //buffer2 = new byte[1];
                    //client.Receive(buffer2);
                    buffer2 = client.Receive(1);
                    _programmer.SetPageFlags(operand, (MemoryAccessFlag)buffer2[0]);
                    client.Send(buffer[0]);
                    break;
            }
        }

        private static void Test()
        {
            var f = _programmer.GetPageFlags(10);
            _programmer.SetPageFlags(10, MemoryAccessFlag.Write);
            var f2 = _programmer.GetPageFlags(10);


            var context123 = _programmer.GetContext();
            for (uint i = 0; i < 100; i++)
            {
                var flags = _programmer.GetPageFlags(i);
            }

            for (uint i = 0; i < 10; i++)
            {
                _programmer.SetPageFlags(i, MemoryAccessFlag.Read | Processor.MemoryAccessFlag.Execute);
            }

            _programmer.Restart();
            _programmer.Continue();
            var context = _programmer.GetContext();
        }

        public static void Main()
        {
            

            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];

            Debug.Print(networkInterface.IPAddress.ToString());
            //networkInterface.IsDynamicDnsEnabled = false;
            // 
            networkInterface.EnableStaticIP("192.168.1.200", "255.255.255.0", "192.168.1.1");
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 5230));
                socket.Listen(1);

                while (true)
                {
                    using (var client = socket.Accept())
                    {
                        //new Thread(() =>
                        //{
                            var clientActive = true;

                            while (clientActive)
                            {
                                //var buffer = new byte[5];
                                //client.Receive(buffer);
                                var buffer = client.Receive(5);
                                Interpret(buffer, client);
                            }
                        //}).Start();
                    }
                }
            }
        }

    }
}
