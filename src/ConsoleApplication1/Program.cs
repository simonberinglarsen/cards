using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string operation;
            operation = "OpeningBook";
            operation = "FindMate";
            if (operation == "FindMate")
            {
                MateCardGenerator mateCardGenerator = new MateCardGenerator("matepgns\\20170114_mate.pgn");
                var mateCards = mateCardGenerator.Generate();
                mateCardGenerator.PostProcess(mateCards);

                MateCardPrinter mcp = new MateCardPrinter(mateCards);
                mcp.Print();
            }
            else if (operation == "OpeningBook")
            {
                OpeningCardGenerator openingCardGenerator = new OpeningCardGenerator(@"openingpgns\scandinaviandefence.pgn", "Scandinavian Defence");
                OpeningCard[] cards = openingCardGenerator.GenerateForBlack();
                OpeningCardPrinter c = new OpeningCardPrinter(cards);
                c.Print();
            }
        }
    }
}
