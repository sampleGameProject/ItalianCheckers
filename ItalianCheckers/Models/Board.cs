using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItalianCheckers.Models
{
    public enum FigureEnum
    {
        NONE,
        WHITE_PAWN,
        WHITE_SENIOR,
        BLACK_PAWN,
        BLACK_SENIOR
    }

    public class Board
    {
        public const int SIZE = 8;
        FigureEnum[,] items = new FigureEnum[SIZE,SIZE];

        public Board()
        {
            for (int x = 0; x < SIZE; x++)
                for (int y = 0; y < SIZE; y++)
                {
                    int dx = (y + 1) % 2;

                    if ((x + dx) % 2 != 0)
                    {
                        if (y < 3)
                            items[x, y] = FigureEnum.BLACK_PAWN;
                        else if (y > 4)
                            items[x, y] = FigureEnum.WHITE_PAWN;
                        else
                            items[x, y] = FigureEnum.NONE;
                    }
                    else
                        items[x, y] = FigureEnum.NONE;
                }
        }

        public FigureEnum this[int x, int y]
        {
            get { return items[x, y]; }
            set { items[x, y] = value; }
        }

        internal bool IsWhite(int x, int y)
        {
            return items[x, y] == FigureEnum.WHITE_SENIOR || items[x, y] == FigureEnum.WHITE_PAWN;
        }

        internal bool IsBlack(int x, int y)
        {
            return items[x, y] == FigureEnum.BLACK_PAWN || items[x, y] == FigureEnum.BLACK_SENIOR;
        }

        internal bool IsEmpty(int x, int y)
        {
            return items[x, y] == FigureEnum.NONE;
        }

        internal Board Clone()
        {
            Board b = new Board();
            for(int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    b.items[i, j] = items[i, j];
            return b;
        }

        internal bool IsSenior(int x, int y)
        {
            return items[x, y] == FigureEnum.WHITE_SENIOR || items[x, y] == FigureEnum.BLACK_SENIOR;
        }
    }
}
