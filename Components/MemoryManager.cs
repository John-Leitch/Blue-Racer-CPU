using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class MemoryManager
    {
        private Stream _stream;

        private bool[] _bitmap;

        private int _growSize = 0x100;

        private Dictionary<int, int> _sizeTable = new Dictionary<int, int>();

        public int PageSize { get; private set; }

        public bool ZeroMemory { get; set; }

        public MemoryManager(Stream stream)
            : this(stream, 0x1000)
        {
        }

        public MemoryManager(Stream stream, int pageSize)
        {
            _stream = stream;
            PageSize = pageSize;
            _bitmap = new bool[_growSize];
            ZeroMemory = true;
        }

        private void Grow()
        {
            Array.Resize(ref _bitmap, _bitmap.Length + _growSize);
        }

        public Allocation Allocate(int size)
        {
            var pageCount = GetPageCount(size);
            int index;

            lock (_bitmap)
            {
                while ((index = FindBlock(pageCount)) == -1)
                {
                    Grow();
                }

                MarkBlock(index, pageCount);
            }

            var actualSize = pageCount * PageSize;

            return new Allocation(this, index, size, actualSize);
        }

        public void Free(Allocation allocation)
        {
            var pageCount = _sizeTable[allocation.Handle];

            if (ZeroMemory)
            {
                lock (_stream)
                {
                    SetPosition(allocation);
                    var buffer = new byte[allocation.Size];
                    _stream.Write(buffer);
                }
            }

            lock (_bitmap)
            {
                ClearBlock(allocation.Handle, pageCount);
            }
        }

        public byte[] Read(Allocation allocation)
        {
            return Read(allocation, GetSize(allocation));
        }

        public byte[] Read(Allocation allocation, int bufferSize)
        {
            lock (_stream)
            {
                SetPosition(allocation);

                return _stream.Read(bufferSize);
            }
        }

        public void Write(Allocation allocation, byte[] buffer)
        {
            var size = GetSize(allocation);

            if (buffer.Length > size)
            {
                throw new InternalBufferOverflowException();
            }

            lock (_stream)
            {
                SetPosition(allocation);
                _stream.Write(buffer);
            }
        }

        private int GetPageCount(int size)
        {
            var x = size / PageSize;

            if ((size % PageSize) != 0)
            {
                x++;
            }

            return x;
        }

        private int FindBlock(int pageCount)
        {
            for (int x = 0; x <= _bitmap.Length - pageCount; x++)
            {
                var isMatch = true;

                for (int y = 0; y < pageCount; y++)
                {
                    if (_bitmap[x + y])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return x;
                }
            }

            return -1;
        }

        private void MarkBlock(int index, int pageCount)
        {
            SetBlock(index, pageCount, true);
        }

        private void ClearBlock(int index, int pageCount)
        {
            SetBlock(index, pageCount, false);
        }

        private void SetBlock(int index, int pageCount, bool value)
        {
            if (value)
            {
                _sizeTable.Add(index, pageCount);
            }
            else
            {
                _sizeTable.Remove(index);
            }

            for (int i = index; i < index + pageCount; i++)
            {
                _bitmap[i] = value;
            }
        }

        private int GetSize(Allocation allocation)
        {
            return _sizeTable[allocation.Handle] * PageSize;
        }

        private int GetPosition(Allocation allocation)
        {
            return allocation.Handle * PageSize;
        }

        private void SetPosition(Allocation allocation)
        {
            _stream.Position = GetPosition(allocation);
        }
    }
}
