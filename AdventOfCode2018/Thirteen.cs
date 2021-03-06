﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Thirteen
    {
        public static Dictionary<Point, char> Tracks { get; set; } = new Dictionary<Point, char>();
        public static string Input = ThirteenInput.FullInput;
        //public static string Input = ThirteenInput.TestInput;
        //public static string Input = ThirteenInput.TestInput2;
        public static List<MineCart> Minecarts { get; set; } = new List<MineCart>();


        public static void Part1()
        {
            LoadInput();

            bool collision = false;
            while (!collision)
            {
                //DrawTrack();
                //Thread.Sleep(1000);

                foreach (MineCart m in Minecarts.OrderBy(m => m.Vector.Position.Y).ThenBy(m => m.Vector.Position.X))
                {
                    m.Tick();

                    var group = Minecarts.GroupBy(m2 => m2.Vector.Position).Where(g => g.Count() > 1);
                    if (group.Count() > 0)
                    {
                        Point p = group.OrderBy(g => g.Key.Y).First().Key;
                        Console.WriteLine($"Collision at {p}");
                        collision = true;
                        break;
                    }
                }

            }
        
        }

        public static void Part2()
        {
            LoadInput();

            bool multipleCarts = true;
            while (multipleCarts)
            {
                //DrawTrack();
                //Thread.Sleep(1000);

                foreach (MineCart m in Minecarts.OrderBy(m => m.Vector.Position.Y).ThenBy(m => m.Vector.Position.X))
                {
                    m.Tick();

                    if (Minecarts.Count == 1)
                    {
                        multipleCarts = false;
                        break;
                    }

                    var groups = Minecarts.GroupBy(m2 => m2.Vector.Position).Where(g => g.Count() > 1);
                    if (groups.Count() > 0)
                    {
                        Point p = groups.OrderBy(g => g.Key.Y).First().Key;
                        Console.WriteLine($"Collision at {p}");
                        foreach (IGrouping<Point, MineCart> g in groups) Minecarts.RemoveAll(mc => g.Select(q => q.ID).Contains(mc.ID));
                    }

                }

            }

            Console.WriteLine($"Final cart is at {Minecarts.First().Vector.Position}");
        }


        private static void DrawTrack()
        {
            int width = Input.IndexOf('\n');
            int height = Input.Count(c => c=='\n');
            if (width > 80 || height > 80) return;
            for(int y=0; y<= height; y++)
            {
                Console.Write("   ");
                for(int x=0; x<= width; x++)
                {
                    char c = ' ';
                    if (Tracks.ContainsKey(new Point(x, y))) c = Tracks[new Point(x, y)];

                    if (Minecarts.Any(m => m.Vector.Position.Equals(new Point(x, y)))) c = '*';
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }

        public static void LoadInput()
        {
            int x = 0;
            int y = 0;
            foreach (char c in Input)
            {
                if (c == '\n') { y++; x = 0; continue; }
                else if (c == '>' || c == '<')
                {
                    Tracks.Add(new Point(x, y), '-');
                    Minecarts.Add(MineCart.GetMineCart(c, new Point(x, y)));
                }
                else if (c == '^' || c == 'v')
                {
                    Tracks.Add(new Point(x, y), '|');
                    Minecarts.Add(MineCart.GetMineCart(c, new Point(x, y)));
                }
                else if (c == ' ') { }
                else Tracks.Add(new Point(x, y), c);

                x++;
            }
        }
    }

    public class MineCart
    {
        public int ID { get; }
        public Vector Vector { get; set; }
        private int IntersectionTurnTracker { get; set; }

        public void Tick()
        {
            Vector = Vector.GetVectorAt(1);
            char trackValue = Thirteen.Tracks[Vector.Position];
            TurnActions[trackValue]();
        }

        public static int CartId = 0;
        public static MineCart GetMineCart(char c, Point location)
        {
            Vector v;
            switch(c)
            {
                case '>':
                    v = new Vector(location, new Point(1, 0));
                    break;
                case '^':
                    v = new Vector(location, new Point(0, -1));
                    break;
                case '<':
                    v = new Vector(location, new Point(-1, 0));
                    break;
                case 'v':
                    v = new Vector(location, new Point(0, 1));
                    break;
                default:
                    throw new Exception($"Bad minecart char {c}");
            }

            return new MineCart(v, CartId++);
        }

        public void DoIntersectionTurn()
        {
            if (IntersectionTurnTracker == 0) TurnLeft();
            else if (IntersectionTurnTracker == 1) GoStraight();
            else if (IntersectionTurnTracker == 2) TurnRight();
            if (++IntersectionTurnTracker > 2) IntersectionTurnTracker = 0;
        }

        private void TurnLeft()
        {
            if (Vector.Velocity.X != 0 && Vector.Velocity.Y != 0) throw new Exception("Minecarts can't go diagonal!");
            if (Math.Abs(Vector.Velocity.X) > 1 || Math.Abs(Vector.Velocity.Y) > 1) throw new Exception("Minecarts can't go faster than 1!");

            if(Vector.Velocity.X == 0)
            {
                if (Vector.Velocity.Y == -1) Vector.Velocity = new Point(-1, 0);
                else Vector.Velocity = new Point(1, 0);
            }
            else
            {
                if (Vector.Velocity.X == -1) Vector.Velocity = new Point(0, 1);
                else Vector.Velocity = new Point(0, -1);
            }
        }

        private void TurnRight()
        {
            if (Vector.Velocity.X != 0 && Vector.Velocity.Y != 0) throw new Exception("Minecarts can't go diagonal!");
            if (Math.Abs(Vector.Velocity.X) > 1 || Math.Abs(Vector.Velocity.Y) > 1) throw new Exception("Minecarts can't go faster than 1!");

            if (Vector.Velocity.X == 0)
            {
                if (Vector.Velocity.Y == -1) Vector.Velocity = new Point(1, 0);
                else Vector.Velocity = new Point(-1, 0);
            }
            else
            {
                if (Vector.Velocity.X == -1) Vector.Velocity = new Point(0, -1);
                else Vector.Velocity = new Point(0, 1);
            }
        }

        private void GoStraight()
        { // do nothing 
        }

        // '/'
        private void FSCurve()
        {
            if (Math.Abs(Vector.Velocity.Y) == 1) TurnRight();
            else if (Math.Abs(Vector.Velocity.X) == 1) TurnLeft();
            else throw new Exception("Bad Forward Slash curve");
        }


        // '\'
        private void BSCurve()
        {
            if (Math.Abs(Vector.Velocity.Y) == 1) TurnLeft();
            else if (Math.Abs(Vector.Velocity.X) == 1) TurnRight();
            else throw new Exception("Bad Back Slash curve");
        }

        Dictionary<char, Action> TurnActions = new Dictionary<char, Action>();

        public MineCart(Vector vector, int id)
        {
            ID = id;
            Vector = vector;
            IntersectionTurnTracker = 0;

            TurnActions = new Dictionary<char, Action>
            {
                { '|', GoStraight },
                { '-', GoStraight },
                { '/', FSCurve},
                { '\\', BSCurve},
                { '+', DoIntersectionTurn},
            };
        }
    }

    public static class ThirteenInput
    {
        public static string TestInput = 
@"/->-\        
|   |  /----\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/";

        public static string TestInput2 =
@"/+++--++--\|
||||  ||  ||
||||  ||  ||
||||  ||  ||
v|||  ||  ||
||||  ||  ||
||||  ||  ||
||||  ||  ||
++++--++--++
|\++--++--++";

        public static string FullInput =
@"                                      /<------------->-----------------------------------\        /---------------------------------------------\     
                                      |     /--------------------------------------------+--------+-----------------<----------\                |     
                                /-----+-----+-------------\                 /------------+--------+----------------------------+----------\     |     
          /---------------------+-----+-----+-------------+-------\         |            |        |                            |          |     |     
/---------+---------------------+-----+----\|             |       |         |            |        |  /-------------------------+--\       |     |     
|         |         /-----------+-----+----++-\           |       |    /----+------------+--------+--+------\   /-----------\  |  |       |     |     
|         |         |           |     |    || |           |       |    |    |            |       /+--+------+---+-----------+-\|  |       |     |     
|         |         |     /-----+-----+----++-+-----------+-------+----+----+------------+-------++--+------+---+-----------+-++-\|       |     |     
|         |  /------+-----+-----+-----+----++-+-----------+-------+---\|    |            |       || /+------+---+-----------+-++-++-------+-----+-\   
|         |  |      |     |/----+-----+----++-+-----------+-------+---++----+------------+-----\ || ||      |   |           | || ||       |     | |   
|         |  |      |    /++----+-----+----++-+-----------+---\   |   ||    |            |     | || ||      |   |           | || ||      /+-----+-+-\ 
|/--------+--+------+----+++----+-----+----++-+-----------+---+---+---++\   |            |     | || ||      |   |           | || ||      ||     | | | 
||        |  |      |/---+++----+---\ |/---++-+-----------+---+---+---+++---+------------+-----+-++-++------+---+-----------+-++-++------++-\   | | | 
||        |  |      ||   |||    |   | ||   || |           |   |   |   |||   |            |     | || ||      |   |           | || ||      || |   | | | 
||        |  |      ||   |||    |/--+-++---++-+\         /+---+---+---+++---+------------+-----+-++-++----\ |   |    /------+-++-++-----\|| |   | | | 
||        |  |      ||   |||    ||  | ||   || ||         ||   |   |   |||   |            |     | || ||    | |   |    |      | || ||     ||| |   | | | 
||        |  |      ||   |||/---++--+-++---++-++---------++---+---+--\|||   | /----------+-----+-++-++----+-+---+----+---\  | || ||     ||| |   | | | 
\+--------+--+------++---++++---++--+-++---/| ||         ||   |   |  ||||   | |          |     | || ||    | |   |    |   |  | || ||     ||| |   | | | 
 |        |  |      ||   ||||  /++--+-++----+-++---------++---+---+--++++---+-+----------+-----+-++-++----+-+---+----+---+-\| || ||     ||| |   | | | 
 |       /+--+------++---++++--+++--+-++----+-++---------++---+---+\ ||||   | |          |     | || ||    | |   |    |   | || || ||     ||| |   | | | 
 |       ||  |      ||   ||||  |||  | ||    | ||         ||   |   || ||||   | |          |     | || ||    | |   |    |   | || || ||     ||| |   | | | 
 | /-----++--+------++---++++--+++\ | ||    | ||         ||   |   || ||||   | |          |     | \+-++----+-+---+----+---+-++-/| ||     ||| |   | | | 
 | |     ||  |      ||   ||||  |||| | ||    | ||         ||   |   || ||||   | |   /------+---\ | /+-++----+-+--\|    |   | ||  | ||     ||| |   | | | 
 | |     ||  |   /--++---++++--++++-+-++----+-++---------++---+---++-++++---+-+---+------+---+\| || ||    | |  ||    |   | ||  | ||     ||| |   | | | 
 | |     ||  |   |  ||   ||||  |||| | ||   /+-++---------++---+---++-++++---+-+---+------+---+++-++-++----+-+--++----+---+-++--+-++-\   ||| |   | | | 
 | |     ||  |   |  ||   |\++--++++-+-++---++-++---------++---+---++-++++---+-+---+------+---+++-++-++----+-+--++----+---+-++--+-/| |   ||| |   | | | 
 | |     ||  |   |  || /-+-++--++++-+-++---++-++---------++---+---++-++++---+-+\  |      |   ||| |\-++----+-+--++----+---+-++--+--+-+---+++-+---/ | | 
 | |     ||  |   |  || | | ||  |||| | ^|   || ||         ||   |   || ||||   | ||  |      |   ||| |  ||    | |  ||    |   | ||  |  | |   ||| |     | | 
 | |     |\--+---+--++-+-+-++--++++-+-++---++-++---------++---+---/| ||||   | ||  |      |   ||| |  || /--+-+--++--\ |   | ||  |  | |   ||| |     | | 
 | |     |   |/--+--++-+-+-++--++++-+-++---++-++---------++---+----+-++++---+-++--+-----\|   ||| |  \+-+--+-+--++--+-+---+-++--+--+-+---+++-+-----/ | 
 | |     |   ||  |  || | | ||  |||| | ||   || ||         ||   |/---+-++++---+-++--+-----++---+++-+---+-+--+-+--++--+-+---+-++--+--+-+---+++-+-------+\
 | |     |   ||  |  || | | ||  |||| | ||   || ||      /--++---++---+-++++---+-++--+-----++---+++-+---+-+--+-+--++--+-+---+-++--+--+\|   ||| |       ||
 | |     |   ||  |  || | | ||  |||| | ||   || ||      |  \+---++---+-++++---+-++--+-----++---+++-+---+-+--/ |  ||  | |   | ||  |  |||   ||| |       ||
 | |     |/--++--+--++-+\| ||  |||| | ||   || ||  /---+---+---++---+-++++---+-++--+-----++---+++-+--\| |    |  ||  | |   | ||  |  |||   ||| |       ||
 | |     ||  ||  |  || ||| ||  |||| |/++---++-++--+---+---+---++---+-++++---+-++--+-----++---+++-+--++-+----+--++--+-+---+-++--+--+++---+++-+-\     ||
 | |     ||  ||  |  || ||| ||  |||| ||||   || ||  |   |   |   ||   | ||||   | ||  |     ||  /+++-+--++-+----+--++--+-+---+-++--+--+++\  ||| | |     ||
 | |     ||  ||  |  || ||| |\--++++-++++---++-++->+---+---+---++---+-/|||   | ||  |     ||  |||| |  || |    |  ||  | |   v ||  |  ||||  ||| | |     ||
 | |   /-++--++--+--++\||| |   |||| ||||   || ||  |/--+---+---++---+--+++---+-++--+-----++--++++-+--++-+----+\ ||  | |   | ||  |  ||||  ||| | |     ||
 | |   | ||  ||  |  |||||| |   |||| ||||   || ||  ||  |   |   ||  /+--+++---+-++--+-\   ||  |||| |  || |    || ||  | |   | ||  |  ||||  ||| | |     ||
 | |   | ||  ||  |  |||||| |   |||| ||||   || ||  ||  |   |   ||  ||  |||   |/++--+-+---++\/++++-+--++-+----++-++--+-+---+-++\ | /++++--+++\| |     ||
 | |   | ||  ||  |  |||||| |   |||| ||||/--++-++--++--+---+---++--++--+++---++++--+-+---++++++++-+--++-+----++\||  | |   | ||| | |||||  ||||| |     ||
 | |   | ||/-++--+--++++++-+---++++-+++++--++-++--++--+---+---++--++--+++---++++--+-+---++++++++-+--++-+----+++++-\| |   | ||| | |||||  ||||| |     ||
 | |   | ||| ||  | /++++++-+---++++-+++++--++-++--++--+---+---++--++--+++---++++--+-+---++++++++-+--++\|    ||||| || |   | ||| | |||||  ||||| |     ||
 | |/--+-+++-++--+-+++++++-+---++++-+++++--++-++--++--+---+---++--++--+++---++++\ | |   |||||||| |  ||||    ||||| || |   | ||| | |||||  ||||| |     ||
 \-++--+-+++-++--+-+++++++-+<--++++-+++++--++-++--++--+---+---++--++--++/   ||||| | |   |||||||| |  |\++----+++++-++-+---+-+++-+-+/|||  ||||| |     ||
   ||  | ||| ||  | ||||||| |   \+++-+++++--++-++--++--+---+---++--++--++----+++++-+-+---++++++++-+--+-++----+++++-++-+---+-/|| | | |||  ||||| |     ||
   ||  | ||| ||  | ||||||| |    ||| |||||  || ||/-++--+---+---++--++--++--\ ||||| | |   ||||||||/+--+-++----+++++-++\|   |  || | | |||  ||||| |     ||
   ||  | ||| \+--+-+++++++-+----+++-+++++--++-+++-++--+---+---++--++--/|  | ||||| | |   ||||||||||  | ||    ||||\-++++---+--/| | | |||  ||||| |     ||
   ||  | |||  |  | ||||||\-+----+++-+++++--++-+++-++--+---+---/|  ||   |  | ||||| | |   ||||||||||  | |\----++++--+/||   |   | | | |||  |\+++-+-----/|
   ||  | ||^  |  | ||||||  |    |||/+++++--++-+++-++--+---+----+--++---+--+\||||| | |/--++++++++++--+-+-----++++--+-++---+---+-+-+\|||  | ||| |      |
   ||  | |||  |  | ||||||  |    |||||||||  || ||| ||  |   |    |  ||   \--+++++++-+-++--++++++++++--+-+-----/|||  | v|   |   | | |||||  | ||| |      |
   ||  | |||  |  | ||||||  |   /+++++++++--++-+++-++--+---+----+--++------+++++++-+-++--++++++++++--+-+------+++--+-++---+---+-+-+++++-\| ||| |      |
   ||  | |||  |  | |\++++--+---++++++++++--++-/|| ||  |   |    |  ||      ||||||| | ||  ||||||||||  | |      |||  | ||/--+---+-+\||||| || ||| |      |
   ||/-+-+++--+--+-+-++++--+---++++++++++--++--++-++--+---+----+-\||      ||||||| | ||  ||||||||||  | |      |||  | |||  |   | ||||||| || ||| |      |
   ||| | |||  |  | | ||||  |   ||||||||||/-++--++-++--+---+----+-+++------+++++++-+-++--++++++++++--+-+------+++--+-+++--+---+-+++++++-++-+++-+-----\|
   ||| | |||  |  | | |||v  |   ||||||||||| ||  ||/++--+---+\   | |||      ||||||| | ||  ||||||||||  | |      |||  | |||  |   | ||||||| || ||| |     ||
   ||| | |||  |  | | ||||  |   |||||||\+++-++--+++++--+---++---+-+++------+++++++-+-++--+/||||||||  | |      |||  | |||  |   | ||||||| || ||| |     ||
   ||| | |||  |  | | ||||  |   ||||||| ||| ||/-+++++--+---++---+-+++------+++++++\| ||  | ||||||||  | |      |||  | |||  |   | ||||||| || ||| |     ||
   ||| | |||  |  | | ||||/-+---+++++++-+++-+++-+++++--+---++---+-+++------+++++++++-++--+-++++++++--+-+------+++--+-+++\ |   | ||||||| || ||| |     ||
   ||| | |||  |  | | ||||| \---+++++++-+++-+++-+++++--+---++---+-+++------+++++++++-++--+-+++++/||  | |      |||  | |||| |   | ||||||| || ||| |     ||
   ||| | |||  |  \-+-+++++-----+++++++-+++-+++-+++++--+---++---+-+++------+++++++++-++--+-++++/ ||  | |      |||  | |||| |   | ||||||| || ||| |     ||
   ||| | |||  |    | |||||     ||||||| ||| ||| |||||  |   ||   | |||      ||||||||| ||  | ||||  ||  | |      |||  | |||| |   | ||||||| || ||| |     ||
   ||| | |||  |    | |||||     ||||\++-+++-+++-+++++--+---++---+-+++------+/||\++++-++--+-++++--++--+-+------+++--+-++++-/   | ||||||| || ||| |     ||
/--+++-+-+++--+----+-+++++-----++++-++-+++-+++-+++++--+---++\  | |||     /+-++-++++-++--+-++++--++--+-+------+++--+-++++-----+-+++++++-++-+++\|     ||
|  ||| | |||  |    | |||||     |||| || ||| ||| |||||  |   |||  | |||     || || |||| ||  | ||||  ||  | |      |||  | ||||     | ||||||| || |||||     ||
|  ||| | |||  |    | |||||     |\++-++-+++-+++-+++++--+---/||  | |||     || \+-++++-++--+-++++--++--+-+------+++--+-++++-----+-+++++++-++-/||||     ||
|  ||| | |||  |/---+-+++++-\   | || || ||| ||| ||||| /+----++--+-+++-----++--+-++++-++--+-++++--++--+\|      |||  | |\++-----+-+++++++-+/  ||||     ||
|  ||| | |||  ||   | ||||| |   | || || ||| ||| ||||| ||    ||  | |||     ||  | |||| ||  | ||||  ||  |||      |||  | | ||     | ||||||| |   ||||     ||
|  ||| | |||  ||   | ||||| |   | ||/++-+++-+++-+++++-++----++--+-+++-----++--+-++++-++--+-++++--++--+++----\ |||  | | ||     | ||||||| |   ||||     ||
| /+++-+-+++--++---+-+++++-+---+-+++++-+++-+++-+++++-++----++--+-+++-----++--+-++++-++\ | ||||  ||  |||    | |||  | | ||     | ||||||| |   ||||     ||
| |||| |/+++--++--\| ||||| |   | ||||| ||| ||| ||\++-++----/|  | |||     ||  \-++++-+++-+-/|||  ||  |||    | |||  | | ||     | ||||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| ||| || || ||     | /+-+++-----++----++++-+++-+--+++--++--+++----+-+++--+-+-++\    | ||||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| |\+-++-++-++-----+-++-+++-----++----++++-+++-+--+++--++--+++----+-+++--+-+-+++----+-/|||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| | | || || ||     | || |||     ||/---++++-+++-+--+++--++--+++----+-+++-\| | |||    |  |||||| |   ||||     ||
| |||| |v|||  ||  || ||||| |   | ||||| ||| | | || || ||     | || |||     |||  /++++-+++-+--+++-\||  |||    | ||| || | |||    |  |||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| | | || || ||     | || |||     |||  ||||| ||| |  ||| |||  |||    | ||| || | |||    |  |||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| | | || || ||     | || |||     |||  ||||| ||| |  \++-+++--+++----+-+++-++-+-+++----/  |||||| |   ||||     ||
| |||| |||||  ||  || ||||| |   | ||||| ||| | | || || ||     | || |||     |||  ||||| ||| |   || |||  |||    | ||| || | |||       |||||| |   ||||     ||
| |||\-+++++--++--++-+++++-+---+-+++++-+++-+-+-++-++-++-----+-++-/||     |||  ||||| ||| |   || |||  |||    | ||| || | |||       |||||| |   ||||     ||
| |||  ||\++--++--++-+++++-+---+-+++++-+++-+-+-++-++-++-----+-++--+/     |||  ||||| ||| |   || |||  |||    |/+++-++-+-+++-------++++++-+--\||||     ||
| |||  || ||  ||  || ||||| |   | ||||| ||| \-+-++-++-++-----+-++--+------+++--+++++-+++-+---++-+++--+++----+++++-++-+-+++-------++++/| |  |||||     ||
| |||  || ||  ||  || ||||| |   | ||||| |||   | || || ||     | ||  |      |||  ||||| ||| |   || |\+--+++----+++++-++-/ \++-------/||| | |  |||||     ||
| |||  || ||  ||  || ||||| |   | ||||| |||/--+-++-++-++-----+-++--+\     |||  ||||| ||| |   || | |  |||    ||||| ||    ||        ||| | |  |||||     ||
| |||  || ||/-++--++-+++++-+-\ | ||||| ||||  | || || ||     | ||  ||     |||  \++++-+++-+---++-/ |  |||    ||||| ||    ||        ||| | |  |||||     ||
| |||  || ||| \+--++-+++++-+-+-+-+++++-++++--+-++-++-++-----+-++--++-----+++---++++-+++-/   ||   |  |||    ||||| ||    ||        ||| | |  |||||     ||
| |||  || |||  |  || \++++-+-+-+-+++/| ||||  | || || ||     | ||  ||     |||   |||| |||     ||   |  |||    ||||| ||    ||        ||| | |  |||||     ||
| |||  || |||  |  || /++++-+-+-+-+++-+-++++--+-++-++\||     | ||  ||     |||   |||| |||     ||   |  ||| /--+++++-++----++--\     ||| | |  |||||     ||
| |||  |\-+++--+--/| ||||| | | | ||| | ||||  | || |||||     | |\--++-----+++---++++-+++-----++---+--+++-+--+++++-++----++--+-----+++-+-+--+++++-----+/
| |||  |  |||  |   | ||||| | | | ||| | ||||  | || |||||     | |   ||     |||   |||| |||     ||   |  ||| |  ||||| ||    ||  |     ||| | |  |||||     | 
| |||  | /+++--+---+-+++++-+-+-+-+++-+-++++--+-++-+++++-----+-+---++-\   |||   |||| |||     ||   |  ||| |  ||||| ||    ||  |     ||| | |  |||||     | 
| |||  | ||||  |   | ||||| | | | ||| | ||||  | || |||\+-----+-+---++-+---+++---++++-+++-----++---+--+/| |  ||||| ||    ||  |     ||| | |  |||||     | 
| |||  | ||||  |   | ||||| |/+-+-+++-+-++++--+-++-+++-+-----+-+---++-+---+++---++++-+++-----++---+--+-+-+--+++++-++\   ||  |     ||| | |  |||||     | 
| |||  | ||||  |   | ||||| ||| | ||| | ||||  | || ||| |     | |   ||/+---+++---++++-+++-----++---+--+-+-+-\||||| |||   ||  |/----+++-+\|  |||||     | 
| |||  | ||v|  |   | ||||| ||| | ||| | ||||  | || ||| |     | |   ||||   |||/--++++-+++-----++-\ |  | | | |||||| |||   ||  ||   /+++-+++-\|||||     | 
| ||| /+-++++--+---+-+++++-+++-+-+++-+-++++--+-++-+++-+-----+-+---++++---++++--++++-+++--\  || | |  | | | |||||| |||   || /++---++++-+++-++++++--\  | 
| ||| || ||||  |   | |||v| ||| | ||| | ||||  | || ||| |     | |   ||||   ||||  |||| |||  |  || | |  | | | |||||| |||   || |||   |\++-+++-++/|||  |  | 
| ||| || ||||  |   | ||||| ||| | ||| | ||\+--+-++-+++-+-----+-+---++++---++++--++++-+++--+--++-+-+--+-+-+-++++++-+++---++-+++---+-++-+++-++-+++--+--/ 
| ||| || ||||  |   | ||||| ||| | ||| | || |  | || ||| |     | |   ||||   ||||  |||| |||  |  || | |  | | | |||||| |||   || |||   | || ||| || |||  |    
| ||| || ||||  |   | ||||| ||| | ||| | || |  | || ||| |     | |   ||||   ||||  |||| |||  |  || | |  | | | |||||| |||   || |||   | || ||| || |||  |    
| ||| || ||||  |   | ||||| ||| | ||| | \+-+--+-++-+++-+-----+-+---++++---++++--++++-+++--+--++-+-+--+-+-+-++++++-+++---++-+++---+-++-+++-++-/||  |    
| ||| || ||||  |   \-+++++-+++-+-+++-+--+-+--+-++-+++-+-----+-+---++++---++++--++++-+++--+--++-+-+--+-/ | |||||| |||   || |||   | || ||| ||  ||  |    
| ||| |\-++++--+-----+/||| ||| | ||| |  | |  | || |\+-+-----+-+---++++---++++--++++-+++--+--++-+-+--+---+-+++/|| |||   || |||   | || ||| ||  ||  |    
| ||| |  ||||  |     | ||\-+++-+-+++-+--+-+--+-++-+-+-+-----+-+---++++---++++--++++-+++--+--++-+-+--+---+-+++-++-+++---/| ||\---+-++-+/| ||  ||  |    
| ||| |  ||||  |     | ||  ||| | ||| \--+-+--+-++-+-+-+-----+-+---++++---++++--++++-+++--+--++-+-+--+---+-+++-++-+++----+-++----+-++-+-+-++--+/  |    
| ||| |  ||||  |     | ||  ||| | |||    | \--+-++-+-+-+-----+-+---+/|| /-++++--++++-+++-\|  || | |  |   | ||| || |||    | ||    | || | | ||  |   |    
| |\+-+--++++--+-----+-++--+++-+-+/|    |    | || | | |     | |   | || | ||||  |||| ||| ||  || | | /+---+-+++-++-+++--\ | ||    | || | | ||  |   |    
| | \-+--++++--+-----+-++--+++-+-+-+----+----+-++-+-+-+-----+-+---+-++-+-++++--+/|| ||| ||  || | | ||   | ||| || |||  | | ||    | || | | ||  |   |    
| |   |  ||||  |     | ||  ||| | | |    |    | || | | |     | |   | || | ||||  | || ||| ||  || |/+-++---+-+++-++-+++--+-+-++----+-++-+\| ||  |   |    
| |   |  ||||  |     | ||  ||| | | |    |    | || | | \-----+-+---+-++-+-++++--+-++-+++-++--++-+++-++---+-+++-++-+++--+-+-++----+-+/ ||| ||  |   |    
| |   |  ||||  |     | ||  ||| | | |    |    | || | |       | |   | || | ||||  |/++-+++-++--++-+++-++---+-+++-++\|||  | | ||    \-+--+++-/|  |   |    
| |   |  ||||  |     | ||  ||| | | |    \----+>++-+-+-------+-+---+-++-+-++++--++++-+++-++--++-+++-++---+-+++-/|||||  | | ||      |  |||  |  |   |    
|/+---+--++++--+-----+-++--+++-+-+-+---------+-++-+-+-------+-+---+-++-+-++++--++++-+++-++--++\||| ||   | |||  |||||  | | ||      |  |||  |  |   |    
|||   |  ||||  |     | ||  ||| | \-+---------+-/| | |       | |   | || | ||||  |||\-+++-++--+/|||\-++---+-+++--/||||  | | \+------+--+++--+--+---/    
|||   |  ||||  |     | ||  ||| |   \---------+--+-+-+-------+-+---+-++-+-++++--+++--+++-++--+-+++--++---+-+/|   ||||  | |  |      |  |||  |  |        
|||  /+--++++--+-----+-++--+++-+----------\  |  | | |       | |   | || | ||\+--+++--+++-++--+-+++--++---+-+-+---+/||  | |  |      |  |||  |  |        
|||/<++--++++--+-----+-++--+++-+----------+--+--+\| |       | |   | || | || |  |||  ||| ||  | |||  ||   \-+-+---+-++--+-+--/      |  |||  |  |        
|||| ||  |||| /+-----+-++--+++-+-------\  |  |  ||| |  /----+-+---+-++-+-++-+--+++--+++-++--+-+++--++-----+-+---+-++--+-+---------+--+++--+-\|        
\+++-++--++++-++-----+-++--+++-+-------+--+--+--+++-+--+----/ |   | || | || |  |||  ||| ||  \-+++--++-----+-+---+-++--+-+---------+--/||  | ||        
 ||| ||  |||| ||     | ||  ||| \-------+--+--+--+++-+--+------+---+-++-+-++-+--+++--+++-++----+++--++-----+-+---+-++--+-+---------+---+/  | ||        
 ||| ||  ||\+-++-----+-++--+++---------+--+--+--+++-+--+------+---+-++-+-++-+--+++--+++-++----+++--++-----+-+---+-/|  | |         |   |   | ||        
 ||| ||  || | ||     | ||  |||         |  |  |  ||| |  |      |   | || | || \--+++--+++-++----+/|  ||     | |   |  |  | |         |   |   | ||        
 |||/++--++-+-++-----+-++--+++-\       |  |  \--+++-+--+------+---+-++-+<++----++/  ||| ||    | |  ||     | |   |  |  | |         |   |   | ||        
 ||||||  || | ||     | ||  ||| |       |  |     ||| |  |      |   | || | ||    ||   ||| ||    | \--++-----+-+---+--+--+-+---------+---/   | ||        
 ||||||  || | ||     | ||  ||| |       |  |     ||| |  |      |   | \+-+-++----++---+++-++----+----++-----/ |   |  |  | |         |       | ||        
 ||||||  || | ||     | ||  ||| |       |  |     ||| |  |      |   |  | | ||    ||   ||| ||    |    ||       |/--+--+--+-+---------+-------+-++-----\  
 \+++++--++-+-++-----+-++--+++-+-------+--+-----+++-+--+------+---+--+-+-++----++---+++-++----/    ||       ||  |  |  | |         |       | ||     |  
  |||||  ||/+-++-----+-++--+++-+-------+--+-----+++-+--+------+---+--+\| ||    ||   ||| ||         ||       ||  |/-+--+-+---------+---\   | ||     |  
  |||||  |||| ||     | ||  ||| |       |  |     ||| |  |      |   |  ||| ||    ||   ||| ||         ||       ||  || |  | |         |   |   | ||     |  
  |||||  |||| \+-----+-++--+++-+-------/  |     ||| |  |      |   |  ||| ||    ||   ||| ||         ||       ||  || |  | |         |   |   | ||     |  
  |||||  ||||  |     | ||  |\+-+----------+-----+++-+--+------+---+--+++-++----++---+++-++---------++-------++--++-/  | |         |   |   | ||     |  
  \++++--++++--+-----+-++--+-+-+----------+-----+++-+--+------+---+--+++-++----++---++/ ||         ||       \+--++----+-+---------+---+---/ ||     |  
   ||||  ||||  |     | ||  | | |       /--+-----+++-+--+------+---+--+++-++----++---++--++---------++---\    |  ||    | |         |   |     ||     |  
   ||\+--++++--+-----+-++--+-+-+-------+--/     ||| |  |      |   |  ||| ||    ||   ||  ||         ||   |    |  ||    | |         |   |     ||     |  
   || |  ||||  |     | \+--+-+-+-------+--------+++-+--+------+---+--+++-++----/|   |\--++---------++---+----+--++----+-+---------/   |     ||     |  
   || |  ||||  |     |  |  | | |       |        ||\-+--+------+---+--+++-++-----+---+---++---------+/   |    \--++----+-+-------------+-----++-----/  
   || |  ||||  |     |  |  | | |       |        ||  |  |      |   |  ||| ||     |   |   ||         |    |       ||    | |             |     ||        
   || |  ||||  |     |  |  | | |       |        \+--+--+------+---+--+++-+/     |   |   ||         |    |       ||    | |             |     ||        
   \+-+--++++--+-----+--+--+-+-+-------+---------/  |  |      \---+--+++-+------+---+---++---------+----+-------++----+-/             |     ||        
    \-+--++++--+-----+--+--+-+-/       |            |  |     /----+--+++-+------+---+---++---\     \----+-------++----/               |     ||        
      |  ||||  |     |  |  | |         |            |  |     |    |  ||| |      \---+---++---+----------+-------/|                    |     ||        
      |  |\++--+-----+--/  | |         |            |  |     |    |  ||| |          |   ||   |          |        \--------------------/     ||        
      |  | ||  |     |     | |         |            |  |     |    \--+++-+----------/   ||   |          |                                   ||        
      |  | ||  |     |     | |         |            |  |     |       ||\-+--<-----------/|   |          |                                   ||        
      |  | |\--+-----+-----+-/         \------------+--+-----+-------++--+---------------+---+----------/                                   ||        
      \--+-+---+-----+-----+------------------------+--+-----+-------++--+---------------/   |                                              ||        
         | |   \-----+-----/                        |  \-----+-------++--+-------------------+----------------------------------------------/|        
         \-+---------+------------------------------+--------+-------/|  \-------------------+-----------------------------------------------/        
           \---------+------------------------------+--------+--------/                      |                                                        
                     \------------------------------/        |                               |                                                        
                                                             \-------------------------------/                                                        ";
    }



}
