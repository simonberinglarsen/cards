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

        public OpeningBook()
        {
            // read pgns
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

        public string[] GenerateCards()
        {
            Variant b = CreateBook();
            StringBuilder output = new StringBuilder();
            b.PrettyPrint(1, output);
            string text = output.ToString();
            string[] cards = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return cards.OrderBy(x => x).ToArray();
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