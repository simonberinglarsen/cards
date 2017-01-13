using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System;

namespace ConsoleApplication1
{
    class BookCard
    {
        public string MainVariant { get; set; }
        public string NextMoves { get; set; }
        public string OpeningName { get; set; }
        public string Diagram { get; set; }
        public string Depth { get; set; }
        public string NodeType { get; set; }
    }
    class CardPrinter
    {
        private readonly BookCard[] _cards;
        private const string directoryPath = "Cards";
        private int cardno = 0;
        string inkscapePath = @"C:\Users\sends\Desktop\simon\toos\inkscape";
        public CardPrinter(BookCard[] cards)
        {
            this._cards = cards;
        }

        private string DisableId(string x, string id)
        {
            using (StringReader sr = new StringReader(x))
            {
                XDocument doc = XDocument.Load(sr);
                XElement root = doc.Root;
                var xmlns = "{" + root.GetDefaultNamespace().NamespaceName + "}";
                var gElement = root.Descendants(xmlns + "g").Attributes().Where(a => a.Name == "id" && a.Value == id).Select(z => z.Parent).Single();
                var attribute = gElement.Attributes().Where(a => a.Name == "style").SingleOrDefault();
                if (attribute == null)
                {
                    attribute = new XAttribute("style", "");
                    gElement.Add(attribute);
                }
                attribute.SetValue("opacity:0");
                return doc.ToStringWithDeclaration();
            }
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
            string template = File.ReadAllText("_template_card.svg");
            cardno = 0;
            foreach (var card in _cards)
            {
                string newSvg = template
                    .Replace("##MAINVARIANT##", card.MainVariant)
                    .Replace("##NEXTMOVES##", card.NextMoves)
                    .Replace("##OPENINGNAME##", card.OpeningName)
                    .Replace("+*+*+*+p", card.Diagram.Substring(0, 8))
                    .Replace("*+*+*+*r", card.Diagram.Substring(8, 8))
                    .Replace("+*+*+*+n", card.Diagram.Substring(16, 8))
                    .Replace("*+*+*+*b", card.Diagram.Substring(24, 8))
                    .Replace("+*+*+*+q", card.Diagram.Substring(32, 8))
                    .Replace("*+*+*+*k", card.Diagram.Substring(40, 8))
                    .Replace("+*+*+*pp", card.Diagram.Substring(48, 8))
                    .Replace("*+*+*+pr", card.Diagram.Substring(56, 8));
                int depth = int.Parse(card.Depth);
                newSvg = newSvg
                    .Replace("''1", $"{depth}")
                    .Replace("''2", $"{cardno + 1,0:D3}");
                if (card.NodeType != "LEAF")
                    newSvg = DisableId(newSvg, "g6312");
                else
                    newSvg = DisableId(newSvg, "g4637");
                File.WriteAllText(Path.Combine(directoryPath, $"card{cardno,0:D3}.svg"), newSvg);
                cardno++;
            }
            Print3CardsWithBacksideOnA4();
            Print2CardsNoBacksideOn4InchPhoto();
            Print9CardsOnA4BacksidesSeperate();
        }

        private void Print9CardsOnA4BacksidesSeperate()
        {
            // create fullpages w. 9 cards
            string fullpage_template = File.ReadAllText("_template_9fullpage.svg");
            int cardsPrSheet = 9;
            int sheets = cardno / cardsPrSheet;
            if (cardno % cardsPrSheet != 0)
                sheets++;
            // one sheet of backsides for each sheet of frontsheet
            sheets++;
            for (int i = 0; i < sheets; i++)
            {
                string newSvg = fullpage_template;
                bool lastSheet = i == sheets - 1;
                string outputfilename = Path.Combine(directoryPath, $"9fullpage_{i * cardsPrSheet,0:D3}_{i * cardsPrSheet + cardsPrSheet - 1,0:D3}.svg");
                for (int j = 0; j < cardsPrSheet; j++)
                {
                    string cardfile = Path.Combine(directoryPath, $"card{i * cardsPrSheet + j,0:D3}.svg");
                    if (lastSheet) cardfile = "_template_cardback.svg";

                    string cardText = File.Exists(cardfile) ? GetCardGroup(cardfile) : "";
                    newSvg = newSvg.Replace($"<!-- ##CARD{j}## -->", cardText);
                }
                if (lastSheet) outputfilename = Path.Combine(directoryPath, "_template_cardback.svg");

                File.WriteAllText(outputfilename, newSvg);
            }
            // convert to pdf
            Process p = new Process();
            string args = $@"/C ""for /r %i in (9*.svg;_template_cardback.svg) do ""{inkscapePath}\inkscape.exe"" %i -A %i.pdf""";
            p.StartInfo = new ProcessStartInfo("cmd.exe", args);
            p.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
            p.Start();
            p.WaitForExit();

            // make one big pdf with all backsides and frontsides
            var all = Directory.GetFiles(directoryPath, "9*.pdf").SelectMany(x => new string[] { x, Path.Combine(directoryPath, "_template_cardback.svg.pdf") }).ToArray();
            MergePDFs("Cards\\9all.pdf", all.OrderBy(x => x).ToArray());
        }

