using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    public class Engine
    {
        private Stopwatch sw1 = new Stopwatch();
        private Stopwatch sw2 = new Stopwatch();
        private Stopwatch sw3 = new Stopwatch();
        private const int A1 = 2 * 12 + 2;
        private const int A8 = 9 * 12 + 2;
        private const int H1 = 2 * 12 + 9;
        private const int H8 = 9 * 12 + 9;
        private const int N = 12;
        private const int S = -12;
        private const int W = -1;
        private const int E = 1;
        private const int NW = N + W;
        private const int NE = N + E;
        private const int SW = S + W;
        private const int SE = S + E;
        private const int N1 = N * 2 + E;
        private const int N2 = N * 2 + W;
        private const int N3 = S * 2 + E;
        private const int N4 = S * 2 + W;
        private const int N5 = E * 2 + N;
        private const int N6 = E * 2 + S;
        private const int N7 = W * 2 + N;
        private const int N8 = W * 2 + S;
        public int NoMoveEvaluation;
        public bool ActiveColorIsWhite { get; set; }

        public bool WhiteCastleLong { get; set; }

        public bool WhiteCastleShort { get; set; }

        public bool BlackCastleLong { get; set; }

        public bool BlackCastleShort { get; set; }

        public int EnPassantTarget { get; set; }

        public int HalfmoveClock { get; set; }

        public int FullMoveNumber { get; set; }


        char[] _board = new char[12 * 12];

        public Engine()
        {
            Init();
        }

        public int[] BestMove(int depth)
        {
            int[] best = new int[0];
            NegaMax(depth, ref best);
            return best;
        }

        int NegaMax(int depth, ref int[] bestMove)
        {
            var ms = GenerateMoves();
            if (ms.Count == 0)
                return ActiveColorIsWhite ? NoMoveEvaluation : -NoMoveEvaluation;
            if (depth == 0)
                return Evaluate();
            int max = -int.MaxValue;
            int[] tempBestMove = bestMove;
            foreach (var m in ms)
            {
                DoMove(m);
                int score = -NegaMax(depth - 1, ref bestMove);
                UndoMove(m);
                if (score > max)
                {
                    max = score;
                    tempBestMove = m;
                }
            }
            bestMove = tempBestMove;
            return max;
        }


        private int Evaluate()
        {
            int eval = 0;
            int p, r, n, b, q, k, P, R, N, B, Q, K;
            p = r = n = b = q = k = P = R = N = B = Q = K = 0;
            int j = 0;
            int blackKingIndex = 0, whiteKingIndex = 0;
            for (int i = 12 * 2; i < 12 * 10; i++)
            {
                switch (_board[i])
                {
                    case 'p': eval -= 100; p++; break;
                    case 'r': eval -= 500; r++; break;
                    case 'n': eval -= 300; n++; break;
                    case 'b': eval -= 300; b++; break;
                    case 'q': eval -= 900; q++; break;
                    case 'k': k++; blackKingIndex = j; break;
                    case 'P': eval += 100; P++; break;
                    case 'R': eval += 500; R++; break;
                    case 'N': eval += 300; N++; break;
                    case 'B': eval += 300; B++; break;
                    case 'Q': eval += 900; Q++; break;
                    case 'K': K++; whiteKingIndex = j; break;
                }
                j++;
            }
            bool bkOnly = p + r + n + b + q == 0;
            bool wkOnly = P + R + N + B + Q == 0;
            if (bkOnly)
            {
                int bkingdist = Math.Abs(blackKingIndex / 12) + Math.Abs(blackKingIndex % 12 - 2);
                int wkingdist = Math.Abs(whiteKingIndex / 12 - 2) + Math.Abs(whiteKingIndex % 12 - 4);
                eval -= bkingdist * 2;
                eval -= wkingdist;
            }
            else if (wkOnly)
            {
                int wkingdist = Math.Abs(whiteKingIndex / 12) + Math.Abs(whiteKingIndex % 12 - 2);
                int bkingdist = Math.Abs(blackKingIndex / 12 - 2) + Math.Abs(blackKingIndex % 12 - 4);
                eval -= wkingdist * 2;
                eval -= bkingdist;
            }
            return ActiveColorIsWhite ? eval : -eval;
        }

        private bool IsSafeSquare(int i, bool whitesTurn)
        {
            char atttackingRook = whitesTurn ? 'r' : 'R';
            char attackingKnight = whitesTurn ? 'n' : 'N';
            char attackingBishop = whitesTurn ? 'b' : 'B';
            char attackingQueen = whitesTurn ? 'q' : 'Q';
            char attackingPawn = whitesTurn ? 'p' : 'P';
            char attackingKing = whitesTurn ? 'k' : 'K';

            if (_board[i + N1] == attackingKnight) return false;
            if (_board[i + N2] == attackingKnight) return false;
            if (_board[i + N3] == attackingKnight) return false;
            if (_board[i + N4] == attackingKnight) return false;
            if (_board[i + N5] == attackingKnight) return false;
            if (_board[i + N6] == attackingKnight) return false;
            if (_board[i + N7] == attackingKnight) return false;
            if (_board[i + N8] == attackingKnight) return false;

            if (_board[i + N] == attackingKing) return false;
            if (_board[i + S] == attackingKing) return false;
            if (_board[i + E] == attackingKing) return false;
            if (_board[i + W] == attackingKing) return false;
            if (_board[i + NE] == attackingKing) return false;
            if (_board[i + NW] == attackingKing) return false;
            if (_board[i + SE] == attackingKing) return false;
            if (_board[i + SW] == attackingKing) return false;

            for (int k = i + N; ; k = k + N)
            {
                if (_board[k] == atttackingRook) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + S; ; k = k + S)
            {
                if (_board[k] == atttackingRook) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + E; ; k = k + E)
            {
                if (_board[k] == atttackingRook) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + W; ; k = k + W)
            {
                if (_board[k] == atttackingRook) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + NE; ; k = k + NE)
            {
                if (_board[k] == attackingBishop) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + NW; ; k = k + NW)
            {
                if (_board[k] == attackingBishop) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + SE; ; k = k + SE)
            {
                if (_board[k] == attackingBishop) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            for (int k = i + SW; ; k = k + SW)
            {
                if (_board[k] == attackingBishop) return false;
                if (_board[k] == attackingQueen) return false;
                if (_board[k] != ' ') break;
            }
            int forward = whitesTurn ? N : S;
            if (_board[i + forward + E] == attackingPawn) return false;
            if (_board[i + forward + W] == attackingPawn) return false;

            return true;
        }


        public List<int[]> GenerateMoves()
        {
            int i = 12 * 2 + 2;
            int stateBefore = 0;
            if (WhiteCastleLong) stateBefore |= 0x01;
            if (WhiteCastleShort) stateBefore |= 0x02;
            if (BlackCastleLong) stateBefore |= 0x04;
            if (BlackCastleShort) stateBefore |= 0x08;
            int stateAfter = stateBefore;
            List<int[]> moveList = new List<int[]>();
            while (true)
            {
                char piece = _board[i];
                if (piece == '.')
                {
                    // if border - skip to next
                    i += 4;
                    piece = _board[i];
                    // if next is border were done
                    if (piece == '.')
                        break;
                }
                if (piece == ' ')
                {
                    // skip empty
                    i++;
                    continue;
                }
                bool whitesTurn = ActiveColorIsWhite;
                if (((piece & 0x60) == 0x60 && !whitesTurn) ||
                    ((piece & 0x60) == 0x40 && whitesTurn))
                {
                    int opponentMask = whitesTurn ? 0x60 : 0x40;
                    int pawn, rook, knight, bishop, queen, king;
                    if (whitesTurn)
                    {
                        pawn = 'P';
                        rook = 'R';
                        knight = 'N';
                        bishop = 'B';
                        queen = 'Q';
                        king = 'K';
                    }
                    else
                    {
                        pawn = 'p';
                        rook = 'r';
                        knight = 'n';
                        bishop = 'b';
                        queen = 'q';
                        king = 'k';
                    }
                    if (piece == knight)
                    {
                        piece = _board[i + N1]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N1, piece, knight });
                        piece = _board[i + N2]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N2, piece, knight });
                        piece = _board[i + N3]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N3, piece, knight });
                        piece = _board[i + N4]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N4, piece, knight });
                        piece = _board[i + N5]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N5, piece, knight });
                        piece = _board[i + N6]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N6, piece, knight });
                        piece = _board[i + N7]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N7, piece, knight });
                        piece = _board[i + N8]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, knight, ' ', i + N8, piece, knight });
                    }
                    else if (piece == rook)
                    {
                        for (int tempIndex = i + N; ; tempIndex = tempIndex + N)
                        {
                            piece = _board[tempIndex];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, rook, ' ', tempIndex, piece, rook });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int tempIndex = i + S; ; tempIndex = tempIndex + S)
                        {
                            piece = _board[tempIndex];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, rook, ' ', tempIndex, piece, rook });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int tempIndex = i + E; ; tempIndex = tempIndex + E)
                        {
                            piece = _board[tempIndex];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, rook, ' ', tempIndex, piece, rook });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int tempIndex = i + W; ; tempIndex = tempIndex + W)
                        {
                            piece = _board[tempIndex];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, rook, ' ', tempIndex, piece, rook });
                            if (piece != ' ' || piece == '.') break;
                        }
                    }
                    else if (piece == queen)
                    {
                        for (int k = i + N; ; k = k + N)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + S; ; k = k + S)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + E; ; k = k + E)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + W; ; k = k + W)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + NE; ; k = k + NE)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + NW; ; k = k + NW)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + SE; ; k = k + SE)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + SW; ; k = k + SW)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, queen, ' ', k, piece, queen });
                            if (piece != ' ' || piece == '.') break;
                        }
                    }
                    else if (piece == bishop)
                    {
                        for (int k = i + NE; ; k = k + NE)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, bishop, ' ', k, piece, bishop });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + NW; ; k = k + NW)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, bishop, ' ', k, piece, bishop });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + SE; ; k = k + SE)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, bishop, ' ', k, piece, bishop });
                            if (piece != ' ' || piece == '.') break;
                        }
                        for (int k = i + SW; ; k = k + SW)
                        {
                            piece = _board[k];
                            if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, bishop, ' ', k, piece, bishop });
                            if (piece != ' ' || piece == '.') break;
                        }
                    }
                    else if (piece == king)
                    {
                        piece = _board[i + N]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + N, piece, king });
                        piece = _board[i + S]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + S, piece, king });
                        piece = _board[i + E];
                        if (piece == ' ' || (piece & 0x60) == opponentMask)
                        {
                            moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + E, piece, king });
                            if (piece == ' ')
                            {
                                bool castleShort = whitesTurn ? WhiteCastleShort : BlackCastleShort;
                                castleShort = castleShort && IsSafeSquare(i, ActiveColorIsWhite);
                                if (castleShort
                                    && _board[i + E] == ' '
                                    && _board[i + 2 * E] == ' '
                                    && IsSafeSquare(i + E, ActiveColorIsWhite)
                                    && IsSafeSquare(i + 2 * E, ActiveColorIsWhite))
                                    moveList.Add(new[]
                                    {
                                        stateBefore, stateAfter,
                                        EnPassantTarget, 0,
                                        i + 0*E, king, ' ',
                                        i + 2*E, ' ', king,
                                        i + 1*E, ' ', rook,
                                        i + 3*E, rook, ' ',
                                    });

                            }
                        }
                        piece = _board[i + W];
                        if (piece == ' ' || (piece & 0x60) == opponentMask)
                        {
                            moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + W, piece, king });
                            if (piece == ' ')
                            {
                                bool castleLong = whitesTurn ? WhiteCastleLong : BlackCastleLong;
                                castleLong = castleLong && IsSafeSquare(i, ActiveColorIsWhite);
                                if (castleLong
                                    && _board[i + W] == ' '
                                    && _board[i + 2 * W] == ' '
                                    && _board[i + 3 * W] == ' '
                                    && IsSafeSquare(i + W, ActiveColorIsWhite)
                                    && IsSafeSquare(i + 2 * W, ActiveColorIsWhite))
                                    moveList.Add(new[]
                                    {
                                        stateBefore, stateAfter,
                                        EnPassantTarget, 0,
                                        i + 0*W, king, ' ',
                                        i + 2*W, ' ', king,
                                        i + 1*W, ' ', rook,
                                        i + 4*W, rook, ' ',
                                    });
                            }
                        }
                        piece = _board[i + NE]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + NE, piece, king });
                        piece = _board[i + NW]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + NW, piece, king });
                        piece = _board[i + SE]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + SE, piece, king });
                        piece = _board[i + SW]; if (piece == ' ' || (piece & 0x60) == opponentMask) moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, king, ' ', i + SW, piece, king });
                    }
                    else if (piece == pawn)
                    {
                        int forward = whitesTurn ? N : S;
                        int row = (i + forward) / 12;
                        bool promote = row == 2 || row == 9;
                        foreach (int d in new[] { E, W })
                        {
                            int k = i + forward + d;
                            piece = _board[k];
                            if ((piece & 0x60) == opponentMask)
                            {
                                if (promote)
                                {
                                    moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, rook });
                                    moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, knight });
                                    moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, bishop });
                                    moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, queen });
                                }
                                else
                                    moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, pawn });
                            }
                            else if (k == EnPassantTarget)
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, ' ', pawn, k - forward, _board[k - forward], ' ' });
                        }
                        piece = _board[i + forward];
                        if (piece == ' ')
                        {
                            if (promote)
                            {
                                int k = i + forward;
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, rook });
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, knight });
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, bishop });
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', k, piece, queen });
                            }
                            else
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, 0, i, pawn, ' ', i + forward, ' ', pawn });
                            bool firstMove = (whitesTurn && row == 4) || (!whitesTurn && row == 7);
                            if (firstMove && _board[i + 2 * forward] == ' ')
                            {
                                moveList.Add(new[] { stateBefore, stateAfter, EnPassantTarget, i + forward, i, pawn, ' ', i + 2 * forward, ' ', pawn });
                            }
                        }
                    }
                }
                i++;
            }

            // filter illegal moves..
            NoMoveEvaluation = 0;
            List<int[]> legalMoves = new List<int[]>();
            int ki = FindKing(0, ActiveColorIsWhite);
            if (ki == -1) return legalMoves;
            foreach (var m in moveList)
            {
                // set capture/pawnadvance state 
                int p = m[5];
                bool capture = (m[8] != ' ');
                bool pawnAdvance = p == 'p' || p == 'P';
                stateAfter = pawnAdvance || capture ? stateBefore | 0x10 : stateBefore;
                if (m[4] == A1 || m[7] == A1)
                    stateAfter = stateAfter & ~0x01;
                else if (m[4] == H1 || m[7] == H1)
                    stateAfter = stateAfter & ~0x02;
                else if (m[4] == A8 || m[7] == A8)
                    stateAfter = stateAfter & ~0x04;
                else if (m[4] == H8 || m[7] == H8)
                    stateAfter = stateAfter & ~0x08;
                else if (m[5] == 'K')
                    stateAfter = stateAfter & ~0x03;
                else if (m[5] == 'k')
                    stateAfter = stateAfter & ~0x0c;
                m[1] = stateAfter | (HalfmoveClock << 8);
                DoMove(m);
                if (IsSafeSquare(FindKing(ki, !ActiveColorIsWhite), !ActiveColorIsWhite))
                    legalMoves.Add(m);
                UndoMove(m);
            }
            // if no legal moves check for draw or mate..
            if (legalMoves.Count == 0 && !IsSafeSquare(ki, ActiveColorIsWhite))
                NoMoveEvaluation = ActiveColorIsWhite ? -30000 : 30000;
            return legalMoves;
        }




        public void DoMove(int[] m)
        {
            // move structure
            // m[0]: castle info before
            // m[1]: castle+capture info after 
            // m[2]: en passant before
            // m[3]: en passant after
            // m[4]: index (source)
            // m[5]: piece before
            // m[6]: piece after
            // m[7]: index (dest)
            // m[8]: piece before
            // m[9]: piece after
            // m[10...]: other affected squares (en passant + castle)

            int i = 4;
            while (i < m.Length)
            {
                _board[m[i]] = (char)m[i + 2];
                i += 3;
            }
            WhiteCastleLong = (m[1] & 0x01) != 0;
            WhiteCastleShort = (m[1] & 0x02) != 0;
            BlackCastleLong = (m[1] & 0x04) != 0;
            BlackCastleShort = (m[1] & 0x08) != 0;
            EnPassantTarget = m[3];
            if ((m[1] & 0x10) != 0)
                HalfmoveClock = 0;
            else
                HalfmoveClock++;
            ActiveColorIsWhite = !ActiveColorIsWhite;
            if (ActiveColorIsWhite)
                FullMoveNumber++;
        }
        public void UndoMove(int[] m)
        {
            if (ActiveColorIsWhite)
                FullMoveNumber--;
            ActiveColorIsWhite = !ActiveColorIsWhite;
            HalfmoveClock = m[1] >> 8;
            EnPassantTarget = m[2];
            WhiteCastleLong = (m[0] & 0x01) != 0;
            WhiteCastleShort = (m[0] & 0x02) != 0;
            BlackCastleLong = (m[0] & 0x04) != 0;
            BlackCastleShort = (m[0] & 0x08) != 0;
            int i = 4;
            while (i < m.Length)
            {
                _board[m[i]] = (char)m[i + 1];
                i += 3;
            }
        }

        private int FindKing(int kingIndex, bool findWhiteKing)
        {
            char pK = (findWhiteKing) ? 'K' : 'k';
            if (kingIndex == 0)
            {
                for (int i = 2 * 12 + 2; i < 12 * 12; i++)
                {
                    if (_board[i] == pK) return i;
                }
            }
            else
            {
                if (_board[kingIndex] == pK) return kingIndex;
                if (_board[kingIndex + E] == pK) return kingIndex + E;
                if (_board[kingIndex + W] == pK) return kingIndex + W;
                if (_board[kingIndex + NW] == pK) return kingIndex + NW;
                if (_board[kingIndex + NE] == pK) return kingIndex + NE;
                if (_board[kingIndex + N] == pK) return kingIndex + N;
                if (_board[kingIndex + SW] == pK) return kingIndex + SW;
                if (_board[kingIndex + SE] == pK) return kingIndex + SE;
                if (_board[kingIndex + S] == pK) return kingIndex + S;
                if (_board[kingIndex + 2 * E] == pK) return kingIndex + 2 * E;
                if (_board[kingIndex + 2 * W] == pK) return kingIndex + 2 * W;
            }
            return -1;
        }

        public void SetPosition(string p)
        {
            _board = p.ToCharArray();
        }

        public void Init()
        {
            _board = (
                "............" +
                "............" +
                "..RNBQKBNR.." +
                "..PPPPPPPP.." +
                "..        .." +
                "..        .." +
                "..        .." +
                "..        .." +
                "..pppppppp.." +
                "..rnbqkbnr.." +
                "............" +
                "............").ToCharArray();
            ActiveColorIsWhite = true;
            WhiteCastleLong = WhiteCastleShort = BlackCastleLong = BlackCastleShort = true;
            EnPassantTarget = 0;
            HalfmoveClock = 0;
            FullMoveNumber = 1;
        }


        public string PrintMove(int[] m)
        {
            int src = m[4];
            int dest = m[7];
            string srcText = (char)('a' + ((src % 12) - 2)) + "" + (char)('1' + ((src / 12) - 2));
            string destText = (char)('a' + ((dest % 12) - 2)) + "" + (char)('1' + ((dest / 12) - 2));
            string promoteTo = "";
            if (m[5] != m[9])
                promoteTo = (char)m[9] + "";
            return srcText + destText + promoteTo.ToLower();
        }
        public string[] PrintMoves(List<int[]> moveList)
        {
            List<string> textMoves = new List<string>();
            foreach (var m in moveList)
            {
                textMoves.Add(PrintMove(m));
            }
            return textMoves.ToArray();
        }

        public string PrintBoard()
        {
            var b = new string(_board);

            b =
                b.Substring(9 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(8 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(7 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(6 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(5 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(4 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(3 * 12 + 2, 8) + Environment.NewLine +
                b.Substring(2 * 12 + 2, 8) + Environment.NewLine;
            return b;
        }
        public string PrintFen()
        {
            var b = new string(_board);
            b =
                b.Substring(9 * 12 + 2, 8) + "/" +
                b.Substring(8 * 12 + 2, 8) + "/" +
                b.Substring(7 * 12 + 2, 8) + "/" +
                b.Substring(6 * 12 + 2, 8) + "/" +
                b.Substring(5 * 12 + 2, 8) + "/" +
                b.Substring(4 * 12 + 2, 8) + "/" +
                b.Substring(3 * 12 + 2, 8) + "/" +
                b.Substring(2 * 12 + 2, 8);
            b = b
                .Replace("        ", "8")
                .Replace("       ", "7")
                .Replace("      ", "6")
                .Replace("     ", "5")
                .Replace("    ", "4")
                .Replace("   ", "3")
                .Replace("  ", "2")
                .Replace(" ", "1");
            string w = ActiveColorIsWhite ? "w" : "b";
            string castleInfo = (WhiteCastleShort ? "K" : "") + (WhiteCastleLong ? "Q" : "") + (BlackCastleShort ? "k" : "") + (BlackCastleLong ? "q" : "");
            if (castleInfo == "") castleInfo = "-";
            string e = EnPassantTarget == 0 ? "-" : "" + ((char)((EnPassantTarget % 12 - 2) + 'a')) + ((char)((EnPassantTarget / 12 - 2) + '1'));
            string hm = HalfmoveClock.ToString();
            string fm = FullMoveNumber.ToString();

            // sf hack/adjustment
            if (EnPassantTarget != 0)
            {
                if (ActiveColorIsWhite)
                {
                    if (_board[EnPassantTarget - 13] != 'P' && _board[EnPassantTarget - 11] != 'P')
                        e = "-";
                }
                else
                {
                    if (_board[EnPassantTarget + 13] != 'p' && _board[EnPassantTarget + 11] != 'p')
                        e = "-";
                }
            }

            b = b + $" {w} {castleInfo} {e} {hm} {fm}";
            return b;
        }

        public void SetPositionByFen(string fen)
        {
            string[] elements = fen.Split(new char[] { ' ' });

            elements[0] = elements[0]
                .Replace("1", " ")
                .Replace("2", "  ")
                .Replace("3", "   ")
                .Replace("4", "    ")
                .Replace("5", "     ")
                .Replace("6", "      ")
                .Replace("7", "       ")
                .Replace("8", "        ")
                .Replace("/", "....");
            elements[0] =
                elements[0].Substring(7 * 12, 8) + "...." +
                elements[0].Substring(6 * 12, 12) +
                elements[0].Substring(5 * 12, 12) +
                elements[0].Substring(4 * 12, 12) +
                elements[0].Substring(3 * 12, 12) +
                elements[0].Substring(2 * 12, 12) +
                elements[0].Substring(1 * 12, 12) +
                elements[0].Substring(0 * 12, 12);

            elements[0] = ".........................." + elements[0] + "......................";
            _board = elements[0].ToCharArray();
            ActiveColorIsWhite = elements[1] == "w";
            WhiteCastleLong = elements[2].IndexOf("Q", StringComparison.Ordinal) >= 0;
            WhiteCastleShort = elements[2].IndexOf("K", StringComparison.Ordinal) >= 0;
            BlackCastleLong = elements[2].IndexOf("q", StringComparison.Ordinal) >= 0;
            BlackCastleShort = elements[2].IndexOf("k", StringComparison.Ordinal) >= 0;
            EnPassantTarget = elements[3] == "-" ? 0 :
                ((elements[3][0] - 'a') + 2) + ((elements[3][1] - '1') + 2) * 12;
            HalfmoveClock = int.Parse(elements[4]);
            FullMoveNumber = int.Parse(elements[5]);
        }

        public string ElapsedReport()
        {
            return $"sw1:{sw1.ElapsedMilliseconds}, sw2:{sw2.ElapsedMilliseconds}";
        }




        public string[] PrintAsSan(List<int[]> moveList)
        {

            List<string> textMoves = new List<string>();
            foreach (var m in moveList)
            {
                // move structure
                // m[0]: castle info before
                // m[1]: castle+capture info after 
                // m[2]: en passant before
                // m[3]: en passant after
                // m[4]: index (source)
                // m[5]: piece before
                // m[6]: piece after
                // m[7]: index (dest)
                // m[8]: piece before
                // m[9]: piece after
                // m[10...]: other affected squares (en passant + castle)
                string fulltext = PrintMove(m);

                fulltext =
                    char.ToUpper((char)m[5])
                    + fulltext.Substring(0, 2)
                    + (m[8] != ' ' ? "x" : " ")
                    + fulltext.Substring(2, 2)
                    + (m[5] != m[9] ? ("=" + char.ToUpper((char)m[9])) : "");

                textMoves.Add(fulltext);
            }
            // trim
            for (int i = 0; i < textMoves.Count; i++)
            {
                string curmove = textMoves[i];
                if (curmove[0] != 'P')
                {
                    if (textMoves.Count(x => RemoveSourceRow(x) == RemoveSourceRow(curmove)) == 1)
                    {
                        if (textMoves.Count(x => RemoveSourceColumn(RemoveSourceRow(x)) == RemoveSourceColumn(RemoveSourceRow(curmove))) == 1)
                            curmove = RemoveSourceColumn(RemoveSourceRow(curmove));
                        else
                            curmove = RemoveSourceRow(curmove);
                    }
                    else
                    {
                        if (textMoves.Count(x => RemoveSourceColumn(x) == RemoveSourceColumn(curmove)) == 1)
                            curmove = RemoveSourceColumn(curmove);
                    }
                }
                else
                {
                    if (curmove.Contains("x"))
                        curmove = RemoveSourceRow(curmove);
                    else
                        curmove = RemoveSourceRow(RemoveSourceColumn(curmove));
                }
                textMoves.RemoveAt(i);
                textMoves.Insert(i, curmove);
            }
            for (int i = 0; i < textMoves.Count; i++)
            {
                var m = textMoves[i].ToCharArray();
                if (m[0] == 'P')
                    m[0] = ' ';
                if (m[0] == 'K')
                {
                    // castling
                    var movearr = moveList[i];
                    if (movearr[4] - movearr[7] == -2)
                        m = "O-O".ToCharArray();
                    else if (movearr[4] - movearr[7] == 2)
                        m = "O-O-O".ToCharArray();
                }

                textMoves.RemoveAt(i);
                textMoves.Insert(i, new string(m).Replace(" ", ""));
            }

            return textMoves.ToArray();
        }

        private string RemoveSourceRow(string s)
        {
            var m = s.ToCharArray();
            m[2] = ' ';
            return new string(m);
        }
        private string RemoveSourceColumn(string s)
        {
            var m = s.ToCharArray();
            m[1] = ' ';
            return new string(m);
        }
    }
}