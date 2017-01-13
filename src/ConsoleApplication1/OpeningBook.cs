using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class OpeningBook
    {
        private readonly List<string> _pgnList;
        private string _openingName;

        public OpeningBook(string pgnBook, string openingName)
        {
            _openingName = openingName;
            // read pgns
            string allPgns = File.ReadAllText(pgnBook);
            // chop it
            string[] pgns = ChopIt(allPgns);
            List<PgnParser> parsed = new List<PgnParser>();
            foreach (var pgn in pgns)
            {
                PgnParser parser = new PgnParser(pgn);
                parser.Parse();
                parsed.Add(parser);

            }
            _pgnList = parsed.Select(x => x.Game).ToList();
        }

        private static string[] ChopIt(string allPgns)
        {
            List<string> pgns = new List<string>();

            int start = 0;
            while (start >= 0)
            {
                int temp = allPgns.IndexOf("]\r\n\r\n1. ", start);
                if (temp == -1)
                {
                    temp = allPgns.IndexOf("]\n\n1. ", start);
                    if (temp == -1) break;
                }
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

        public BookCard[] GenerateCardsForBlack()
        {
            return GenerateCards(false);
        }

        public BookCard[] GenerateCardsForWhite()
        {
            return GenerateCards(true);
        }

        private BookCard[] GenerateCards(bool asWhite)
        {
            Variant bookRoot = CreateBook();
            List<BookCard> cards = new List<BookCard>();
            PrettyPrint(bookRoot, 1, cards, asWhite);
            return cards.OrderBy(x => x.MainVariant).ToArray();
        }

        public void PrettyPrint(Variant current, int depth, List<BookCard> output, bool asWhite)
        {
            if (current.Children.Count == 1)
            {
                current.Children.ForEach(v => PrettyPrint(v, depth, output, asWhite));
            }
            else if (current.Children.Count > 1)
            {
                string pre = "";
                int moveno = 1 + current.Index / 2;
                if (current.Index % 2 == 1)
                    pre = $"{moveno}...";
                else
                    pre = $"{moveno}. ";
                string fen = DiagramFromMoves(current, asWhite);
                BookCard newcard = new BookCard()
                {
                    OpeningName = _openingName,
                    Depth = $"{depth:0000}",
                    NodeType = "VARIANT",
                    MainVariant = current.Text.Trim(),
                    NextMoves = pre + String.Join(" / ", current.Children.Select(x => x.MoveText + $"[{x.LeafsInSubtree()}]")),
                    Diagram = fen

                };
                output.Add(newcard);

                current.Children.ForEach(v => PrettyPrint(v, depth + 1, output, asWhite));
            }
            else if (current.Children.Count == 0)
            {
                Variant parent = current.Parent;
                while (parent.Children.Count == 1)
                {
                    parent = parent.Parent;
                }
                string pre = "";
                int moveno = 1 + parent.Index / 2;
                if (parent.Index % 2 == 1)
                    pre = $"{moveno}...";
                string fen = DiagramFromMoves(current, asWhite);

                BookCard newcard = new BookCard()
                {
                    OpeningName = _openingName,
                    Depth = $"{depth:0000}",
                    NodeType = "LEAF",
                    MainVariant = parent.Text.Trim(),
                    NextMoves = pre + current.Text.Substring(parent.Text.Length).Trim(),
                    Diagram = fen
                };
                output.Add(newcard);
            }
        }

        private string DiagramFromMoves(Variant v, bool asWhite)
        {
            string moves = "";
            while (v.Parent != null)
            {
                moves = v.MoveText + " " + moves;
                v = v.Parent;
            }
            var allmoves = moves.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Engine e = new Engine();
            for (int i = 0; i < allmoves.Length; i++)
            {
                string san = allmoves[i].Replace("+", "");
                if (san.IndexOf("1/2-1/2") >= 0
                   || san.IndexOf("1-0") >= 0
                    || san.IndexOf("0-1") >= 0)
                {
                    // gameover
                    break;
                }
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
                if ((i / 8 + i % 8) % 2 == 1)
                {
                    temp[i] = char.ToUpper(temp[i]);
                    if (temp[i] == ' ')
                        temp[i] = '+';
                }
            }
            if (!asWhite)
            {
                var flipped = new char[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    flipped[temp.Length - 1 - i] = temp[i];
                }
                temp = flipped;
            }
            return new string(temp);
        }

        private Variant CreateBook()
        {
            Variant b = new Variant() { MoveText = "ROOT", Parent = null, Index = 0 };
            foreach (var variant in _pgnList)
            {
                string[] pgnHalfMoves = variant.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> allPgnHalfMoves = new List<string>();
                for (int i = 0; i < pgnHalfMoves.Length; i++)
                {
                    if (i % 3 == 0)
                        continue;
                    allPgnHalfMoves.Add(pgnHalfMoves[i]);
                }
                var currentVar = b;
                var openingHalfMoves = allPgnHalfMoves.Take(20).ToList();
                for (int i = 0; i < openingHalfMoves.Count; i++)
                {
                    var openingHalfMove = openingHalfMoves[i];
                    var x = currentVar.Children.SingleOrDefault(v => v.MoveText == openingHalfMove);
                    if (x == null)
                    {
                        x = new Variant() { MoveText = openingHalfMove, Parent = currentVar, Index = currentVar.Index + 1 };
                        currentVar.Children.Add(x);
                    }
                    currentVar = x;

                }
            }
            return b;
        }
    }
}