using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            OpeningBook book = new OpeningBook();
            string[] cards = book.GenerateCards();
            Cards c = new Cards(cards);
            c.Generate();
        }
    }
}
