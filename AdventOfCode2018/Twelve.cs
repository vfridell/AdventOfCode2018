using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Twelve
    {
        public static void Part1()
        {
            Rule = FullRule;
            InitialState = FullInitialState;

            Dictionary<int, bool> CurrentGen = new Dictionary<int, bool>();
            Dictionary<int, bool> NextGen = new Dictionary<int, bool>();

            for(int i = 0; i<InitialState.Length; i++)
            {
                if (InitialState[i] == '#') CurrentGen[i] = true;
                else CurrentGen[i] = false;
            }

            int generationsToRun = 20;
            List<int> answers = new List<int>();
            answers.Add(CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key));
            while(generationsToRun > 0)
            {
                int minPosition = CurrentGen.Keys.Min() - 2;
                int maxPosition = CurrentGen.Keys.Max() + 2;

                for(int i = minPosition; i<=maxPosition; i++)
                {
                    int state = GetStateAtPosition(CurrentGen, i);
                    NextGen[i] = Rule.Contains(state);
                }

                CurrentGen = new Dictionary<int, bool>(NextGen);
                generationsToRun--;
                
                //answers.Add(CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key));
                //string emptySpots = "........................................".Substring(0, 40 - Math.Abs(CurrentGen.Keys.Min()));
                //Console.Write(emptySpots);
                //foreach (var kvp in CurrentGen) Console.Write(kvp.Value ? "#" : ".");
                //Console.WriteLine();
            }

            int answer = CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key);

            Console.WriteLine($"The answer is {answer}");
        }

        public static void Part2()
        {
            Rule = FullRule;
            InitialState = FullInitialState;

            Dictionary<int, bool> CurrentGen = new Dictionary<int, bool>();
            Dictionary<int, bool> NextGen = new Dictionary<int, bool>();

            for (int i = 0; i < InitialState.Length; i++)
            {
                if (InitialState[i] == '#') CurrentGen[i] = true;
                else CurrentGen[i] = false;
            }

            int generationsToRun = 1000;
            int generation = 1;
            int diff = 0;
            int lastDiff = -1;
            List<int> answers = new List<int>();
            answers.Add(CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key));
            while (generation <= generationsToRun)
            {
                int minPosition = CurrentGen.Keys.Min() - 2;
                int maxPosition = CurrentGen.Keys.Max() + 2;

                for (int i = minPosition; i <= maxPosition; i++)
                {
                    int state = GetStateAtPosition(CurrentGen, i);
                    NextGen[i] = Rule.Contains(state);
                }

                CurrentGen = new Dictionary<int, bool>(NextGen);

                answers.Add(CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key));

                diff = answers[generation] - answers[generation - 1];
                if (diff == lastDiff)
                    break;

                lastDiff = diff;
                generation++;
            }

            double answer = CurrentGen.Where(kvp => kvp.Value).Sum(kvp => kvp.Key);
            answer = answer + (75 * (50000000000d - generation));

            Console.WriteLine($"The answer is {answer}");
        }

        private static int GetStateAtPosition(Dictionary<int,bool> currentGen, int position)
        {
            int place = 0;
            double result = 0;
            for (int i = position + 2; i >= position - 2; i--)
            {
                if (currentGen.ContainsKey(i) && currentGen[i]) result += Math.Pow(2, place);
                place++;
            }
            return (int)result;
        }

        private static List<bool> GetRuleList(HashSet<int> rule)
        {
            List<bool> result = new List<bool>(32);
            for (int i = 0; i < 32; i++) result[i] = Rule.Contains(i);
            return result;
        }

        public static HashSet<int> Rule;

        public static string InitialState;
        public static string TestInitialState = "#..#.#..##......###...###";
        public static string FullInitialState = "#......##...#.#.###.#.##..##.#.....##....#.#.##.##.#..#.##........####.###.###.##..#....#...###.##";

        public static HashSet<int> TestRule = new HashSet<int>()
        {
            0b00011,
            0b00100,
            0b01000,
            0b01010,
            0b01011,
            0b01100,
            0b01111,
            0b10101,
            0b10111,
            0b11010,
            0b11011,
            0b11100,
            0b11101,
            0b11110,
        };

        public static HashSet<int> FullRule = new HashSet<int>()
        {
            0b11011,
            0b00110,
            0b11000,
            0b00100,
            0b01110,
            0b01010,
            0b10011,
            0b01101,
            0b10111,
            0b01100,
            0b10101,
            0b01000,
            0b01001,
            0b00101,
            0b00010,
            0b11110,
            0b11100,
        };
                }

            }
