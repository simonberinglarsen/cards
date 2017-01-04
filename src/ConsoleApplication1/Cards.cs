using System.IO;
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
            // create fullpages
            string fullpage_template = File.ReadAllText("fullpage_template.svg");
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
                File.WriteAllText($"Cards\\fullpage_{i,0:D3}_{i + 2,0:D3}.svg", newSvg);
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