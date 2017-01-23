using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    internal class MateCardGenerator
    {
        private string _jsonFilename = "matecards.json";
        private StockFishProxy _stockFish = new StockFishProxy();
        private List<PgnParser> _pgnList = new List<PgnParser>();

        public MateCardGenerator(string inputfile)
        {
            // read pgns
            string allPgns = File.ReadAllText(inputfile);
            // chop it
            string[] pgns = PgnParser.ChopIt(allPgns);
            foreach (var pgn in pgns)
            {
                PgnParser parser = new PgnParser(pgn);
                parser.Parse();
                _pgnList.Add(parser);
            }
        }

        public TacticCard[] LoadCachedCards()
        {
            return LoadCachedCards(_jsonFilename);
        }

        public TacticCard[] LoadCachedCards(string filename)
        {
            if (File.Exists(filename))
            {
                return JsonConvert.DeserializeObject<TacticCard[]>(File.ReadAllText(filename));
            }
            return new TacticCard[0];
        }

        internal TacticCard[] Generate(bool appendToCached)
        {
            int test = 0;
            List<TacticCard> allMates = new List<TacticCard>();
            foreach (var pgn in _pgnList.Skip(0))
            {

                // find if we have a mate in 1
                string notation = pgn.Game;
                string[] moves = notation.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Engine e = new Engine();
                StringBuilder allMovesInLan = new StringBuilder();
                for (int i = 0; i < moves.Length; i++)
                {
                    if (_pgnList.IndexOf(pgn) == 19 && i == 106)
                    {
                        int debugfail = 1;
                    }
                    // get move
                    string currentMove = moves[i].Replace("+", "").Replace("#", "");
                    if (currentMove.IndexOf("1/2-1/2") >= 0
                        || currentMove.IndexOf("1-0") >= 0
                        || currentMove.IndexOf("0-1") >= 0)
                    {
                        // game over
                        break;
                    }
                    if (i % 3 == 0)
                        continue;
                    var generatedMoves = e.GenerateMoves();
                    var generatedMovesAsSan = e.PrintAsSan(generatedMoves);
                    var moveIndex = generatedMovesAsSan.ToList().IndexOf(currentMove);
                    string moveAsLan = e.PrintMove(generatedMoves[moveIndex]);

                    // ask stockfish if mate in X
                    TacticCard tacticCard = _stockFish.FindTactic(allMovesInLan);
                    if (tacticCard?.Data.ScoreType == ScoreType.Mate 
                        && tacticCard.Data.FullMovesToMate == 1 
                        && tacticCard.Data.MultipleSolutions == false
                        && e.Evaluate()<500)
                    {
                        tacticCard.Data.WhiteToMove = e.ActiveColorIsWhite;
                        tacticCard.Data.Fen = e.PrintFen();

                        int idx = e.PrintMoves(generatedMoves).ToList().IndexOf(tacticCard.Data.WinningMoveLan);
                        tacticCard.Data.WinningMoveSan = generatedMovesAsSan[idx];
                        tacticCard.Data.WinningPieceUpper = char.ToUpper((char)generatedMoves[idx][5]);
                        tacticCard.Data.IsCapture = tacticCard.Data.WinningMoveSan.Contains("x");
                        allMates.Add(tacticCard);
                        System.Diagnostics.Debug.WriteLine($"{test}: {(_pgnList.IndexOf(pgn) * 100.0 / _pgnList.Count),3:0.#}% complete, game#{_pgnList.IndexOf(pgn)}/{_pgnList.Count}, mates found: {allMates.Count} (last mate in {((allMates.Count != 0) ? allMates.Last().Data.FullMovesToMate.ToString() : " - ")})");
                        break;
                    }

                    // do move and continue searching for mate
                    e.DoMove(generatedMoves[moveIndex]);
                    allMovesInLan.Append(moveAsLan + " ");

                    // validate position with stockfish
                    var fen1 = e.PrintFen();
                    var fen2 = _stockFish.FenAfterMoves(allMovesInLan.ToString());
                    if (fen1 != fen2)
                    {
                        // error
                        throw new Exception("oh no! seems to be a bug in engine!?!?");
                    }
                }
                test = 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'P' && x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'P' && !x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'R' && x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'R' && !x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'N' && x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'N' && !x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'B' && x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'B' && !x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'Q' && x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                test += allMates.Count(x => x.Data.WinningPieceUpper == 'Q' && !x.Data.WhiteToMove && x.Data.FullMovesToMate == 1) > 5 ? 1 : 0;
                

                if (test == 10)
                    break;
            }
            if (appendToCached)
            {
                allMates.AddRange(LoadCachedCards());
            }
            File.WriteAllText(_jsonFilename, JsonConvert.SerializeObject(allMates.ToArray()));
            return allMates.ToArray();
        }
    }

}