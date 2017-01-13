using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Variant
    {
        public string Text
        {
            get
            {
                if (Parent == null)
                    return "";
                if (Index % 2 == 1)
                    return Parent.Text + $" {1 + Index / 2}. " + MoveText;
                else
                    return Parent.Text + " " + MoveText;
            }
        }

        public int Index = 0;
        public int LeafsInSubtree()
        {
            if (Children.Count == 0)
                return 1;
            int total = 0;
            Children.ForEach(c => total += c.LeafsInSubtree());
            return total;
        }
        public string MoveText = "";
        public Variant Parent = null;
        public List<Variant> Children = new List<Variant>();

      
    }
}