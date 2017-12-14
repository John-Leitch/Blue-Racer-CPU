using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class Table<TKey, TValue>
    {
        private Func<TValue> _createValue = null;

        private Dictionary<TKey, TValue> _table = new Dictionary<TKey, TValue>();

        public Table()
        {
        }

        public Table(Dictionary<TKey, TValue> dictionary)
        {
            _table = dictionary.ToDictionary(x => x.Key, x => x.Value);
        }

        public Table(Func<TValue> createValue)
        {
            _createValue = createValue;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue a;

                if (!_table.TryGetValue(key, out a))
                {
                    a = _createValue == null ? 
                        default(TValue) :
                        _createValue();

                    _table.Add(key, a);
                }

                return a;
            }
            set
            {
                if (!_table.ContainsKey(key))
                {
                    _table.Add(key, value);
                }
                else
                {
                    _table[key] = value;
                }
            }
        }

        public Dictionary<TKey, TValue> GetDictionary()
        {
            return _table;
        }
    }
}
