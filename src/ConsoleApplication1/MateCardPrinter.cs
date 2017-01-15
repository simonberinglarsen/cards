using System;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    internal class MateCardPrinter
    {
        private readonly Mate[] _cards;
        private const string directoryPath = "MateCards";
        private int cardno = 0;
        string inkscapePath = @"C:\Users\sends\Desktop\simon\toos\inkscape";

        public MateCardPrinter(Mate[] cards)
        {
            _cards = cards;
        }

        public void Print()
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var dir = new DirectoryInfo(directoryPath);
            dir.EnumerateFiles("*.svg").ToList().ForEach(x => x.Delete());
            dir.EnumerateFiles("*.pdf").ToList().ForEach(x => x.Delete());
            string template = File.ReadAllText("_pro_cardtemplate.svg");
            cardno = 0;
            foreach (var card in _cards)
            {
                var diagram = DiagramFromMoves(card.Fen);
                string newSvg = template
                    .Replace("+*+*+*+p", diagram.Substring(0, 8))
                    .Replace("*+*+*+*r", diagram.Substring(8, 8))
                    .Replace("+*+*+*+n", diagram.Substring(16, 8))
                    .Replace("*+*+*+*b", diagram.Substring(24, 8))
                    .Replace("+*+*+*+q", diagram.Substring(32, 8))
                    .Replace("*+*+*+*k", diagram.Substring(40, 8))
                    .Replace("+*+*+*pp", diagram.Substring(48, 8))
                    .Replace("*+*+*+pr", diagram.Substring(56, 8));
                if(!card.WhiteToMove)
                {
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6049", "1");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6051", "2");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6053", "3");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6055", "4");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6057", "5");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6059", "6");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6061", "7");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6063", "8");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara6079", "hgfedcba");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara4300", "Black to move...");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara4292", "Black to move...");
                }
                else
                {
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara4300", "White to move...");
                    newSvg = SvgManipulator.ReplaceText(newSvg, "flowPara4292", "White to move...");
                }
                File.WriteAllText(Path.Combine(directoryPath, $"card{cardno,0:D3}.svg"), newSvg);
                cardno++;
            }

        }


        private string DiagramFromMoves(string fen)
        {
            Engine e = new Engine();
            e.SetPositionByFen(fen);
            bool asWhite = e.ActiveColorIsWhite;
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
    }
}
