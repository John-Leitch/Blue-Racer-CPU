using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class ModelSimListItem : Dictionary<string, string>
    {
        public int Time { get; set; }

        public int Delta { get; set; }

        public ModelSimListItem(int time, int delta)
            : this()
        {
            Time = time;
            Delta = delta;
        }

        public ModelSimListItem()
        {
        }

        public static ModelSimListItem Parse(string[] names, string text)
        {
            var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var items = parts
                .Skip(2)
                .Select((x, i) =>
                new
                {
                    Key = names[i],
                    Value = x,
                });

            var item = new ModelSimListItem(int.Parse(parts[0]), int.Parse(parts[1]));

            foreach (var i in items)
            {
                item.Add(i.Key, i.Value);
            }

            return item;
        }
    }
}
