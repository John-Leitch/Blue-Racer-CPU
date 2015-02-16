using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class StructMember
    {
        public string Name { get; private set; }

        public string Type { get; private set; }

        public int Offset { get; private set; }

        public int Size { get; private set; }

        public StructMember(string name, string type, int offset, int size)
        {
            Name = name;
            Type = type;
            Offset = offset;
            Size = size;
        }
    }
}
