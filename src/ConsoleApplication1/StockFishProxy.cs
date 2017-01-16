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
        private const string SFPath = @"C:\Users\bc0618\Desktop\simon\chess\stockfish-7-win\Windows\stockfish 7 x64.exe";
        //private const string SFPath = @"C:\Users\bc0618\Desktop\simon\chess\stockfish-7-win\Windows\Houdini_15a_x64.exe";
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

        public MateCard FindMate(StringBuilder moveSequence, int depth)
        {
            string beforeFen = FenAfterMoves(moveSequence.ToString());
            string moves = moveSequence.ToString();
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("go depth " + depth);
            MateCard mateCard = null;
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = ReadLine(_proc);

                if (line.IndexOf("info depth ", StringComparison.Ordinal) == 0)
                {
                    int mateindex = line.IndexOf("mate");
                    if (mateindex >= 0)
                    {
                        // mate found!
                        string mateInMoves = line.Substring(mateindex + 5, line.IndexOf(" ", mateindex + 5) - (mateindex + 5));
                        string principalVariation = line.Substring(line.IndexOf(" pv ")).Trim().Substring(3);
                        string winningMove = principalVariation.Split(new char[] { ' ' }).First();
                        mateCard = new MateCard();
                        mateCard.FullMoves = int.Parse(mateInMoves);
                        mateCard.WinningMoveLan = winningMove;
                    }
                }
                else if (line.IndexOf("bestmove", StringComparison.Ordinal) == 0)
                {
                    if (mateCard != null && mateCard.FullMoves < 0)
                        return null;
                    return mateCard;
                }
            }
            throw new Exception("error! no bestmove found");
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

  
}
