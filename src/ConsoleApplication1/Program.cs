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
                // generate cards
                bool loadFromCache = true;
                string inputfile = @"matepgns\\ficsgamesdb_search_1434432.pgn";
                var mateCardGenerator = new MateCardGenerator(inputfile);
                var tacticCards = loadFromCache ? mateCardGenerator.LoadCachedCards() : mateCardGenerator.Generate(false);

                // collect cards into deck and postprocess (update card data)
                Deck deck = new Deck(tacticCards);
                deck.PostProcess();

                // print cards (update svg / card layout)
                MateCardPrinter mcp = new MateCardPrinter(deck.Cards);
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
