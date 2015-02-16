using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidStruct
    {
        public string Name { get; private set; }

        public StructMember[] Members { get; private set; }

        public int Size { get; private set; }

        public AphidStruct(string name, StructMember[] members, int size)
        {
            Name = name;
            Members = members;
            Size = size;
        }
    }
}
