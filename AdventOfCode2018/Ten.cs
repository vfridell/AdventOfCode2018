using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Vector : IRectangle
    {

        public Vector(Point position, Point velocity)
        {
            Position = position;
            Velocity = velocity;
            _rectangle = new Rect(Position.X - 1, Position.Y - 1, 3, 3);
        }

        public Point Position { get; set; }
        public Point Velocity { get; set; }

        private Rect _rectangle;
        public Rect Rectangle => _rectangle;

        public Vector GetVectorAt(int time)
        {
            Point newPosition = new Point(Position.X + (Velocity.X * time), Position.Y + (Velocity.Y * time));
            return new Vector(newPosition, Velocity);
        }
    }

    public class Ten
    {

        public static void Part1()
        {
            Input = TestInput;

            List<Vector> vectors = new List<Vector>();
            foreach(string vecString in Input)
            {
                Point position = new Point(int.Parse(regex.Matches(vecString)[0].Groups[1].Value), int.Parse(regex.Matches(vecString)[0].Groups[2].Value));
                Point velocity = new Point(int.Parse(regex.Matches(vecString)[0].Groups[3].Value), int.Parse(regex.Matches(vecString)[0].Groups[4].Value));
                vectors.Add(new Vector(position, velocity));
            }

            int checkTime1 = 10000;
            int checkTime2 = 5000;

            List<Vector> newVectors1 = GetVectorsAtTime(vectors, checkTime1);
            List<Vector> newVectors2 = GetVectorsAtTime(vectors, checkTime2);
            int boundingBoxSize1 = GetBoundingBoxMaxSide(newVectors1);
            int boundingBoxSize2 = GetBoundingBoxMaxSide(newVectors2);

            if(boundingBoxSize1 < boundingBoxSize2)
            {

            }
            


            //int minX = vectors.Min(v => v.Position.X);
            //int minY = vectors.Min(v => v.Position.Y);
            //int maxX = vectors.Max(v => v.Position.X);
            //int maxY = vectors.Max(v => v.Position.Y);
            //int rootNodeSize = Math.Max(Math.Abs(minX) + Math.Abs(maxX), Math.Abs(minY) + Math.Abs(maxY));
            //int minSquare = ((rootNodeSize / 32d) < 10d) ? 10 : rootNodeSize / 32;
            //QuadTree<Vector> quadTree = QuadTree<Vector>.GetNew(new Point(minX, minY), rootNodeSize, minSquare);

        }

        public static List<Vector> GetVectorsAtTime(List<Vector> vectors, int time)
        {
            List<Vector> adjustedVectors = new List<Vector>(vectors.Count);
            for (int i = 0; i < baseVectors.Count; i++) adjustedVectors[i] = vectors[i].GetVectorAt(time);
            return adjustedVectors;
        }

        public static int GetBoundingBoxMaxSide(List<Vector> vectors)
        {
            int minX = vectors.Min(v => v.Position.X);
            int minY = vectors.Min(v => v.Position.Y);
            int maxX = vectors.Max(v => v.Position.X);
            int maxY = vectors.Max(v => v.Position.Y);
            return Math.Max(Math.Abs(minX) + Math.Abs(maxX), Math.Abs(minY) + Math.Abs(maxY));
        }

        public static Regex regex = new Regex("position=<([0-9- ]+), ([0-9- ]+)> velocity=<([0-9- ]+), ([0-9- ]+)>");

        public static List<string> Input;
        public static List<string> TestInput = new List<string>
        {
            "position=< 9,  1> velocity=< 0,  2>",
"position=< 7,  0> velocity=<-1,  0>",
"position=< 3, -2> velocity=<-1,  1>",
"position=< 6, 10> velocity=<-2, -1>",
"position=< 2, -4> velocity=< 2,  2>",
"position=<-6, 10> velocity=< 2, -2>",
"position=< 1,  8> velocity=< 1, -1>",
"position=< 1,  7> velocity=< 1,  0>",
"position=<-3, 11> velocity=< 1, -2>",
"position=< 7,  6> velocity=<-1, -1>",
"position=<-2,  3> velocity=< 1,  0>",
"position=<-4,  3> velocity=< 2,  0>",
"position=<10, -3> velocity=<-1,  1>",
"position=< 5, 11> velocity=< 1, -2>",
"position=< 4,  7> velocity=< 0, -1>",
"position=< 8, -2> velocity=< 0,  1>",
"position=<15,  0> velocity=<-2,  0>",
"position=< 1,  6> velocity=< 1,  0>",
"position=< 8,  9> velocity=< 0, -1>",
"position=< 3,  3> velocity=<-1,  1>",
"position=< 0,  5> velocity=< 0, -1>",
"position=<-2,  2> velocity=< 2,  0>",
"position=< 5, -2> velocity=< 1,  2>",
"position=< 1,  4> velocity=< 2,  1>",
"position=<-2,  7> velocity=< 2, -2>",
"position=< 3,  6> velocity=<-1, -1>",
"position=< 5,  0> velocity=< 1,  0>",
"position=<-6,  0> velocity=< 2,  0>",
"position=< 5,  9> velocity=< 1, -2>",
"position=<14,  7> velocity=<-2,  0>",
"position=<-3,  6> velocity=< 2, -1>",
        };
    }
}