        private void Print2CardsNoBacksideOn4InchPhoto()
        {
            // create fullpages w. 2 cards
            string fullpage_template = File.ReadAllText("pixum10x15.svg");
            for (int i = 0; i < cardno; i += 2)
            {
                string cardfile1 = Path.Combine(directoryPath, $"card{i,0:D3}.svg");
                string cardfile2 = Path.Combine(directoryPath, $"card{i + 1,0:D3}.svg");
                string cardbackfile = "_template_card.svg";
                string card1Text = File.Exists(cardfile1) ? GetCardGroup(cardfile1) : "";
                string card2Text = File.Exists(cardfile2) ? GetCardGroup(cardfile2) : "";

                string cardbackText = GetCardGroup(cardbackfile);
                string newSvg = fullpage_template
                    .Replace("<!-- ##CARD1## -->", card1Text)
                    .Replace("<!-- ##CARD2## -->", card2Text);
                File.WriteAllText(Path.Combine(directoryPath, $"2fullpage_{i,0:D3}_{i + 1,0:D3}.svg"), newSvg);
            }

            // convert to png
            Process p = new Process();
            string args = $@"/C ""for /r %i in (2*.svg;) do ""{inkscapePath}\inkscape.exe"" %i -w 1800 -h 1200 -e %i.png""";
            p.StartInfo = new ProcessStartInfo("cmd.exe", args);
            p.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
            p.Start();
            p.WaitForExit();
        }

        private void Print3CardsWithBacksideOnA4()
        {
            // create fullpages w. 3 cards
            string fullpage_template = File.ReadAllText("_template_3fullpage.svg");
            for (int i = 0; i < cardno; i += 3)
            {
                string cardfile1 = Path.Combine(directoryPath, $"card{i,0:D3}.svg");
                string cardfile2 = Path.Combine(directoryPath, $"card{i + 1,0:D3}.svg");
                string cardfile3 = Path.Combine(directoryPath, $"card{i + 2,0:D3}.svg");
                string cardbackfile = "_template_card.svg";
                string card1Text = File.Exists(cardfile1) ? GetCardGroup(cardfile1) : "";
                string card2Text = File.Exists(cardfile2) ? GetCardGroup(cardfile2) : "";
                string card3Text = File.Exists(cardfile3) ? GetCardGroup(cardfile3) : "";

                string cardbackText = GetCardGroup(cardbackfile);
                string newSvg = fullpage_template
                    .Replace("<!-- ##CARD1## -->", card1Text)
                    .Replace("<!-- ##CARD2## -->", card2Text)
                    .Replace("<!-- ##CARD3## -->", card3Text)
                    .Replace("<!-- ##CARDBACKX## -->", cardbackText);
                File.WriteAllText(Path.Combine(directoryPath, $"3fullpage_{i,0:D3}_{i + 2,0:D3}.svg"), newSvg);
            }
        }

        public static void MergePDFs(string targetPath, params string[] pdfs)
        {
            using (PdfDocument targetDoc = new PdfDocument())
            {
                foreach (string pdf in pdfs)
                {
                    using (PdfDocument pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < pdfDoc.PageCount; i++)
                        {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }
                targetDoc.Save(targetPath);
            }
        }

        private string GetCardGroup(string cardfile1)
        {
            string s = File.ReadAllText(cardfile1);
            if (string.IsNullOrWhiteSpace(s))
                return "";
            s = s.Substring(s.IndexOf("<g"));
            s = s.Replace("</svg>", "");
            return s;
        }
    }

    public static class XDocExtensions
    {
        public static string ToStringWithDeclaration(this XDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }
    }
}