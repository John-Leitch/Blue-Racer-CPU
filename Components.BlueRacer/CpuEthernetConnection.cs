using Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public sealed class CpuEthernetConnection : CpuConnection
    {
        private NetworkStream _stream;

        public override void Open(string connectionString)
        {
            ExceptionHelper.InvalidOperationIf(IsConnected);
            IsConnected = true;
            _stream = TcpHelper.GetStream(connectionString, 5230);
        }

        public override void Disconnect()
        {
            ExceptionHelper.InvalidOperationIf(!IsConnected);
            IsConnected = false;
            _stream.Dispose();
            _stream = null;
        }

        public override void Dispose()
        {
            ExceptionHelper.InvalidOperationIf(IsConnected);
        }

        public override void Write(byte[] buffer)
        {
            _stream.Write(buffer);
        }

        public override byte[] Read(int bufferLength)
        {
            return _stream.ReadAll(bufferLength);
        }

        public static CpuEthernetConnection Create(string host)
        {
            var connection = new CpuEthernetConnection();
            connection.Open(host);

            return connection;
        }
    }
}
