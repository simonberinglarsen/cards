using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists("Cards"))
            {
                Directory.Delete("Cards", true);
                while (Directory.Exists("Cards"))
                {
                    Thread.Sleep(2000);
                }
            }
            Directory.CreateDirectory("Cards");
            while (!Directory.Exists("Cards"))
            {
                Thread.Sleep(2000);
            }
            string allPgns = File.ReadAllText(@"pgndownload.pgn");
            // chop it
            string[] pgns = ChopIt(allPgns);
            List<PgnParser> parsed = new List<PgnParser>();
            foreach (var pgn in pgns)
            {
                PgnParser parser = new PgnParser(pgn);
                parser.Parse();
                parsed.Add(parser);

            }
            var list = parsed.Select(x => x.Game).ToList();
            Variant b = CreateBook(list);
            StringBuilder output = new StringBuilder();
            b.PrettyPrint(1, output);
            string text = output.ToString();
            string[] cards = text.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            string template = File.ReadAllText("card_template.svg");
            int cardno = 0;
            string diagram = 
                      "tMvWlVmT"
                    + "OoOoOoOo"
                    + " + + + +"
                    + "+ + + + "
                    + " + + + +"
                    + "+ + + + "
                    + "pPpPpPpP"
                    + "RnBqKbNr";
            foreach (var card in cards)
            {
                string[] e = card.Split(new char[] {';'});
                string newSvg = template
                    .Replace("£", e[0])
                    .Replace("§", e[1])
                    .Replace("##MAINVARIANT##", e[2])
                    .Replace("##NEXTMOVES##", e[3])
                    .Replace("##DIAGRAMLINE1##", e[4].Substring(0, 8))
                    .Replace("##DIAGRAMLINE2##", e[4].Substring(8, 8))
                    .Replace("##DIAGRAMLINE3##", e[4].Substring(16, 8))
                    .Replace("##DIAGRAMLINE4##", e[4].Substring(24, 8))
                    .Replace("##DIAGRAMLINE5##", e[4].Substring(32, 8))
                    .Replace("##DIAGRAMLINE6##", e[4].Substring(40, 8))
                    .Replace("##DIAGRAMLINE7##", e[4].Substring(48, 8))
                    .Replace("##DIAGRAMLINE8##", e[4].Substring(56, 8));

                File.WriteAllText($"Cards\\card{cardno,0:D3}.svg", newSvg);
                cardno++;
            }
            // create fullpages
            string fullpage_template = File.ReadAllText("fullpage_template.svg");
            for (int i = 0; i < cardno; i += 3)
            {
                string cardfile1 = $"Cards\\card{i,0:D3}.svg";
                string cardfile2 = $"Cards\\card{i+1,0:D3}.svg";
                string cardfile3 = $"Cards\\card{i+2,0:D3}.svg";
                string cardbackfile = "cardback_template.svg";
                string card1Text = File.Exists(cardfile1) ? GetCardGroup(cardfile1) : "";
                string card2Text = File.Exists(cardfile2) ? GetCardGroup(cardfile2) : "";
                string card3Text = File.Exists(cardfile3) ? GetCardGroup(cardfile3) : "";

                string cardbackText = GetCardGroup(cardbackfile);
                string newSvg = fullpage_template
                    .Replace("<!-- ##CARD1## -->", card1Text)
                    .Replace("<!-- ##CARD2## -->", card2Text)
                    .Replace("<!-- ##CARD3## -->", card3Text)
                    .Replace("<!-- ##CARDBACKX## -->", cardbackText);
                File.WriteAllText($"Cards\\fullpage_{i,0:D3}_{i+2,0:D3}.svg", newSvg);
            }
        }

        private static string GetCardGroup(string cardfile1)
        {
            string s = File.ReadAllText(cardfile1);
            if (string.IsNullOrWhiteSpace(s))
                return "";
            s = s.Substring(s.IndexOf("<g"));
            s = s.Replace("</svg>", "");
            return s;
        }

        private static Variant CreateBook(List<string> list)
        {
            Variant b = new Variant() { MoveText = "ROOT", Parent = null, Index = 0 };
            foreach (var variant in list)
            {
                string[] halfmoves = variant.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> keys = new List<string>();
                for (int i = 0; i < halfmoves.Length; i++)
                {
                    if (i % 3 == 0)
                        continue;
                    keys.Add(halfmoves[i]);
                }
                var currentVar = b;
                foreach (var key in keys.Take(20))
                {
                    var x = currentVar.Children.SingleOrDefault(v => v.MoveText == key);
                    if (x == null)
                    {
                        x = new Variant() { MoveText = key, Parent = currentVar, Index = currentVar.Index + 1 };
                        currentVar.Children.Add(x);
                    }
                    currentVar = x;
                }
            }
            return b;
        }

        private static string[] ChopIt(string allPgns)
        {
            List<string> pgns = new List<string>();

            int start = 0;
            while (start >= 0)
            {
                int temp = allPgns.IndexOf("]\n\n1. ", start);
                if (temp == -1) break;
                int end = allPgns.IndexOf("[", temp);
                string pgn;
                if (end == -1)
                    pgn = allPgns.Substring(start);
                else
                    pgn = allPgns.Substring(start, end - start);
                pgns.Add(pgn);
                start = end;
            }

            return pgns.ToArray();
        }
    }

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
