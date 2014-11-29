using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItalianCheckers.Models
{
    
    public static class Rules
    {
       
        public static List<Motion> GetAllMotions(Board board, bool isWhite)
        {
            List<List<Point>> moves = new List<List<Point>>();
            List<KillItem> kills = new List<KillItem>();
            
            Func<int, int, bool> colorValidator;

            if (isWhite)
                colorValidator = board.IsWhite;
            else
                colorValidator = board.IsBlack;

            for (int i = 0; i < Board.SIZE; i++)
                for (int j = 0; j < Board.SIZE; j++)
                {
                    if (colorValidator(i, j))
                    {
                        var kill = new KillItem();

                        if (board.IsSenior(i, j))
                            Senior.FindMotions(board, isWhite, i, j, ref moves, ref kill);
                        else
                            Pawn.FindMotions(board, isWhite, i, j, ref moves, ref kill);

                        if (kill.Move != null) // значит кого-то убили
                            kills.Add(kill);
                    }
                }

            var resultKills = new List<List<Point>>();

            kills.ForEach(pk =>
            {
                resultKills.AddRange(pk.SplitToBranches());
                pk.Dispose();
            });

            var list = resultKills.Count > 0 ? resultKills : moves;

            var ret = list.Select(m => new Motion(m.ToArray())).ToList();

            return ret;
        }


        #region Pawn
        static class Pawn
        {
            private static int[] dX = new int[2] { 1, -1 };
            internal static void FindMotions(Board board, bool isWhite, int x, int y, 
                ref  List<List<Point>> moves, 
                ref  KillItem kills)
            {
                FindMoves(board, isWhite, x, y, ref moves);
                FindKills(board, isWhite, x, y, ref kills);
            }

            internal static void FindMoves(Board board, bool isWhite, int x, int y,
                ref  List<List<Point>> moves)
            {
                int dY = isWhite ? -1 : 1;
                int yN = y + dY;

                for (int i = 0; i < dX.Length; i++)
                {
                    int xN = x + dX[i];

                    if (CanMove(board, xN, yN))
                    {
                        moves.Add(new List<Point>() { new Point(x, y), new Point(xN, yN) });
                    }
                }
            }

            internal static bool CanMove(Board board, int xN, int yN)
            {
                if (!InBounds(xN) || !InBounds(yN))
                    return false;

                return board[xN, yN] == FigureEnum.NONE;
            }


            private static void FindKills(Board board, bool isWhite, int x, int y, ref KillItem kills)
            {
                int[,] _whiteDirections = new int[,]  { { -1, -1 }, { 1, -1 } };
                int[,] _blackDirections = new int[,] { { 1, 1 }, { -1, 1 } };

                var dir = isWhite ? _whiteDirections : _blackDirections;
                
                for(int i = 0; i < 2; i++)
                {
                    int xN = x + dir[i, 0];
                    int yN = y + dir[i, 1];
                    int xN2 = x + 2 * dir[i, 0];
                    int yN2 = y + 2 * dir[i, 1];

                    if (!InBounds(xN) || !InBounds(yN) || !InBounds(xN2) || !InBounds(yN2))
                        continue;

                    if (board[xN2, yN2] != FigureEnum.NONE)
                        continue;

                    if( CheckersHasDifferentColor(board, x, y, xN, yN) && !board.IsSenior(xN,yN))
                    {
                        var killed = new Point(xN,yN);

                        if(kills.BranchContaintsValue(killed))
                            continue;

                        if(kills.Move == null)
                        {
                            kills.Move = new Point(x, y);
                        }
                        
                        var beat = new KillItem
                        {
                            Move = new Point(xN2,yN2),
                            Killed = killed
                        };
                                                
                        kills.AddChild(ref beat);

                        var boardCopy = (Board) board.Clone();
                        
                        boardCopy[xN2, yN2] = boardCopy[x, y];
                        boardCopy[x, y] = FigureEnum.NONE;

                        if (ShouldBecomeSenior(yN2, isWhite))
                            Senior.FindKills(boardCopy, isWhite, xN2, yN2, ref beat);
                        else
                            FindKills(boardCopy, isWhite, xN2, yN2, ref beat);
                    }
                }
            }
        }

        #endregion

        #region Senior
        static class Senior
        {
            static int[,] _moveDirections = new int[,] { { 1, 1 }, { -1, 1 }, { -1, -1 }, { -1, -1 } };

            internal static void FindMotions(Board board, bool isWhite, int i, int j, ref List<List<Point>> moves, ref KillItem kill)
            {
                FindMoves(board, i,j, ref moves);
                FindKills(board, isWhite, i, j, ref kill);
                kill.FilterForSeniorKills();
            }


            internal static void FindMoves(Board board, int x, int y, ref List<List<Point>> moves)
            {
                for(int i = 0; i < 4; i++)
                {
                    int xN = x + _moveDirections[i, 0];
                    int yN = y + _moveDirections[i, 1];

                    while(InBounds(xN) && InBounds(yN) && board.IsEmpty(xN,yN))
                    {
                        moves.Add(new List<Point>() { new Point(x, y), new Point(xN, yN) });

                        xN += _moveDirections[i, 0];
                        yN += _moveDirections[i, 1];
                    }
                }
            }

            internal static void FindKills(Board board, bool isWhite, int x, int y, ref KillItem kills)
            {
                for (int i = 0; i < 4; i++)
                {
                    int xN = x + _moveDirections[i, 0];
                    int yN = y + _moveDirections[i, 1];
                    int xN2 = x + 2 * _moveDirections[i, 0];
                    int yN2 = y + 2 * _moveDirections[i, 1];

                    if (!InBounds(xN) || !InBounds(yN) || !InBounds(xN2) || !InBounds(yN2))
                        continue;

                    if (board[xN2, yN2] != FigureEnum.NONE)
                        continue;

                    if (CheckersHasDifferentColor(board, x, y, xN, yN))
                    {
                        var beated = new Point(xN, yN);

                        if (kills.BranchContaintsValue(beated))
                            continue;

                        if (kills.Move == null)
                        {
                            kills.Move = new Point(x, y);
                        }

                        var beat = new KillItem
                        {
                            Move = new Point(xN2, yN2),
                            Killed = beated
                        };

                        kills.AddChild(ref beat);

                        var boardCopy = (Board)board.Clone();

                        boardCopy[xN2, yN2] = boardCopy[x, y];
                        boardCopy[x, y] = FigureEnum.NONE;

                        FindKills(boardCopy, isWhite, xN2, yN2, ref beat);
                    }
                }

            }


        }

        #endregion
       
        public static EndGameEnum CheckGameOver(Board board)
        {
            int blackPawns = 0, blackSeniors = 0, whitePawns = 0, whiteSeniors = 0;
            
            for (int i = 0; i < Board.SIZE; i++)
            {
                for (int j = 0; j < Board.SIZE; j++)
                {
                    switch(board[i,j])
                    {
                        case FigureEnum.BLACK_PAWN:
                            blackPawns++;
                            break;
                        case FigureEnum.BLACK_SENIOR:
                            blackSeniors++;
                            break;
                        case FigureEnum.WHITE_PAWN:
                            whitePawns++;
                            break;
                        case FigureEnum.WHITE_SENIOR:
                            whiteSeniors++;
                            break;
                    }
                }
            }

            int totalBlacks = blackSeniors + blackPawns;
            int totalWhites = whiteSeniors + whitePawns;

            if (totalBlacks == 0 && totalWhites > 0) return EndGameEnum.EG_WIN_WHITE;
            if (totalBlacks > 0 && totalWhites == 0) return EndGameEnum.EG_WIN_BLACK;

            if (blackPawns == 0 && whitePawns == 0 && blackSeniors == 1 && whiteSeniors == 1)
                return EndGameEnum.EG_DRAW;

            return EndGameEnum.EG_NONE;
        }


        public static Board ApplyMotion(Board board, Motion mtn, bool IsWhite)
        {
            if (mtn.IsEmpty()) return board;

            bool beated;
            var newBoard = (Board)board.Clone();
            ApplyMotion(ref newBoard, mtn, IsWhite, out beated);

            return newBoard;
        }
        
        private static void ApplyMotion(ref Board newBoard, Motion mtn, bool IsWhite, out bool beated)
        {
            beated = false;

            var steps = mtn.Moves.ToArray();
            var last = steps.GetUpperBound(0);

            int oldX, oldY, newX, newY;

            oldX = steps[0].X;
            oldY = steps[0].Y;

            newX = steps[last].X;
            newY = steps[last].Y;

            var movedChecker = newBoard[oldX, oldY];
            newBoard[oldX, oldY] = FigureEnum.NONE;
            newBoard[newX, newY] = movedChecker;

            for (int i = 1; i <= steps.GetUpperBound(0); i++)
            {
                if (ShouldBecomeSenior(steps[i].Y, IsWhite))
                {
                    if (IsWhite)
                        newBoard[newX, newY] = FigureEnum.WHITE_SENIOR;
                    else
                        newBoard[newX, newY] = FigureEnum.BLACK_SENIOR;
                }
            }

            for (int i = 0; i < last; i++)
            {
                int dX = steps[i].X > steps[i + 1].X ? -1 : 1;
                int dY = steps[i].Y > steps[i + 1].Y ? -1 : 1;

                int dist = Math.Abs(steps[i].X - steps[i + 1].X);

                for (int j = 1; j < dist; j++)
                {
                    int currX = steps[i].X + dX * j;
                    int currY = steps[i].Y + dY * j;

                    if (!newBoard.IsEmpty(currX, currY))
                    {
                        newBoard[currX, currY] = FigureEnum.NONE;
                        beated = true;
                    }
                }
            }

        }
       
        private static bool InBounds(int val)
        {
            return val >= 0 && val < Board.SIZE;
        }

        private static bool ShouldBecomeSenior(int y, bool isWhite)
        {
            return y == 7 && !isWhite || y == 0 && isWhite;
        }

        private static bool CheckersHasSameColor(Board board, int x, int y, int xN, int yN)
        {
            return (board.IsWhite(x, y) && board.IsWhite(xN, yN) ||
                (board.IsBlack(x, y) && board.IsBlack(xN, yN)));
        }

        private static bool CheckersHasDifferentColor(Board board, int x, int y, int xN, int yN)
        {
            return (board.IsWhite(x,y) && board.IsBlack(xN, yN) || 
                (board.IsBlack(x,y) && board.IsWhite(xN, yN)));
        }

    }
}
