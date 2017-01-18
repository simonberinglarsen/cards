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
                inputfile = @"matepgns\\ficsgamesdb_search_1434325.pgn";
                MateCardGenerator mateCardGenerator = new MateCardGenerator(inputfile);
                var mateCards = mateCardGenerator.GenerateMateIn(true);
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
