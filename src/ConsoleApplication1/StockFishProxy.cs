using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class StockFishProxy : IDisposable
    {
        private const string LogPath = @"C:\Users\bc0618\Desktop\simon\chess\stockfish-7-win\Windows\output.log";
        private readonly Process _proc;

        public StockFishProxy()
        {
            string path = @"stockfish\\stockfish_8_x64.exe";
            _proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _proc.Start();
        }

        public TacticCard FindMate(StringBuilder moveSequence, int depth)
        {
            string beforeFen = FenAfterMoves(moveSequence.ToString());
            string moves = moveSequence.ToString();
            _proc.StandardInput.WriteLine("setoption name MultiPV value 1");
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("go depth " + depth);
            TacticCard tacticCard = null;
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = ReadLine(_proc);

                if (line.IndexOf("info depth ", StringComparison.Ordinal) == 0)
                {
                    int mateindex = line.IndexOf("mate");
                    if (mateindex >= 0)
                    {
                        // mate found!
                        string mateInMoves = line.Substring(mateindex + 5,
                            line.IndexOf(" ", mateindex + 5) - (mateindex + 5));
                        string principalVariation = line.Substring(line.IndexOf(" pv ")).Trim().Substring(3);
                        string winningMove = principalVariation.Split(new char[] {' '}).First();
                        tacticCard = new TacticCard();
                        tacticCard.Data.FullMovesToMate = int.Parse(mateInMoves);
                        tacticCard.Data.WinningMoveLan = winningMove;
                    }
                }
                else if (line.IndexOf("bestmove", StringComparison.Ordinal) == 0)
                {
                    if (tacticCard != null && tacticCard.Data.FullMovesToMate < 0)
                        return null;
                    return tacticCard;
                }
            }
            throw new Exception("error! no bestmove found");
        }

        public TacticCard FindTactic(StringBuilder moveSequence)
        {
            string beforeFen = FenAfterMoves(moveSequence.ToString());
            string moves = moveSequence.ToString();

            _proc.StandardInput.WriteLine("setoption name MultiPV value 2");
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("go depth 1");

            TacticCard tacticCard = null;

            int infoLineCounter = 0;
            string l1 = null, l2 = null;
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = ReadLine(_proc);

                if (line.IndexOf("info depth ", StringComparison.Ordinal) == 0)
                {
                    infoLineCounter++;
                    if (infoLineCounter % 2 == 0)
                        l1 = line;
                    else
                        l2 = line;
                }
                else if (line.IndexOf("bestmove", StringComparison.Ordinal) == 0)
                {
                    Score bestmove;
                    Score secondMove;
                    if (l1 == null) l1 = l2;
                    if (l2 == null) l2 = l1;
                    bestmove = DeconstructScore(l1);
                    if (bestmove.Bestmove)
                        secondMove = DeconstructScore(l2);
                    else
                    {
                        secondMove = bestmove;
                        bestmove = DeconstructScore(l2);
                    }
                    int delta = bestmove.CPScore - secondMove.CPScore;

                    tacticCard = new TacticCard() {Data = new CardData()};

                    if (bestmove.ScoreType == ScoreType.Mate)
                    {
                        tacticCard.Data.FullMovesToMate = bestmove.CPScore;
                        tacticCard.Data.MultipleSolutions = secondMove.ScoreType == ScoreType.Mate;
                    }
                    else
                    {
                        
                    }
                    tacticCard.Data.ScoreCP = bestmove.CPScore;
                    tacticCard.Data.ScoreType = bestmove.ScoreType;
                    tacticCard.Data.ImprovementCP = delta;
                    tacticCard.Data.WinningMoveLan = bestmove.PrincipalVariation.Split(new char[] {' '})[0];

                    return tacticCard;
                }
            }
            throw new Exception("error! no bestmove found");
        }

        private Score DeconstructScore(string l1)
        {
            Score s = new Score();
            List<string> parts = l1.Split(new char[] {' '}).ToList();
            int scoreIndex = parts.IndexOf("score");
            if (scoreIndex < 0) throw new Exception("Wierd score from engine");
            if (parts[scoreIndex + 1] == "cp")
                s.ScoreType = ScoreType.Tactic;
            else if (parts[scoreIndex + 1] == "mate")
                s.ScoreType = ScoreType.Mate;
            else
                throw new Exception("Wierd score-unit form engine");
            
            s.CPScore = int.Parse(parts[scoreIndex + 2]);
            s.PrincipalVariation = l1.Substring(l1.IndexOf(" pv ")+4);
            s.Bestmove = l1.IndexOf("multipv 1") >= 0;
            return s;
        }

        private string ReadLine(Process _proc)
        {
            string line = _proc.StandardOutput.ReadLine();
            //File.AppendAllLines(LogPath, new string[] { line });
            return line;
        }

        public string FenAfterMoves(string moves)
        {
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("d");
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = ReadLine(_proc);
                if (line.IndexOf("Fen:") == 0)
                {
                    return line.Substring(5);
                }
            }
            return "";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _proc.StandardInput.WriteLine("quit");
            if (!_proc.WaitForExit(5000))
            {
                _proc.Kill();
            }
            _disposed = true;
        }

        public string GetInfo()
        {
            _proc.StandardInput.WriteLine("uci");
            StringBuilder result = new StringBuilder();
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = ReadLine(_proc);
                if (line.IndexOf("id name ") == 0)
                {
                    result.Append(line.Substring(8));
                }
                else if (line.IndexOf("uciok") == 0)
                {
                    return result.ToString();
                }
            }
            return "unknown_engine";
        }
    }

    public class Score
    {
        public int CPScore { get; set; }
        public string PrincipalVariation { get; internal set; }
        public bool Bestmove { get; set; }
        public ScoreType ScoreType { get; set; }
    }

    public enum ScoreType
    {
        Tactic,
        Mate
    }
}
