using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            OpeningBook book = new OpeningBook(@"scandinaviandefence.pgn", "Scandinavian Defence");
            BookCard[] cards = book.GenerateCardsForBlack();
            CardPrinter c = new CardPrinter(cards);
            c.Print();
        }
    }
}
