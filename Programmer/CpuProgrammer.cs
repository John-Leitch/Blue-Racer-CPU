using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Processor;

namespace Programmer
{
    public sealed class CpuProgrammer : IDisposable
    {
        public static CpuProgrammer Instance { get; private set; }

        private SPI.Configuration _spiConfig;

        private SPI _spi;

        public CpuProgrammer()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException();
            }

            Instance = this;

            //_spiConfig = new SPI.Configuration(
            //    SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D10,
            //    false,
            //    0,
            //    0,
            //    false,
            //    true,
            //    5000,
            //    SPI_Devices.SPI1);

            _spiConfig = new SPI.Configuration(
                SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D10,
                false,
                20,
                20,
                false,
                true,
                2000,
                SPI_Devices.SPI1);

            _spi = new SPI(_spiConfig);
        }

        public void Break()
        {
            _spi.WriteRead(CpuCommand.Break);
        }

        public void Continue()
        {
            _spi.WriteRead(CpuCommand.Continue);
        }

        public void Restart()
        {
            _spi.WriteRead(CpuCommand.Restart);
        }

        public void Reset()
        {
            _spi.WriteRead(CpuCommand.Reset);
        }

        private uint Nop()
        {
            return _spi.WriteRead(CpuCommand.Nop);
        }

        public uint Read4(uint address)
        {
            _spi.WriteRead(CpuCommand.ReadAddress);
            _spi.WriteRead(address);
            _spi.WriteRead(CpuCommand.Read4);

            return Nop();
        }

        public byte[] ReadBuffer(uint address, uint length)
        {
            var buffer = new byte[length];
            var end = address + length;

            for (uint a = address; a < end; a += 4)
            {
                var b = BigEndianBitConverter.GetBytes(Read4(a));
                b.CopyTo(buffer, (int)(a - address));

            }

            return buffer;
        }

        public void Write4(uint address, uint data)
        {
            _spi.WriteRead(CpuCommand.WriteAddress);
            _spi.WriteRead(address);

            _spi.WriteRead(CpuCommand.Write4);
            _spi.WriteRead(data);
        }

        private uint GetValue(CpuCommand command)
        {
            _spi.WriteRead(command);

            return Nop();
        }

        public uint GetInstructionAddress()
        {
            return GetValue(CpuCommand.GetInstructionAddress);
        }

        public uint GetProgramCounter()
        {
            return GetValue(CpuCommand.GetProgramCounter);
        }

        public uint GetOpcode()
        {
            return GetValue(CpuCommand.GetOpcode);
        }

        public uint GetOperands()
        {
            return GetValue(CpuCommand.GetOperands);
        }

        public uint GetError()
        {
            return GetValue(CpuCommand.GetError);
        }

        public uint GetErrorCode()
        {
            return GetValue(CpuCommand.GetErrorCode);
        }

        public void SetPageFlags(uint pageNumber, MemoryAccessFlag flag)
        {
            var packet = ((uint)flag) << 29;

            if (pageNumber > 0x1FFFFFFF)
            {
                throw new InvalidOperationException();
            }

            packet |= pageNumber;

            var aa = _spi.WriteRead(CpuCommand.SetPageFlags);
            var bb = _spi.WriteRead(packet);
            var cc = Nop();
        }

        public byte GetPageFlags(uint pageNumber)
        {
            _spi.WriteRead(CpuCommand.GetPageFlags);
            _spi.WriteRead(pageNumber);
            var x = Nop();
            return (byte)x;
        }

        public CpuContext GetContext()
        {
            return new CpuContext(
                GetInstructionAddress(),
                GetProgramCounter(),
                GetOpcode(),
                GetOperands(),
                GetError(),
                GetErrorCode());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
