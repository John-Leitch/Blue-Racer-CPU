using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CliArgAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsRequired { get; set; }

        public bool IsInputFile { get; set; }

        public CliArgAttribute(string name)
        {
            Name = name;
            IsRequired = false;
        }
    }
}
