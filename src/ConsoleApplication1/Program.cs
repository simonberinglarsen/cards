using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                string inputfile;
                MateCardGenerator mateCardGenerator;
                MateCard[] mateCards;

                inputfile = @"matepgns\\ficsgamesdb_search_1434432.pgn";
                mateCardGenerator = new MateCardGenerator(inputfile);
                //mateCards = mateCardGenerator.GenerateMateIn(false);
                mateCards = mateCardGenerator.LoadCachedCards();
                mateCards = mateCardGenerator.PostProcess(mateCards);

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
