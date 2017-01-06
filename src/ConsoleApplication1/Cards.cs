using System.IO;
using System.Linq;
using System.Threading;

namespace ConsoleApplication1
{
    class Cards
    {
        private readonly string[] _cards;

        public Cards(string[] cards)
        {
            this._cards = cards;
        }

        public void Generate()
        {
            string directoryPath = "Cards";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var dir = new DirectoryInfo(directoryPath);
            dir.EnumerateFiles("*.svg").ToList().ForEach(x => x.Delete());
            dir.EnumerateFiles("*.pdf").ToList().ForEach(x => x.Delete());

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
            foreach (var card in _cards)
            {
                string[] e = card.Split(new char[] { ';' });
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
            // create fullpages w. 3 cards
            string fullpage_template = File.ReadAllText("3fullpage_template.svg");
            for (int i = 0; i < cardno; i += 3)
            {
                string cardfile1 = $"Cards\\card{i,0:D3}.svg";
                string cardfile2 = $"Cards\\card{i + 1,0:D3}.svg";
                string cardfile3 = $"Cards\\card{i + 2,0:D3}.svg";
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
                File.WriteAllText($"Cards\\3fullpage_{i,0:D3}_{i + 2,0:D3}.svg", newSvg);
            }
            // create fullpages w. 9 cards
            fullpage_template = File.ReadAllText("9fullpage_template.svg");
            int cardsPrSheet = 9;
            int sheets = cardno / cardsPrSheet;
            if (cardno % cardsPrSheet != 0)
                sheets++;
            // one sheet of backsides
            sheets++;
            for (int i = 0; i < sheets; i++)
            {
                string newSvg = fullpage_template;
                bool lastSheet = i == sheets - 1;
                for (int j = 0; j < cardsPrSheet; j++)
                {
                    string cardfile = $"Cards\\card{i * cardsPrSheet + j,0:D3}.svg";
                    if (lastSheet)
                        cardfile = "cardback_template.svg";
                    string cardText = File.Exists(cardfile) ? GetCardGroup(cardfile) : "";
                    newSvg = newSvg.Replace($"<!-- ##CARD{j}## -->", cardText);
                }
                File.WriteAllText($"Cards\\9fullpage_{i * cardsPrSheet,0:D3}_{i * cardsPrSheet + cardsPrSheet - 1,0:D3}.svg", newSvg);
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
}