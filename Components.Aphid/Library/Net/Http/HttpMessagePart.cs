﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Aphid.Library.Net.Http
{
    public class HttpMessagePart
    {
        private Dictionary<string, List<string>> _fieldDictionary = new Dictionary<string, List<string>>();

        protected Dictionary<string, List<string>> FieldDictionary
        {
            get { return _fieldDictionary; }
        }

        public void ParseFields(string fields, bool skipFirstLine)
        {
            IEnumerable<string> f = fields.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (skipFirstLine)
            {
                f = f.Skip(1);
            }

            foreach (var v in f)
            {
                var fvp = v.Split(':');

                SetField(fvp[0], fvp[1].Trim());
            }
        }

        public void ClearField(string field)
        {
            if (_fieldDictionary.ContainsKey(field))
            {
                _fieldDictionary[field].Clear();
            }
        }

        public List<string> GetField(string field)
        {
            List<string> values;

            if (!_fieldDictionary.TryGetValue(field, out values))
            {
                values = new List<string>();
                _fieldDictionary.Add(field, values);

            }

            return values;
        }

        public bool HasField(string field)
        {
            return GetField(field).Any();
        }

        public void SetField(string field, string value)
        {
            GetField(field).Add(value);
        }

        public void SetField(string field, int value)
        {
            SetField(field, value.ToString());
        }

        public void SetField(string field, long value)
        {
            SetField(field, value.ToString());
        }

        public bool FieldContains(string field, string value)
        {
            return GetField(field).Contains(value);
        }

        public void ResetField(string field, string value)
        {
            ClearField(field);
            SetField(field, value);
        }

        public void ResetField(string field, int value)
        {
            ClearField(field);
            SetField(field, value);
        }

        public void ResetField(string field, long value)
        {
            ClearField(field);
            SetField(field, value);
        }

        public bool TryGetInt(string field, out int value)
        {
            value = 0;
            var values = GetField(field);

            if (values.Count != 0)
            {
                return int.TryParse(values[0], out value);
            }

            return false;
        }

        public string GetAttributeString()
        {
            return string.Join(
                "\r\n",
                FieldDictionary
                    .Select(x => x.Key + ": " + string.Join(",", x.Value)));
        }
    }
}

