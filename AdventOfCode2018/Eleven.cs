using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Eleven
    {
        static int _gridSerialNum = 5177;

        public static void Part1()
        {
            Dictionary<Point, int> powerLevels = new Dictionary<Point, int>();
            int maxPower = int.MinValue;
            Point fuelCellWithMax = null;
            for (int x = 1; x <= 301; x++)
            {
                for (int y = 1; y <= 301; y++)
                {
                    Rect rect = new Rect(x, y, 3, 3);
                    int rectPower = PowerLevel(rect, powerLevels);
                    maxPower = Math.Max(rectPower, maxPower);
                    if (maxPower == rectPower) fuelCellWithMax = new Point(x, y);
                }
            }

            Console.WriteLine($"Max power is {maxPower} at coordinate {fuelCellWithMax}");
        }

        public static void Part2()
        {
            Rect refooct = new Rect(1, 1, 300, 300);
            List<Rect> foorects = RectSplit(refooct);

            Dictionary<int, Rect> maxPowerBySize = new Dictionary<int, Rect>();
            Dictionary<Point, int> powerLevels = new Dictionary<Point, int>();
            Dictionary<Rect, int> powerLevelRects = new Dictionary<Rect, int>();
            int maxPower = int.MinValue;
            Rect rectWithMax = null;
            for (int s = 3; s <= 25; s++)
            {
                maxPower = int.MinValue;
                rectWithMax = null;
                for (int x = 1; x <= 300; x++)
                {
                    for (int y = 1; y <= 300; y++)
                    {
                        int minSide = Math.Min(300 - x, 300 - y);
                        if (minSide < s) continue;

                        //Console.WriteLine($"Doing size {s}, point {x},{y}");
                        Rect rect = new Rect(x, y, s, s);
                        int rectPower = 0;
                        foreach (Rect subRect in RectSplit(rect))
                        {
                            rectPower += PowerLevel(subRect, powerLevels, powerLevelRects);
                        }
                        maxPower = Math.Max(rectPower, maxPower);
                        if (maxPower == rectPower) rectWithMax = rect;
                    }
                }
                maxPowerBySize.Add(s, rectWithMax);
                Console.WriteLine($"Max power for size {s} is {maxPower} with rect {rectWithMax}");
            }
        }

        public static List<Rect> RectSplit(Rect rect)
        {
            if (rect.Width != rect.Height) throw new Exception("Must be done on a square");
            int size = rect.Height;
            List<Rect> rects = new List<Rect>();
            if(size % 3 == 0)
            {
                for (int dx = 0; dx < size / 3; dx++)
                {
                    for (int dy = 0; dy < size / 3; dy++)
                    {
                        rects.Add(new Rect(rect.X + (dx * 3), rect.Y + (dy * 3), 3, 3));
                    }
                }
            }
            else if(size % 4 == 0)
            {
                for (int dx = 0; dx < size / 4; dx++)
                {
                    for (int dy = 0; dy < size / 4; dy++)
                    {
                        rects.Add(new Rect(rect.X + (dx * 4), rect.Y + (dy * 4), 4, 4));
                    }
                }
            }
            else if(size % 5 == 0)
            {
                for (int dx = 0; dx < size / 5; dx++)
                {
                    for (int dy = 0; dy < size / 5; dy++)
                    {
                        rects.Add(new Rect(rect.X + (dx * 5), rect.Y + (dy * 5), 5, 5));
                    }
                }
            }
            else
            {
                rects.Add(rect);
            }

            return rects;
        }

        public static int PowerLevel(Rect rect, Dictionary<Point, int> powerLevels, Dictionary<Rect, int> powerLevelRects)
        {
            if (!powerLevelRects.ContainsKey(rect))
            {
                if (rect.Width != rect.Height) throw new Exception("Must be done on a square");
                int totalPower = 0;
                for (int dx = 0; dx < rect.Width; dx++)
                {
                    for (int dy = 0; dy < rect.Height; dy++)
                    {
                        if (rect.Point.X + dx > 300 || rect.Point.Y + dy > 300) continue;
                        totalPower += PowerLevel(new Point(rect.Point.X + dx, rect.Point.Y + dy), powerLevels);
                    }
                }
                powerLevelRects[rect] = totalPower;
            }
            return powerLevelRects[rect];
        }

        public static int PowerLevel(Rect rect, Dictionary<Point, int> powerLevels)
        {
            if (rect.Width != rect.Height) throw new Exception("Must be done on a square");
            int totalPower = 0;
            for(int dx = 0; dx<rect.Width; dx++)
            {
                for(int dy=0; dy<rect.Height; dy++)
                {
                    if (rect.Point.X + dx > 300 || rect.Point.Y + dy > 300) continue;
                    totalPower += PowerLevel(new Point(rect.Point.X + dx, rect.Point.Y + dy), powerLevels);
                }
            }
            return totalPower;
        }

        public static int PowerLevel(Point p, Dictionary<Point, int> powerLevels)
        {
            if (!powerLevels.ContainsKey(p))
            {
                int rackId = p.X + 10;
                int powerLevel = p.Y * rackId;
                powerLevel += _gridSerialNum;
                powerLevel *= rackId;
                string hundredsDigit = powerLevel.ToString("000000").Reverse().ToList()[2].ToString();
                powerLevel = int.Parse(hundredsDigit);
                powerLevel -= 5;
                powerLevels[p] = powerLevel;
            }
            return powerLevels[p];
        }
    }
}
