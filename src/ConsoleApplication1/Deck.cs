using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    internal class Deck
    {
        private TacticCard[] _cardsInDeck = new TacticCard[0];
        public Deck(TacticCard[] tacticCards)
        {
            _cardsInDeck = tacticCards;
        }

        public TacticCard[] Cards
        {
            get
            {
                return _cardsInDeck;
            }
        }


        public void PostProcess()
        {
            List<TacticCard> finalDeck = new List<TacticCard>();
            List<TacticCard> hugeDeck = new List<TacticCard>(_cardsInDeck.Where(x => x.Data.FullMovesToMate == 1));

            var pw = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'P' && x.Data.WhiteToMove).Take(5).ToArray();
            var pb = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'P' && !x.Data.WhiteToMove).Take(5).ToArray();
            var rw = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'R' && x.Data.WhiteToMove).Take(5).ToArray();
            var rb = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'R' && !x.Data.WhiteToMove).Take(5).ToArray();
            var nw = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'N' && x.Data.WhiteToMove).Take(5).ToArray();
            var nb = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'N' && !x.Data.WhiteToMove).Take(5).ToArray();
            var bw = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'B' && x.Data.WhiteToMove).Take(5).ToArray();
            var bb = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'B' && !x.Data.WhiteToMove).Take(5).ToArray();
            var qw = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'Q' && x.Data.WhiteToMove).Take(5).ToArray();
            var qb = hugeDeck.Where(x => x.Data.WinningPieceUpper == 'Q' && !x.Data.WhiteToMove).Take(5).ToArray();

            finalDeck.AddRange(pw);
            finalDeck.AddRange(rw);
            finalDeck.AddRange(nw);
            finalDeck.AddRange(bw);
            finalDeck.AddRange(qw);
            finalDeck.AddRange(pb);
            finalDeck.AddRange(rb);
            finalDeck.AddRange(nb);
            finalDeck.AddRange(bb);
            finalDeck.AddRange(qb);


            Dictionary<SolutionType, int> setup = new Dictionary<SolutionType, int>()
            {
                {SolutionType.FileAll, 2},
                {SolutionType.FileABCD, 8},
                {SolutionType.FileABEF, 8},
                {SolutionType.FileABGH, 8},
                {SolutionType.FileCDEF, 8},
                {SolutionType.FileCDGH, 8},
                {SolutionType.FileEFGH, 8},
            };
            // verify 
            var sum = setup.Sum(s => s.Value);
            if (sum != 50) throw new Exception("Wrong deck size!");
            if (finalDeck.Count != 50) throw new Exception("Wrong deck size!");
            List<SolutionType> solutionTypes = new List<SolutionType>();
            foreach (var keyPair in setup)
            {
                for (int i = 0; i < keyPair.Value; i++)
                {
                    solutionTypes.Add(keyPair.Key);
                }
            }

            // add titles
            for (int i = 0; i < 50; i++)
            {
                finalDeck[i].Data.Solution = solutionTypes[i];
                finalDeck[i].SolutionText = Translator.SolutionTypeToText(solutionTypes[i]);
                finalDeck[i].CornerText = Translator.SolutionTypeToCornerText(solutionTypes[i]);
                finalDeck[i].Title = Translator.TitleFromCardno(i);
                finalDeck[i].Subtitle = Translator.SubtitleFromCardno(i);
                finalDeck[i].PuzzleText = Translator.PuzzleTextFromWhiteToMove(finalDeck[i].Data.WhiteToMove);
            }
            _cardsInDeck = finalDeck.ToArray();
        }
    }
}