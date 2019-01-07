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
            string foo = "a";
            string bar = null;
            int comp = foo.CompareTo(bar);
            comp = bar.CompareTo(foo);



            List<string> Input;
            //Input = SeventeenInputs.TestInput;
            Input = SeventeenInputs.FullInput;
            Slice slice = Slice.GetSlice(Input);
            //Console.Write(slice.GetPrintableString());

            
        }
    }

    public class Water
    {
        public Point CurrentPosition { get; set; }
        public Point LastDownPosition { get; set; }
        public HashSet<Point> Visited { get; set; } = new HashSet<Point>();

        public void Travel(Slice slice)
        {
            CurrentPosition = slice.WaterSource;
            while(slice.Rect.Contains(CurrentPosition))
            {
                if(slice.Cells[CurrentPosition].Down.WaterPermiable)
                {
                    LastDownPosition = CurrentPosition;
                    CurrentPosition = new Point(CurrentPosition.X, CurrentPosition.Y + 1);
                }
                else if (slice.Cells[CurrentPosition].Left.WaterPermiable && !Visited.Contains(new Point(CurrentPosition.X - 1, CurrentPosition.Y)))
                {
                    CurrentPosition = new Point(CurrentPosition.X - 1, CurrentPosition.Y);
                }
                else if (slice.Cells[CurrentPosition].Right.WaterPermiable && !Visited.Contains(new Point(CurrentPosition.X + 1, CurrentPosition.Y)))
                {
                    CurrentPosition = new Point(CurrentPosition.X + 1, CurrentPosition.Y);
                }
                else
                {
                    CurrentPosition = LastDownPosition;
                }
                Visited.Add(CurrentPosition);
            }
        }
    }

    public class Slice
    {
        private Slice() { }

        public Dictionary<Point, Cell> Cells = new Dictionary<Point, Cell>();
        private Point _min;
        private Point _max;
        private Rect _rect;
        private Point _waterSource = new Point(500, 0);

        public Point WaterSource => _waterSource;
        public Rect Rect => _rect;

        public static Slice GetSlice(List<string> input)
        {
            //"x=504, y=10..13",
            //"y=13, x=498..504",
            Regex regex = new Regex(@"([xy])=([0-9]+), ([xy])=([0-9]+)\.\.([0-9]+)");
            Slice slice = new Slice();
            slice.Cells.Add(slice._waterSource, new WaterSourceCell(slice._waterSource));
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
                        if (slice.Cells.ContainsKey(p)) continue;
                        slice.Cells.Add(p, new ClayCell(p));
                    }
                }
                else
                {
                    for (int x = lineStartPos; x <= lineEndPos; x++)
                    {
                        Point p = new Point(x, lineHorizonPos);
                        if (slice.Cells.ContainsKey(p)) continue;
                        slice.Cells.Add(p, new ClayCell(p));
                    }
                }
            }

            slice._min = new Point(slice.Cells.Min(kvp => kvp.Key.X), slice.Cells.Min(kvp => kvp.Key.Y));
            slice._max = new Point(slice.Cells.Max(kvp => kvp.Key.X), slice.Cells.Max(kvp => kvp.Key.Y));
            slice._rect = new Rect(slice._min.X - 1, slice._min.Y - 1, (slice._max.X + 2) - (slice._min.X - 1), (slice._max.Y + 2) - (slice._min.Y - 1));
            foreach(Point p in slice._rect.GetPoints())
            {
                if (!slice.Cells.ContainsKey(p))
                    slice.Cells.Add(p, new SandCell(p));
            }

            foreach(Point p in slice._rect.GetPoints())
            {
                if (p.X < slice._min.X ||
                    p.Y < slice._min.Y ||
                    p.X > slice._max.X ||
                    p.Y > slice._max.Y) continue;

                Cell cell = slice.Cells[p];
                cell.Left = slice.Cells[new Point(p.X - 1, p.Y)];
                cell.Right = slice.Cells[new Point(p.X + 1, p.Y)];
                cell.Up = slice.Cells[new Point(p.X, p.Y - 1)];
                cell.Down = slice.Cells[new Point(p.X, p.Y + 1)];
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
                sb.Append(Cells[p]);
            }
            return sb.ToString();
        }
    }



    public abstract class Cell
    {
        public Cell(Point p) { Position = p; }
        public Point Position { get; set; }
        public Cell[] Neighbors = new Cell[4];
        public Cell Left { get { return Neighbors[0]; } set { Neighbors[0] = value; } }
        public Cell Right { get { return Neighbors[0]; } set { Neighbors[1] = value; } }
        public Cell Up { get { return Neighbors[0]; } set { Neighbors[2] = value; } }
        public Cell Down { get { return Neighbors[0]; } set { Neighbors[3] = value; } }

        public abstract bool WaterPermiable { get; }

        public void Accept(Water water)
        {
            if (!WaterPermiable) return;

            water.CurrentPosition = Position;
            if (Down.WaterPermiable) Down.Accept(water);
            if (Right.WaterPermiable) Right.Accept(water);
            if (Left.WaterPermiable) Left.Accept(water);
        }
    }
    public class WaterSourceCell : Cell
    {
        public WaterSourceCell(Point p) : base(p) { }
        public override bool WaterPermiable => true;
        public override string ToString() => "+";
    }

    public class ClayCell : Cell
    {
        public ClayCell(Point p) : base(p) { }
        public override bool WaterPermiable => false;

        public override string ToString() => "#";
    }

    public class SandCell : Cell
    {
        public SandCell(Point p) : base(p) { }
        public override bool WaterPermiable => true;
        public override string ToString() => ".";
    }

    public class StandingWaterCell : Cell
    {
        public StandingWaterCell(Point p) : base(p) { }
        public override bool WaterPermiable => false;
        public override string ToString() => "~";
    }
}
