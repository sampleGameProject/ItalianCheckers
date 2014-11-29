using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItalianCheckers.Models
{
    public class Point
    {
        public Point()
            : this(0, 0)
        { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Point;

            if (other == null)
                return false;

            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Motion
    {
        public List<Point> Moves { get; set; }

        public Motion(params Point[] points)
        {
            Moves = new List<Point>(points);
        }

        public bool IsEmpty()
        {
            return Moves.Count == 0;
        }

        public override string ToString()
        {
            return String.Format("{0} x {1} => {2} x {3}",Moves[0].X,Moves[0].Y,Moves[1].X,Moves[1].Y);
        }

    }
}
