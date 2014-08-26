using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class ModelSimList : List<ModelSimListItem>
    {
        private static string[] ParseNames(string[] lines, int len)
        {
            return lines
                .Take(len)
                .Select(x => x
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Last())
                .ToArray();
        }

        private static int CountSignals(string[] lines)
        {
            return lines
                .Last()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .Count();
        }

        public static ModelSimList Parse(string text)
        {
            var lines = text.SplitLines(StringSplitOptions.RemoveEmptyEntries);
            var len = CountSignals(lines);
            var names = ParseNames(lines, len);
            
            var values = lines
                .Skip(len)
                .Select(x => ModelSimListItem.Parse(names, x));

            return new ModelSimList(values);
        }

        public ModelSimList()
        {
        }

        public ModelSimList(IEnumerable<ModelSimListItem> items)
            : base(items)
        {
        }
    }
}
