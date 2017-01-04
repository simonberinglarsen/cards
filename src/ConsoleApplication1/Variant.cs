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

        public string MoveText = "";
        public Variant Parent = null;
        public List<Variant> Children = new List<Variant>();

        public void PrettyPrint(int depth, StringBuilder output)
        {
            if (Children.Count == 1)
            {
                Children.ForEach(v => v.PrettyPrint(depth, output));
            }
            else if (Children.Count > 1)
            {
                output.Append($"A;{depth};" + Text.Trim() + ";");
                string pre = "";
                int moveno = 1 + Index / 2;
                if (Index % 2 == 1)
                    pre = $"{moveno}...";
                else
                    pre = $"{moveno}. ";
                output.Append(pre + String.Join("/", Children.Select(x => x.MoveText)));
                string fen = DiagramFromMoves();
                output.AppendLine(";"+fen);
                Children.ForEach(v => v.PrettyPrint(depth + 1, output));
            }
            else if (Children.Count == 0)
            {
                Variant parent = Parent;
                while (parent.Children.Count == 1)
                {
                    parent = parent.Parent;
                }
                string pre = "";
                int moveno = 1 + parent.Index / 2;
                if (parent.Index % 2 == 1)
                    pre = $"{moveno}...";
                string fen = DiagramFromMoves();
                output.AppendLine($"B;{depth};" + parent.Text.Trim() + ";" +pre + Text.Substring(parent.Text.Length).Trim() + ";"+fen);
            }
        }

        private string DiagramFromMoves()
        {
            Variant v = this;
            string moves = "";
            while (v.Parent != null)
            {
                moves = v.MoveText + " " + moves;
                v = v.Parent;
            }
            var allmoves = moves.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            Engine e = new Engine();
            for (int i = 0; i < allmoves.Length; i++)
            {
                string san = allmoves[i].Replace("+", "");
                var genmoves = e.GenerateMoves();
                var gensan = e.PrintAsSan(genmoves);
                int indexofsan = gensan.ToList().IndexOf(san);
                var move = genmoves[indexofsan];
                e.DoMove(move);
            }
            string board = e.PrintBoard();
            board = board
                .Replace("p", "#p")
                .Replace("r", "#r")
                .Replace("n", "#n")
                .Replace("b", "#b")
                .Replace("q", "#q")
                .Replace("k", "#k")
                .Replace("P", "#P")
                .Replace("R", "#R")
                .Replace("N", "#N")
                .Replace("B", "#B")
                .Replace("Q", "#Q")
                .Replace("K", "#K");
            board = board
                .Replace("#P", "p")
                .Replace("#p", "o")
                .Replace("#R", "r")
                .Replace("#r", "t")
                .Replace("#N", "n")
                .Replace("#n", "m")
                .Replace("#B", "b")
                .Replace("#b", "v")
                .Replace("#Q", "q")
                .Replace("#q", "w")
                .Replace("#K", "k")
                .Replace("#k", "l");

            var temp = board.Replace("\r", "").Replace("\n", "").ToCharArray();
            for (int i = 0; i < temp.Length; i++)
            {
                if ((i/8 + i%8)%2 == 1)
                {
                    temp[i] = char.ToUpper(temp[i]);
                    if (temp[i] == ' ')
                        temp[i] = '+';
                }
            }
            var flipped = new char[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                flipped[temp.Length - 1 - i] = temp[i];
            }
            temp = flipped;

            return new string(temp);
        }
    }
}