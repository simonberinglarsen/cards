﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    internal class MateCardPrinter
    {
        private readonly TacticCard[] _cards;
        private const string directoryPath = "MateCards";
        private int cardno = 0;

        public MateCardPrinter(TacticCard[] cards)
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
            dir.EnumerateFiles("*.png").ToList().ForEach(x => x.Delete());
            string template = File.ReadAllText("_pro_cardtemplate.svg");
            cardno = 0;
            foreach (var card in _cards.Skip(42))
            {
                var diagram = DiagramFromMoves(card.Data.Fen);
                string newSvg = template
                    .Replace("+*+*+*+p", diagram.Substring(0, 8))
                    .Replace("*+*+*+*r", diagram.Substring(8, 8))
                    .Replace("+*+*+*+n", diagram.Substring(16, 8))
                    .Replace("*+*+*+*b", diagram.Substring(24, 8))
                    .Replace("+*+*+*+q", diagram.Substring(32, 8))
                    .Replace("*+*+*+*k", diagram.Substring(40, 8))
                    .Replace("+*+*+*pp", diagram.Substring(48, 8))
                    .Replace("*+*+*+pr", diagram.Substring(56, 8));
                Inkscape inkscape = new Inkscape(newSvg);
                if (!card.Data.WhiteToMove)
                {
                    inkscape.ReplaceTextInFlowPara("flowPara6049", "1");
                    inkscape.ReplaceTextInFlowPara("flowPara6051", "2");
                    inkscape.ReplaceTextInFlowPara("flowPara6053", "3");
                    inkscape.ReplaceTextInFlowPara("flowPara6055", "4");
                    inkscape.ReplaceTextInFlowPara("flowPara6057", "5");
                    inkscape.ReplaceTextInFlowPara("flowPara6059", "6");
                    inkscape.ReplaceTextInFlowPara("flowPara6061", "7");
                    inkscape.ReplaceTextInFlowPara("flowPara6063", "8");
                    inkscape.ReplaceTextInFlowPara("flowPara6079", "hgfedcba");
                    inkscape.ReplaceTextInFlowPara("flowPara4300", "Black to move...");
                    inkscape.ReplaceTextInFlowPara("flowPara4292", "Black to move...");
                }
                else
                {
                    inkscape.ReplaceTextInFlowPara("flowPara4300", "White to move...");
                    inkscape.ReplaceTextInFlowPara("flowPara4292", "White to move...");
                }
                // Title
                inkscape.ReplaceTextInFlowPara("flowPara4270", card.Title);
                inkscape.ReplaceTextInFlowPara("flowPara4278", card.Title);
                // Subtitle
                inkscape.ReplaceTextInFlowPara("flowPara4300", card.Subtitle);
                inkscape.ReplaceTextInFlowPara("flowPara4292", card.Subtitle);
                // challenge
                if (card.Data.WhiteToMove)
                    inkscape.ReplaceTextInFlowPara("flowPara5731", "Mat i 1. Hvid trækker.");
                else
                    inkscape.ReplaceTextInFlowPara("flowPara5731", "Mat i 1. Sort trækker.");
                // card number
                inkscape.ReplaceTextInFlowPara("flowPara4792", $"{cardno + 1,04:D}");
                // solution
                inkscape.ReplaceTextInFlowPara("flowPara5754", card.SolutionText);
                // corner text
                inkscape.ReplaceTextInFlowPara("flowPara4438", card.CornerText.Substring(0, 2));
                inkscape.ReplaceTextInFlowPara("flowPara4440", card.CornerText.Substring(2, 2));
                inkscape.ReplaceTextInFlowPara("flowPara4541", card.CornerText.Substring(0, 2));
                inkscape.ReplaceTextInFlowPara("flowPara4543", card.CornerText.Substring(2, 2));
                // dump svg
                File.WriteAllText(Path.Combine(directoryPath, $"card{cardno,0:D3}.svg"), inkscape.GetSvg());
                cardno++;
            }
            File.Copy("_pro_backside.svg", Path.Combine(directoryPath, "_pro_backside.svg"));
            Process p = new Process();
            string args = $@"/C ""for /r %i in (card*.svg;_pro_backside.svg) do ""{Inkscape.Path}"" %i -w 1650 -h 2250 -e %i.png""";
            p.StartInfo = new ProcessStartInfo("cmd.exe", args);
            p.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
            p.Start();
            p.WaitForExit();

            MakePrintPages();
        }

        private void MakePrintPages()
        {
            int cardIndexOnPage = 0;
            int page = 0;
            Graphics g = null;
            Bitmap newBmp = null;
            float dpi = 600;
            float pxLeftMargin = 0.0f * dpi;
            float pxTopMargin = 0.2f * dpi;
            float pxAlignMargin = 0.05f * dpi;
            float pxA4Height = 11.7f * dpi;
            float pxA4Width = 8.3f * dpi;
            var allFiles = Directory.GetFiles(directoryPath, "card*.png").OrderBy(x => x).ToList();
            int carsOnLastPage = allFiles.Count % 9;
            int fillLastPage = carsOnLastPage == 0 ? 0 : 9 - carsOnLastPage;
            for (int i = 0; i < 9 + fillLastPage; i++)
            {
                allFiles.Add($"{directoryPath}\\_pro_backside.svg.png");
            }

            foreach (var file in allFiles)
            {
                bool firstCardOnPage = cardIndexOnPage == 0;
                if (firstCardOnPage)
                {
                    // new page
                    newBmp = new Bitmap((int)pxA4Width, (int)pxA4Height);
                    newBmp.SetResolution(dpi, dpi);
                    g = Graphics.FromImage(newBmp);
                    g.FillRectangle(Brushes.White, 0, 0, pxA4Width, pxA4Height);

                    g.DrawLine(Pens.Black, 0, pxAlignMargin, pxA4Width, pxAlignMargin);
                    g.DrawLine(Pens.Black, pxAlignMargin, 0, pxAlignMargin, pxA4Height);
                    g.DrawLine(Pens.Black, 0, pxA4Height - pxAlignMargin, pxA4Width, pxA4Height - pxAlignMargin);
                    g.DrawLine(Pens.Black, pxA4Width - pxAlignMargin, 0, pxA4Width - pxAlignMargin, pxA4Height);
                }
                int row = cardIndexOnPage / 3;
                int col = cardIndexOnPage % 3;

                Bitmap bmp = new Bitmap(file);
                bmp.SetResolution(dpi, dpi);
                float cardWidth = bmp.Width;
                float cardHeight = bmp.Height;
                PointF cardpos = new PointF(pxLeftMargin + col * cardWidth, pxTopMargin + row * cardHeight);
                g.DrawImage(bmp, cardpos);

                bool lastCardOnPage = cardIndexOnPage % 9 == 8;
                if(lastCardOnPage)
                {
                    page++;
                    if (g != null)
                    {
                        // flush bitmap
                        newBmp.Save($"{directoryPath}\\page{page}.png", ImageFormat.Png);
                        g.Dispose();
                        newBmp.Dispose();
                    }
                }

                cardIndexOnPage = (cardIndexOnPage + 1) % 9;
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
