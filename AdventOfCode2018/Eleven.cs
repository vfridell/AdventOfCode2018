using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Eleven
    {
        static int _gridSerialNum = 57;

        public static void Part1()
        {
            Dictionary<Point, int> powerLevels = new Dictionary<Point, int>();
            for (int c = 1; c <= 301; c++)
            {
                for (int r = 1; r <= 301; r++)
                {
                    var p = new Point(c, r);
                    powerLevels.Add(p, PowerLevel(p));
                }
            }
            var orderedDict = powerLevels.OrderByDescending(kvp => kvp.Value);

            int fours = orderedDict.Count(kvp => kvp.Value == 4);
        }

        public static int PowerLevel(Point p)
        {
            int rackId = p.X + 10;
            int powerLevel = p.Y * rackId;
            powerLevel += _gridSerialNum;
            powerLevel *= rackId;
            string hundredsDigit = powerLevel.ToString("000000").Reverse().ToList()[2].ToString();
            powerLevel = int.Parse(hundredsDigit);
            powerLevel -= 5;
            return powerLevel;
        }
    }
}
