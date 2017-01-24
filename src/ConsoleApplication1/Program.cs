using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            operation = "SimulateDeck";
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
            else if (operation == "SimulateDeck")
            {
                SimulateDeck();
            }
           
        }
        

        private static void SimulateDeck()
        {
            Dictionary<string, int> deckconfig = new Dictionary<string, int>()
            {
                { "ABEF", 10 },
                { "CDGH", 10 },
                { "ABGH", 10 },
                { "CDEF", 10 },
                { "pawn", 2 },
                { "rook", 2 },
                { "knight", 2 },
                { "bishop", 2 },
                { "queen", 2 }
            };
            List<string> carddeck = new List<string>();
            foreach (var config in deckconfig)
            {
                for (int i = 0; i < config.Value; i++)
                    carddeck.Add(config.Key);
            }

            Random rnd = new Random();
            int wincounter = 0;
            int cardswin = 0;


            for (int j = 1; j < 10000; j++)
            {
                bool win = false;
                List<string> testdeck = new List<string>(carddeck);
                // challenge
                string file = "" + (char)(rnd.Next(8) + 'A');
                string piece = "" + new string[] { "pawn", "rook", "knight", "bishop", "queen" }[rnd.Next(5)];
                //draw 5 cards from deck
                List<string> hand = new List<string>();

                for (int i = 0; i < 6; i++)
                {
                    int indexToDraw = rnd.Next(testdeck.Count);
                    hand.Add(testdeck[indexToDraw]);
                    testdeck.RemoveAt(indexToDraw);

                    // check if its a win
                    if (hand.Last().Contains(file) || piece == hand.Last())
                    {
                        win = true;
                        cardswin++;
                    }
                }
                if (win) wincounter++;
                Debug.WriteLine($"unable-to-solve%:{1.0-wincounter / (double)j:P1}, cards-to-laydown:{cardswin / (double)j:F1} ");
            }
        }
    }
}
