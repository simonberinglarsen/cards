using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string operation;
            operation = "FindMate";
            //operation = "OpeningBook";
            if (operation == "FindMate")
            {
                MateFinder mf = new MateFinder("matepgns\\20170114_mate.pgn");
                var mateCards = mf.FindMateIn(1);
                MateCardPrinter mcp = new MateCardPrinter(mateCards);
                mcp.Print();
            }
            else if (operation == "OpeningBook")
            {
                OpeningBook book = new OpeningBook(@"scandinaviandefence.pgn", "Scandinavian Defence");
                BookCard[] cards = book.GenerateCardsForBlack();
                CardPrinter c = new CardPrinter(cards);
                c.Print();
            }
        }
    }
}
