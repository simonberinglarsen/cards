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
                {SolutionType.FileA, 1},
                {SolutionType.FileB, 1},
                {SolutionType.FileC, 1},
                {SolutionType.FileD, 1},
                {SolutionType.FileE, 1},
                {SolutionType.FileF, 1},
                {SolutionType.FileG, 1},
                {SolutionType.FileH, 1},
                {SolutionType.Rank1234, 8},
                {SolutionType.Rank5678, 8},
                {SolutionType.FileAbcd, 8},
                {SolutionType.FileEfgh, 8},
                {SolutionType.PieceIsPawn, 2},
                {SolutionType.PieceIsRook, 2},
                {SolutionType.PieceIsKnight, 2},
                {SolutionType.PieceIsBishop, 2},
                {SolutionType.PieceIsQueen, 2},
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
                finalDeck[i].Title = Translator.TitleFromCardno(i);
                finalDeck[i].Subtitle = Translator.SubtitleFromCardno(i);
            }
            _cardsInDeck = finalDeck.ToArray();
        }
    }
}