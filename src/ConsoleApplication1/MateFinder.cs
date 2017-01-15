﻿using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApplication1
{
    internal class MateFinder
    {
        private StockFishProxy _stockFish = new StockFishProxy();
        List<PgnParser> _pgnList = new List<PgnParser>();
        public MateFinder(string inputfile)
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

        internal Mate[] FindMateIn(int movesToMateIn)
        {
            List<Mate> allMates = new List<Mate>();
            foreach (var pgn in _pgnList.Skip(0))
            {
               
                // find if we have a mate in 1
                string notation = pgn.Game;
                string[] moves = notation.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Engine e = new Engine();
                StringBuilder allMovesInLan = new StringBuilder();
                for (int i = 0; i < moves.Length; i++)
                {
                    if (_pgnList.IndexOf(pgn) == 19 && i==106)
                    {
                        int debugfail = 1;
                    }
                    // get move
                    string currentMove = moves[i].Replace("+", "").Replace("#","");
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
                    Mate mate = _stockFish.FindMate(allMovesInLan, 10);
                    if (mate != null)
                    {
                        // find shortnotation
                        var generatedMovesAsLan = e.PrintMoves(generatedMoves).ToList();
                        var winningMoveIndex = generatedMovesAsLan.ToList().IndexOf(mate.WinningMoveLan);
                        string winningMoveSan = generatedMovesAsSan[winningMoveIndex];
                        mate.WinningMoveSan = winningMoveSan;
                        mate.Fen = e.PrintFen();
                        mate.WhiteToMove = e.ActiveColorIsWhite;
                        int eval = e.Evaluate();
                        if(Math.Abs(eval) <= 100)
                            allMates.Add(mate);
                        System.Diagnostics.Debug.WriteLine($"{(_pgnList.IndexOf(pgn)*100.0/ _pgnList.Count), 3:0.#}% complete, game#{_pgnList.IndexOf(pgn)}/{_pgnList.Count}, mates found: {allMates.Count} (last mate in {((allMates.Count != 0) ? allMates.Last().HalfMoves.ToString():" - ")})");

                        // hack
                        if(allMates.Count > 0)
                        {
                            return allMates.ToArray();
                        }
                    }

                    // do move and continue searching for mate
                    e.DoMove(generatedMoves[moveIndex]);
                    allMovesInLan.Append(moveAsLan + " ");

                    // validate position with stockfish
                    var fen1 = e.PrintFen();
                    var fen2 = _stockFish.FenAfterMoves(allMovesInLan.ToString());
                    if(fen1 != fen2)
                    {
                        // error
                        throw new Exception("oh no! seems to be a bug in engine!?!?");
                    }
                }
            }
            return allMates.ToArray();
        }
    }

    public class Mate
    {
        public int HalfMoves { get; set; }
        public string WinningMoveLan { get; set; }
        public string WinningMoveSan { get; internal set; }
        public string Fen { get; set; }
        public bool WhiteToMove { get; set; }
    }
}