using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class StructResolver
    {
        private const string _dword = "dword";

        private static readonly string[] _primitives = new[] { _dword };

        private StructResolver _parent;

        private Dictionary<string, AphidStruct> _types = new Dictionary<string, AphidStruct>();

        public StructResolver(StructResolver parent)
        {
            _parent = parent;
        }

        public StructResolver()
            : this(null)
        {
        }

        private int ResolveTypeSize(string type)
        {
            switch (type)
            {
                case _dword:
                    return 4;

                default:
                    return ResolveCustomTypeSize(type);
            }
        }

        private int ResolveCustomTypeSize(string type)
        {
            AphidStruct aphidStruct;

            if (!_types.TryGetValue(type, out aphidStruct))
            {
                throw new InvalidOperationException();
            }

            return aphidStruct.Size;
        }

        public string GetType(IdentifierExpression identifier)
        {
            return identifier.Attributes.Any() ? 
                identifier.Attributes.Single().Identifier : 
                _dword;
        }

        public void InterpretType(IdentifierExpression identifierExp, ObjectExpression objectExp)
        {
            var name = identifierExp.Identifier;
            var offset = 0;

            var fields = objectExp.Pairs
                .Select(x => x.LeftOperand)
                .Cast<IdentifierExpression>()
                .Select(x =>
                {
                    var type = GetType(x);
                    var size = ResolveTypeSize(type);
                    var ofs = offset;
                    offset += size;

                    return new StructMember(x.Identifier, type, ofs, size);
                })
                .ToArray();

            var structSize = fields.Sum(x => x.Size);
            var aphidStruct = new AphidStruct(name, fields, structSize);

            if (!_types.ContainsKey(name))
            {
                _types.Add(name, aphidStruct);
            }
            else
            {
                _types[name] = aphidStruct;
            }
        }

        public AphidStruct ResolveType(IdentifierExpression identifier)
        {
            var type = GetType(identifier);

            return ResolveType(type);            
        }

        public AphidStruct ResolveType(string type)
        {
            return _types[type];
        }

        public int ResolveOffset(string type, string[] path)
        {
            var offset = 0;
            var aphidStruct = ResolveType(type);

            foreach (var memberName in path)
            {
                var member = aphidStruct.Members.Single(x => x.Name == memberName);
                offset += member.Offset;

                if (!_primitives.Contains(member.Type))
                {
                    aphidStruct = ResolveType(member.Type);
                }
            }

            return offset;
        }
    }

}
