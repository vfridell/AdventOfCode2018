using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public enum Direction { Left, Right, Up, Down };

    public class Six
    {
        public static void Part1()
        {
            Input = FullInput;
            Input.Sort();

            List<(Point, int)> pointAreas = new List<(Point, int)>();
            List<Point> nonInfinitePoints = NonInfiniteAreaPoints(Input);
            foreach (Point p in nonInfinitePoints)
            {
                Console.WriteLine($"Checking point {p}");
                int area = GetAreaNearestNonRecursive(p);
                pointAreas.Add((p, area));
            }

            var biggie = pointAreas.Where(pa => pa.Item2 == pointAreas.Max(pa2 => pa2.Item2)).First();
            Console.WriteLine($"Point at {biggie.Item1} has the biggest area at {biggie.Item2}");
        }

        public static void Part2()
        {
            Input = FullInput;
            Input.Sort();
            int safeRegionSize = GetSafeRegionSizeFast();
            Console.WriteLine($"Total area is {safeRegionSize}");
        }

        public static int GetSafeRegionSizeFast()
        {
            int totalArea = 0;

            int X = Input.Min(p => p.X);
            int Y = Input.Min(p => p.Y);
            int width = (Input.Max(p => p.X) - Input.Min(p => p.X));
            int height = (Input.Max(p => p.Y) - Input.Min(p => p.Y));

            Rect boundingRect = new Rect(X, Y, width, height);
            int areaEstimate = boundingRect.Width * boundingRect.Height;
            foreach(Point p in boundingRect.GetPoints())
            {
                if (PointDistanceSum(p) < 10000) totalArea++;
            }

            return totalArea;
        }

        public static int PointDistanceSum(Point referencePoint) => Input.Sum(p => ManhattanDist(referencePoint, p));

        public static void GetAreaNearest(Point referencePoint, Point visiting, List<Point> visited, ref int totalArea)
        {
            Point nearest = GetNearestPoint(Input, visiting);
            if (nearest != null && nearest.Equals(referencePoint))
            {
                totalArea++;
                Console.WriteLine($"{visiting} is nearest to {referencePoint} which makes the total {totalArea}");
            }
            else
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                Point nextPoint = new Point(visiting.X + DirectionPoints[i].X, visiting.Y + DirectionPoints[i].Y);
                if (!visited.Contains(nextPoint))
                {
                    visited.Add(nextPoint);
                    GetAreaNearest(referencePoint, nextPoint, visited, ref totalArea);
                }
            }
            return;
        }

        public static int GetAreaNearestNonRecursive(Point referencePoint)
        {
            int totalArea = 0;
            Point visiting = referencePoint;
            bool keepgoing = true;
            List<Point> areaPoints = new List<Point>();
            List<Point> finishedPoints = new List<Point>();
            List<Point> visited = new List<Point>();
            while (keepgoing)
            {
                foreach (Point nextPoint in DirectionPoints.Select(p => new Point(visiting.X + p.X, visiting.Y + p.Y)))
                {
                    if (!visited.Contains(nextPoint))
                    {
                        visited.Add(nextPoint);
                        Point nearest = GetNearestPoint(Input, nextPoint);
                        if (nearest != null && nearest.Equals(referencePoint) )
                        {
                            totalArea++;
                            areaPoints.Add(nextPoint);
                            Console.WriteLine($"{nextPoint} is nearest to {referencePoint} which makes the total {totalArea}");
                        }
                    }
                }
                finishedPoints.Add(visiting);
                visiting = areaPoints.Where(p => !finishedPoints.Contains(p)).FirstOrDefault();
                keepgoing = visiting != null;
            }
            return totalArea;
        }

        public static Point GetNearestPoint(List<Point> points, Point referencePoint)
        {
            if (points.Contains(referencePoint)) return referencePoint;
            Point nearestPoint = null;
            int minDist = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                Point p = GetNearestPoint(points, referencePoint, (Direction)i);
                if (p == null) continue;
                int currentDist = ManhattanDist(p, referencePoint);
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    nearestPoint = p;
                }
                else if(currentDist == minDist && nearestPoint!=null && !nearestPoint.Equals(p))
                {
                    nearestPoint = null;
                }
            }
            return nearestPoint;
        }

        public static Point GetNearestPoint(List<Point> points, Point referencePoint, Direction direction)
        {
            int dist = int.MaxValue;
            Point nearest = null;
            foreach(Point p in GetPointsInDirection(points, referencePoint, direction))
            {
                int thisDist = ManhattanDist(p, referencePoint);
                if(thisDist < dist)
                {
                    dist = thisDist;
                    nearest = p;
                }
            }
            return nearest;
        }

        public static List<Point> NonInfiniteAreaPoints(List<Point> points)
        {
            List<Point> result = new List<Point>();
            foreach(Point p in points)
            {
                bool infinite = false;
                for(int i=0; i<4; i++)
                {
                    var dirPoints = GetPointsInDirection(points, p, (Direction)i);
                    if (!dirPoints.Any())
                    {
                        infinite = true;
                        break;
                    }
                }
                if (!infinite) result.Add(p);
            }
            return result;
        }
        public static List<Point> DirectionPoints = new List<Point> { new Point(-1,0), new Point(1,0), new Point(0,-1), new Point(0,1), new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1) };
        public static List<Point> GetPointsInDirection(List<Point> otherPoints, Point referencePoint, Direction direction)
        {
            Point directionPoint = DirectionPoints[(int)direction];
            List<Point> result = new List<Point>();
            foreach(Point p in otherPoints)
            {
                // check to see if this point in in the direction passed in
                if (ManhattanDist(p, new Point(referencePoint.X + directionPoint.X, referencePoint.Y + directionPoint.Y)) < ManhattanDist(p, referencePoint))
                {
                    int yDist = ManhattanDist(referencePoint, new Point(referencePoint.X, p.Y));
                    int xDist = ManhattanDist(referencePoint, new Point(p.X, referencePoint.Y));
                    if (direction == Direction.Up || direction == Direction.Down)
                    {
                        if(yDist >= xDist) result.Add(p);
                    }
                    else
                    {
                        if(xDist >= yDist) result.Add(p);
                    }
                }
            }
            return result;
        }

        public static int ManhattanDist(Point p1, Point p2) => (Math.Max(p1.X, p2.X) - Math.Min(p1.X, p2.X)) + (Math.Max(p1.Y, p2.Y) - Math.Min(p1.Y, p2.Y));

        public static List<Point> Input;

        public static List<Point> TestInput = new List<Point>()
        {
            new Point(1, 1),
            new Point(1, 6),
            new Point(8, 3),
            new Point(3, 4),
            new Point(5, 5),
            new Point(8, 9),
        };

        public static List<Point> FullInput = new List<Point>()
        {
            new Point(183, 157),
new Point(331, 86),
new Point(347, 286),
new Point(291, 273),
new Point(285, 152),
new Point(63, 100),
new Point(47, 80),
new Point(70, 88),
new Point(333, 86),
new Point(72, 238),
new Point(158, 80),
new Point(256, 140),
new Point(93, 325),
new Point(343, 44),
new Point(89, 248),
new Point(93, 261),
new Point(292, 250),
new Point(240, 243),
new Point(342, 214),
new Point(192, 51),
new Point(71, 92),
new Point(219, 63),
new Point(240, 183),
new Point(293, 55),
new Point(316, 268),
new Point(264, 151),
new Point(68, 98),
new Point(190, 288),
new Point(85, 120),
new Point(261, 59),
new Point(84, 222),
new Point(268, 171),
new Point(205, 134),
new Point(80, 161),
new Point(337, 326),
new Point(125, 176),
new Point(228, 122),
new Point(278, 151),
new Point(129, 287),
new Point(293, 271),
new Point(57, 278),
new Point(104, 171),
new Point(330, 69),
new Point(141, 141),
new Point(112, 127),
new Point(201, 151),
new Point(331, 268),
new Point(95, 68),
new Point(289, 282),
new Point(221, 359),
        };
    }
}
