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

            hugeDeck.Where(x => x.Data.WinningPieceUpper == 'P' && x.Data.WinningMoveSan != null && x.Data.WinningMoveSan.Contains("=")).Take(1).ToList().ForEach(
                x =>
                {
                    hugeDeck.Remove(x);
                    finalDeck.Add(x);
                });
            int added;
            foreach (var file in "abcdefgh".ToCharArray())
            {
                added = finalDeck.Count;
                TransferToFinalDeck(file, 'Q', finalDeck, hugeDeck);
                TransferToFinalDeck(file, 'P', finalDeck, hugeDeck);
                TransferToFinalDeck(file, 'R', finalDeck, hugeDeck);
                TransferToFinalDeck(file, 'N', finalDeck, hugeDeck);
                TransferToFinalDeck(file, 'B', finalDeck, hugeDeck);
                added = finalDeck.Count - added;
                TransferAnyToFinalDeck(file, 6-added, finalDeck, hugeDeck);
            }

            hugeDeck.Where(x => x.Data.WinningPieceUpper == 'N').Take(50 - finalDeck.Count).ToList().ForEach(
                x =>
                {
                    hugeDeck.Remove(x);
                    finalDeck.Add(x);
                });



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

        private void TransferToFinalDeck(char file, char piece, List<TacticCard> finalDeck, List<TacticCard> hugeDeck)
        {
            hugeDeck.Where(x => x.Data.WinningMoveLan[2] == file && x.Data.WinningPieceUpper == piece).Take(1).ToList().ForEach(
              x =>
              {
                  hugeDeck.Remove(x);
                  finalDeck.Add(x);
              });
        }

        private void TransferAnyToFinalDeck(char file, int count, List<TacticCard> finalDeck, List<TacticCard> hugeDeck)
        {
            hugeDeck.Where(x => x.Data.WinningMoveLan[2] == file).Take(count).ToList().ForEach(
               x =>
               {
                   hugeDeck.Remove(x);
                   finalDeck.Add(x);
               });
        }
    }
}