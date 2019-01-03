using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Seventeen
    {
        public static void Part1()
        {
            List<string> Input;
            //Input = SeventeenInputs.TestInput;
            Input = SeventeenInputs.FullInput;
            Slice slice = Slice.GetSlice(Input);
            Console.Write(slice.GetPrintableString());

            
        }
    }

    public class Slice
    {
        private Slice() { }

        private Dictionary<Point, Cell> _cells = new Dictionary<Point, Cell>();
        private Point _min;
        private Point _max;
        private Rect _rect;
        private Point _waterSource = new Point(500, 0);

        public static Slice GetSlice(List<string> input)
        {
            //"x=504, y=10..13",
            //"y=13, x=498..504",
            Regex regex = new Regex(@"([xy])=([0-9]+), ([xy])=([0-9]+)\.\.([0-9]+)");
            Slice slice = new Slice();
            slice._cells.Add(slice._waterSource, new WaterSourceCell());
            foreach(string s in input)
            {
                MatchCollection mc = regex.Matches(s);
                bool vertical = mc[0].Groups[1].Value == "x";
                int lineHorizonPos = int.Parse(mc[0].Groups[2].Value);
                int lineStartPos = int.Parse(mc[0].Groups[4].Value);
                int lineEndPos = int.Parse(mc[0].Groups[5].Value);
                if (vertical)
                {
                    for(int y = lineStartPos; y <= lineEndPos; y++) 
                    {
                        Point p = new Point(lineHorizonPos, y);
                        if (slice._cells.ContainsKey(p)) continue;
                        slice._cells.Add(p, new ClayCell());
                    }
                }
                else
                {
                    for (int x = lineStartPos; x <= lineEndPos; x++)
                    {
                        Point p = new Point(x, lineHorizonPos);
                        if (slice._cells.ContainsKey(p)) continue;
                        slice._cells.Add(p, new ClayCell());
                    }
                }
            }

            slice._min = new Point(slice._cells.Min(kvp => kvp.Key.X), slice._cells.Min(kvp => kvp.Key.Y));
            slice._max = new Point(slice._cells.Max(kvp => kvp.Key.X), slice._cells.Max(kvp => kvp.Key.Y));
            slice._rect = new Rect(slice._min.X - 1, slice._min.Y - 1, (slice._max.X + 2) - (slice._min.X - 1), (slice._max.Y + 2) - (slice._min.Y - 1));
            foreach(Point p in slice._rect.GetPoints())
            {
                if (!slice._cells.ContainsKey(p))
                    slice._cells.Add(p, new SandCell());
            }

            foreach(Point p in slice._rect.GetPoints())
            {
                if (p.X < slice._min.X ||
                    p.Y < slice._min.Y ||
                    p.X > slice._max.X ||
                    p.Y > slice._max.Y) continue;

                Cell cell = slice._cells[p];
                cell.Left = slice._cells[new Point(p.X - 1, p.Y)];
                cell.Right = slice._cells[new Point(p.X + 1, p.Y)];
                cell.Up = slice._cells[new Point(p.X, p.Y - 1)];
                cell.Down = slice._cells[new Point(p.X, p.Y + 1)];
            }
            return slice;
        }

        public string GetPrintableString()
        {
            StringBuilder sb = new StringBuilder();
            List<Point> rectPoints = _rect.GetPoints(Rect.PointIterationDirection.LeftRight);
            int yprev = rectPoints[0].Y;
            foreach (Point p in rectPoints)
            {
                if (yprev != p.Y)
                {
                    sb.Append("\n");
                    yprev = p.Y;
                }
                sb.Append(_cells[p]);
            }
            return sb.ToString();
        }
    }

    internal class WaterSourceCell : Cell
    {
        public override Vector GetWaterVector(Vector incoming)
        {
            throw new NotImplementedException();
        }
        public override string ToString() => "+";
    }

    public abstract class Cell
    {
        public Cell[] Neighbors = new Cell[4];
        public Cell Left { get { return Neighbors[0]; } set { Neighbors[0] = value; } }
        public Cell Right { get { return Neighbors[0]; } set { Neighbors[1] = value; } }
        public Cell Up { get { return Neighbors[0]; } set { Neighbors[2] = value; } }
        public Cell Down { get { return Neighbors[0]; } set { Neighbors[3] = value; } }

        public bool IsWet { get; set; }
        public bool WasWet { get; set; }
        public bool GoLeft { get; set; }
        public int GetXVec()
        {
            GoLeft = !GoLeft;
            if (GoLeft) return -1;
            else return 1;
        }

        public abstract Vector GetWaterVector(Vector incoming);
    }

    public class ClayCell : Cell
    {
        public override Vector GetWaterVector(Vector incoming)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => "#";
    }

    public class SandCell : Cell
    {

        public override Vector GetWaterVector(Vector incoming)
        {
            throw new NotImplementedException();
            /*
            if (incoming.Velocity.X != 0 && incoming.Velocity.Y != 0) throw new Exception("water can't move diagonal");

            if (IsWet && incoming.Velocity.Y == 1) return new Vector(incoming.Position, new Point(GetXVec(), 0));
            if (!IsWet && incoming.Velocity.Y == 1) return incoming;
            if ()
            */
        }
        public override string ToString() => ".";
    }
}
