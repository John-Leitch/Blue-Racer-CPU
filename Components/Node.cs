using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class Node<TValue>
    {
        public TValue Value { get; set; }

        public TValue Parent { get; set; }

        public List<Node<TValue>> Children { get; set; }

        public Node()
        {
        }

        public Node(TValue value)
            : this()
        {
        }

        public static implicit operator TValue(Node<TValue> node)
        {
            return node.Value;
        }

        public void AddChild(Node<TValue> node)
        {
            Children.Add(node);
            node.Parent = this;
        }
    }
}
