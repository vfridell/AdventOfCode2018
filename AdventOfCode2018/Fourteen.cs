using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Fourteen
    {
        public static StringBuilder Input = new StringBuilder("37");

        public static void Part1()
        {
            

            int elf1 = 0;
            int elf2 = 1;

            //Draw(elf1, elf2);

            int generations = 306281;
            int totalLength = generations + 11;
            while(Input.Length < totalLength)
            {
                int elf1_Score = int.Parse(Input[elf1].ToString());
                int elf2_Score = int.Parse(Input[elf2].ToString());

                string addition = (elf1_Score + elf2_Score).ToString();
                Input = Input.Append(addition);

                elf1 = elf1_Score + 1 + elf1;
                if (elf1 >= Input.Length) elf1 = elf1 % Input.Length;

                elf2 = elf2_Score + 1 + elf2;
                if (elf2 >= Input.Length) elf2 = elf2 % Input.Length;

                if (elf2 == Input.Length) elf2 = 0;
                if (elf1 == Input.Length) elf1 = 0;

                //Draw(elf1, elf2);
            }

            Console.WriteLine("The answer is:");
            foreach (char c in Input.ToString().Skip(generations).Take(10)) Console.Write(c);
        }

        public static void Part2()
        {
            int elf1 = 0;
            int elf2 = 1;

            //Draw(elf1, elf2);

            //int generations = 306281;
            string searchString = "306281";
            bool found = false;
            while (!found)
            {
                int elf1_Score = int.Parse(Input[elf1].ToString());
                int elf2_Score = int.Parse(Input[elf2].ToString());

                string addition = (elf1_Score + elf2_Score).ToString();
                Input = Input.Append(addition);

                elf1 = elf1_Score + 1 + elf1;
                if (elf1 >= Input.Length) elf1 = elf1 % Input.Length;

                elf2 = elf2_Score + 1 + elf2;
                if (elf2 >= Input.Length) elf2 = elf2 % Input.Length;

                if (elf2 == Input.Length) elf2 = 0;
                if (elf1 == Input.Length) elf1 = 0;

                //Draw(elf1, elf2);
                if (Input.Length > searchString.Length)
                {
                    for (int i = 0; i < addition.Length; i++)
                    {
                        found = Input.ToString(Input.Length - ((searchString.Length) + i), searchString.Length) == searchString;
                        if (found)
                        {
                            int index = Input.Length - (searchString.Length + i);
                            Console.WriteLine($"The answer is: {index}");
                            break;
                        }
                    }
                }
            }

        }

        private static void Draw(int elf1, int elf2)
        {
            for (int i = 0; i < Input.Length; i++)
            {
                if (i == elf1) Console.Write($"({Input[i]})");
                else if (i == elf2) Console.Write($"[{Input[i]}]");
                else Console.Write(Input[i]);
            }
            Console.WriteLine();
        }
    }

}
