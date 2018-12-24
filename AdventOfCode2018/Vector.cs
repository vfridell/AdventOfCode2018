using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
