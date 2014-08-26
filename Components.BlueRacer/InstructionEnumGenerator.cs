using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class InstructionEnumGenerator
    {
        public static string Generate<TInternalEnum>(string name, RegisterOpcodeTemplate[] templates)
        {
            var generator = new EnumGenerator(name);
            var internalValues = EnumHelper.GetValues<TInternalEnum>();

            foreach (var e in internalValues)
            {
                generator.Add(e.ToString(), (byte)(object)e);
            }

            var generatedOpcodes = templates
                .SelectMany(x => x.CreateEnums(Register.Count))
                .ToArray();

            foreach (var op in generatedOpcodes)
            {
                generator.Add(op.Item1, op.Item2);
            }

            var enum2 = generator.ToString();

            var matches = Regex
                .Matches(enum2, @"0x[0-9a-fA-F]{2}")
                .OfType<Match>()
                .GroupBy(x => x.Value)
                .ToArray();

            var matches23 = Regex
                .Matches(enum2, @"\s*(.*?)\s*=\s*0x(.*?),")
                .OfType<Match>()
                .Select(x => x.Groups[1])
                .ToArray();

            var collisions = matches.Where(x => x.Count() > 1);

            if (collisions.Any())
            {
                throw new InvalidOperationException();
            }

            return enum2;
        }
    }
}
