using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class CliArgReader
    {
        public string[] Arguments { get; set; }

        public bool CaseSensitive { get; set; }

        public bool IgnoreUnknownArgs { get; set; }

        public CliArgReader(string[] arguments)
        {
            this.Arguments = arguments;
            CaseSensitive = false;
        }

        private Dictionary<string, Tuple<CliArgAttribute, PropertyInfo>> GetArgDictionary(object target)
        {
            var argDictionary = new Dictionary<string, Tuple<CliArgAttribute, PropertyInfo>>();

            var type = target.GetType();

            foreach (var p in type.GetProperties())
            {
                var a = p.GetCustomAttributes(typeof(CliArgAttribute), true);

                if (!a.Any())
                {
                    continue;
                }

                var attr = a.First() as CliArgAttribute;

                var name = attr.Name;

                if (!CaseSensitive)
                {
                    name = name.ToLower();
                }

                if (argDictionary.ContainsKey(name))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Duplicate argument {0}.",
                            name
                        ));
                }

                argDictionary.Add(name, Tuple.Create(attr, p));
            }

            return argDictionary;
        }

        private void ThrowArgumentException(string Message, params object[] Args)
        {
            throw new ArgumentException(string.Format(Message, Args));
        }

        public void SetProperties(object target)
        {
            var argDictionary = GetArgDictionary(target);

            for (int i = 0; i < Arguments.Length; i++)
            {
                var name = Arguments[i];

                if (!CaseSensitive)
                {
                    name = name.ToLower();
                }

                if (!argDictionary.ContainsKey(name))
                {
                    if (IgnoreUnknownArgs)
                    {
                        continue;
                    }
                    else
                    {
                        ThrowArgumentException("Invalid argument {0}.", name);
                    }
                }

                var arg = argDictionary[name];
                var propType = arg.Item2.PropertyType;

                Func<object> getValue = null;

                if (propType == typeof(bool))
                {
                    getValue = () => true;
                }
                else if (++i == Arguments.Length)
                {
                    ThrowArgumentException("No value for argument {0}.", name);
                }
                else if (propType == typeof(string))
                {
                    getValue = () => Arguments[i];
                }
                else if (propType == typeof(int))
                {
                    getValue = () => int.Parse(Arguments[i]);
                }
                else if (propType == typeof(FileInfo))
                {
                    getValue = () =>
                    {
                        var f = new FileInfo(Arguments[i]);

                        if (arg.Item1.IsInputFile && !f.Exists)
                        {
                            throw new ArgumentException(string.Format("Could not find file {0}.", f.FullName));
                        }

                        return f;
                    };
                }
                else
                {
                    ThrowArgumentException("Invalid property type {0}.", propType);
                }

                argDictionary[name].Item2.SetValue(target, getValue(), null);
            }
        }

        public TArgs Read<TArgs>()
            where TArgs : new()
        {
            var args = new TArgs();
            SetProperties(args);

            return args;
        }
    }
}
