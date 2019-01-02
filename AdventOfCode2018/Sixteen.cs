using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Sixteen
    {

        public static void Part1()
        {
            List<string> Input = SixteenInput.FullInput1;
            List<Sample> Samples = new List<Sample>();

            int countTriples = 0;
            int count = 0;
            for(int i = 0; i<Input.Count; i+=4)
            {
                if (!string.IsNullOrEmpty(Input[i + 3])) throw new Exception("Bad input list");
                Sample sample = new Sample(Input[i], Input[i + 1], Input[i + 2]);
                Samples.Add(sample);

                //Console.WriteLine($"Sample {i/4} -------------------------------------------------------");
                //Console.WriteLine(sample.InitialStrings[0]);
                //Console.WriteLine(sample.InitialStrings[1]);
                //Console.WriteLine(sample.InitialStrings[2]);
                //Console.WriteLine("TESTING");

                sample.TestSample();

                if (sample.OpCodeWorks.Count(p => p) >= 3) countTriples++;
                count++;
            }

            Console.WriteLine($"there are {countTriples}/{count} samples where three or more opcodes work");
        }

        public static void Part2Solve()
        {
            Regex regex2 = new Regex(@"([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+)");
            List<string> Input = SixteenInput.FullInputProgram;
            Machine machine = new Machine(0, 0, 0, 0);
            foreach(string args in Input)
            {
                int[] arguments = new int[3];
                int opCodeNum;
                MatchCollection m2 = regex2.Matches(args);
                opCodeNum = int.Parse(m2[0].Groups[1].Value);
                arguments[0] = int.Parse(m2[0].Groups[2].Value);
                arguments[1] = int.Parse(m2[0].Groups[3].Value);
                arguments[2] = int.Parse(m2[0].Groups[4].Value);
                machine.Execute(opCodeNum, arguments[0], arguments[1], arguments[2]);
            }
            Console.WriteLine($"Register 0 is {machine.Registers[0]} after the program is run");
        }

        public static void Part2FixOpCodes()
        {
            List<string> Input = SixteenInput.FullInput1;
            List<Sample> Samples = new List<Sample>();
            Dictionary<int, List<Sample>> sampleWorksDict = new Dictionary<int, List<Sample>>();

            int count = 0;
            for (int i = 0; i < Input.Count; i += 4)
            {
                if (!string.IsNullOrEmpty(Input[i + 3])) throw new Exception("Bad input list");
                Sample sample = new Sample(Input[i], Input[i + 1], Input[i + 2]);
                Samples.Add(sample);
                sample.TestSample();

                int numPotentialOpcodes = sample.OpCodeWorks.Count(p => p);
                if (!sampleWorksDict.ContainsKey(numPotentialOpcodes)) sampleWorksDict.Add(numPotentialOpcodes, new List<Sample>());
                sampleWorksDict[numPotentialOpcodes].Add(sample);

                count++;
            }

            bool[] allCorrect = new bool[16] {true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true};
            foreach(Sample s in Samples)
            {
                if (!s.OpCodeWorks[s.OpCodeNum]) allCorrect[s.OpCodeNum] = false;
            }
            int c = 0;
            foreach (bool b in allCorrect) Console.WriteLine($"{c++} => {b}");
            if (allCorrect.All(b => b)) Console.WriteLine("Done!");


            Console.WriteLine("");
            Console.WriteLine("numPotentialOpcodes => OpCode List Strings");
            foreach (var kvp in sampleWorksDict.OrderBy(kvp => kvp.Key))
            {
                string l = kvp.Value.Select(s => s.ValidOpCodesString()).Distinct().Aggregate("", (s, r) => s + $"{r}");
                Console.WriteLine($"{kvp.Key}: {l}");
            }

            Console.WriteLine("");
            Console.WriteLine("numPotentialOpcodes => OpCode List");
            foreach(var kvp in sampleWorksDict.OrderBy(kvp => kvp.Key))
            {
                string l = kvp.Value.SelectMany(s => s.ValidOpCodes()).Distinct().Aggregate("", (s, r) => s + $"{r}, ");
                Console.WriteLine($"{kvp.Key}: {l}");
            }

            List<Sample> samples = new List<Sample>();

            bool[] AlwaysWorks = new bool[16] {true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true};
            bool[] SometimesWorks = new bool[16];
            samples.Clear();
            for (int i = 1; i <= 5; i++) samples.AddRange(sampleWorksDict[i]);
            samples = Samples;
            foreach (Sample s in samples)
            {
                Console.WriteLine($"OpCode for sample: {s.OpCodeNum} -> {s.OpCodeWorks[s.OpCodeNum]}");
                if (!s.OpCodeWorks[s.OpCodeNum])
                {
                    AlwaysWorks[s.OpCodeNum] = false;
                    for (int i = 0; i < 16; i++)
                        if (s.OpCodeWorks[i]) Console.WriteLine($"{i} Works: {s.Machine.OpCodesNames[i]}");
                }
                else
                {
                    SometimesWorks[s.OpCodeNum] = true;
                }
            }
            int numCorrect = samples.Count(s => s.VerifySample());
            Console.WriteLine($"{numCorrect}/{samples.Count()}");
            Console.WriteLine("Always------------------------------------");
            for (int i = 0; i < 16; i++) Console.WriteLine($"{i} => {AlwaysWorks[i]}");
            Console.WriteLine("Sometimes------------------------------------");
            for (int i = 0; i < 16; i++) Console.WriteLine($"{i} => {SometimesWorks[i]}");




            //samples = sampleWorksDict[3];
            samples = Samples.Where(s => s.OpCodeNum == 11).ToList();
            foreach (Sample s in samples)
            {
                Console.WriteLine($"OpCode for sample: {s.OpCodeNum}");
                for (int i = 0; i < 16; i++)
                    if(s.OpCodeWorks[i]) Console.WriteLine($"{i} Works: {s.Machine.OpCodesNames[i]}");
            }
            numCorrect = samples.Count(s => s.VerifySample());
            Console.WriteLine($"{numCorrect}/{samples.Count()}");
        }
    }

    public class Sample
    {
        public int[] InitialRegisters = new int[4];
        public int OpCodeNum;
        public int[] Arguments = new int[3];
        public int[] ResultsRegisters = new int[4];

        public bool[] OpCodeWorks = new bool[16];
        public Machine Machine { get; set; }

        public string[] InitialStrings;

        public Sample(string initialRegs, string arguments, string results)
        {
            InitialStrings =new string[] { initialRegs, arguments, results };
            //"Before: [1, 2, 3, 2]",
            //"3 1 3 0",
            //"After:  [1, 2, 3, 2]",
            Regex regex1 = new Regex(@"Before: \[([0-9]+), ([0-9]+), ([0-9]+), ([0-9]+)\]");
            Regex regex2 = new Regex(@"([0-9]+) ([0-9]+) ([0-9]+) ([0-9]+)");
            Regex regex3 = new Regex(@"After:  \[([0-9]+), ([0-9]+), ([0-9]+), ([0-9]+)\]");

            MatchCollection m1 = regex1.Matches(initialRegs);
            MatchCollection m2 = regex2.Matches(arguments);
            MatchCollection m3 = regex3.Matches(results);

            InitialRegisters[0] = int.Parse(m1[0].Groups[1].Value);
            InitialRegisters[1] = int.Parse(m1[0].Groups[2].Value);
            InitialRegisters[2] = int.Parse(m1[0].Groups[3].Value);
            InitialRegisters[3] = int.Parse(m1[0].Groups[4].Value);

            OpCodeNum = int.Parse(m2[0].Groups[1].Value);
            Arguments[0] = int.Parse(m2[0].Groups[2].Value);
            Arguments[1] = int.Parse(m2[0].Groups[3].Value);
            Arguments[2] = int.Parse(m2[0].Groups[4].Value);

            ResultsRegisters[0] = int.Parse(m3[0].Groups[1].Value);
            ResultsRegisters[1] = int.Parse(m3[0].Groups[2].Value);
            ResultsRegisters[2] = int.Parse(m3[0].Groups[3].Value);
            ResultsRegisters[3] = int.Parse(m3[0].Groups[4].Value);

            Machine = new Machine(InitialRegisters[0], InitialRegisters[1], InitialRegisters[2], InitialRegisters[3]);
        }

        public Sample(int a, int b, int c, int d, int ar, int br, int cr, int dr, int opcode, int argA, int argB, int argC)
        {
            InitialRegisters[0] = a;
            InitialRegisters[1] = b;
            InitialRegisters[2] = c;
            InitialRegisters[3] = d;

            OpCodeNum = opcode;
            Arguments[0] = argA;
            Arguments[1] = argB;
            Arguments[2] = argC;

            ResultsRegisters[0] = ar;
            ResultsRegisters[1] = br;
            ResultsRegisters[2] = cr;
            ResultsRegisters[3] = dr;

            Machine = new Machine(a, b, c, d);
        }

        public void TestSample()
        {
            if (!Verify()) throw new Exception("verification failed");

            for(int opNum = 0; opNum < 16; opNum++)
            {
                Reset();

                Func<int, int, int, int> f = Machine.OpCodes[opNum];
                int result = f(Arguments[0], Arguments[1], Arguments[2]);
                OpCodeWorks[opNum] = ResultsRegisters.Zip(Machine.Registers, (expected, actual) => expected == actual).All(t => t);

                string afterString = $"{Machine.OpCodesNames[opNum]}: [{Machine.Registers[0]}, {Machine.Registers[1]}, {Machine.Registers[2]}, {Machine.Registers[3]}]";

                //Console.WriteLine(afterString);
            }
        }

        public bool VerifySample()
        {
            Reset();
            Func<int, int, int, int> f = Machine.OpCodes[OpCodeNum];
            int result = f(Arguments[0], Arguments[1], Arguments[2]);
            return ResultsRegisters.Zip(Machine.Registers, (expected, actual) => expected == actual).All(t => t);
        }

        public void Reset()
        {
            Machine.Registers[0] = InitialRegisters[0];
            Machine.Registers[1] = InitialRegisters[1];
            Machine.Registers[2] = InitialRegisters[2];
            Machine.Registers[3] = InitialRegisters[3];
        }

        public bool Verify()
        {
            string beforeString = $"Before: [{InitialRegisters[0]}, {InitialRegisters[1]}, {InitialRegisters[2]}, {InitialRegisters[3]}]";
            string argsString = $"{OpCodeNum} {Arguments[0]} {Arguments[1]} {Arguments[2]}";
            string afterString = $"After:  [{ResultsRegisters[0]}, {ResultsRegisters[1]}, {ResultsRegisters[2]}, {ResultsRegisters[3]}]";

            bool retval = InitialStrings[0] == beforeString && InitialStrings[1] == argsString && InitialStrings[2] == afterString;
            //if (!retval)
            //{
            //    Console.WriteLine(InitialStrings[0]);
            //    Console.WriteLine(beforeString);
            //    Console.WriteLine(InitialStrings[1]);
            //    Console.WriteLine(argsString);
            //    Console.WriteLine(InitialStrings[2]);
            //    Console.WriteLine(afterString);
            //}
            return retval;
        }


        public List<int> ValidOpCodes()
        {
            List<int> r = new List<int>();
            for (int i = 0; i < 16; i++) if (OpCodeWorks[i]) r.Add(i);
            return r;
        }

        public string ValidOpCodesString()
        {
            string st = $"{OpCodeNum}[" + ValidOpCodes().OrderBy(i => i).Aggregate("", (s, r) => s + $"{r},") + "]";
            return st;
        }
    }

    public class Machine
    {
        public Machine(int a, int b, int c, int d)
        {
            Registers[0] = a;
            Registers[1] = b;
            Registers[2] = c;
            Registers[3] = d;
            SetupOpCodes();
        }

        public int[] Registers = new int[4];

        public Func<int, int, int, int>[] OpCodes = new Func<int, int, int, int>[16];
        public string[] OpCodesNames = new string[16];
        public void SetupOpCodes()
        {
            OpCodes[0] = addr;
            OpCodesNames[0] = "addr";
            OpCodes[1] = addi;
            OpCodesNames[1] = "addi";
            OpCodes[2] = mulr;
            OpCodesNames[2] = "mulr";
            OpCodes[3] = muli;
            OpCodesNames[3] = "muli";
            OpCodes[4] = banr;
            OpCodesNames[4] = "banr";
            OpCodes[5] = bani;
            OpCodesNames[5] = "bani";
            OpCodes[6] = borr;
            OpCodesNames[6] = "borr";
            OpCodes[7] = bori;
            OpCodesNames[7] = "bori";
            OpCodes[8] = setr;
            OpCodesNames[8] = "setr";
            OpCodes[9] = seti;
            OpCodesNames[9] = "seti";
            OpCodes[10] = gtir;
            OpCodesNames[10] = "gtir";
            OpCodes[11] = gtri;
            OpCodesNames[11] = "gtri";
            OpCodes[12] = gtrr;
            OpCodesNames[12] = "gtrr";
            OpCodes[13] = eqir;
            OpCodesNames[13] = "eqir";
            OpCodes[14] = eqri;
            OpCodesNames[14] = "eqri";
            OpCodes[15] = eqrr;
            OpCodesNames[15] = "eqrr";

            
            SwapOpCodes(0, 2);
            SwapOpCodes(6, 6);
            SwapOpCodes(15, 7);
            SwapOpCodes(8, 2);
            SwapOpCodes(5, 3);
            SwapOpCodes(12, 1);
            SwapOpCodes(14, 9);
            SwapOpCodes(3, 7);
            SwapOpCodes(10, 13);
            SwapOpCodes(13, 11);
            SwapOpCodes(1, 9);
            SwapOpCodes(4, 9);
        }

        public void SwapOpCodes(int op1Index, int op2Index)
        {
            Func<int, int, int, int> temp = OpCodes[op1Index];
            string tempName = OpCodesNames[op1Index];
            OpCodes[op1Index] = OpCodes[op2Index];
            OpCodesNames[op1Index] = OpCodesNames[op2Index];
            OpCodes[op2Index] = temp;
            OpCodesNames[op2Index] = tempName;
        }

        //(equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
        private int eqrr(int a, int b, int c)
        {
            if (Registers[a] == Registers[b])
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
        private int eqri(int a, int b, int c)
        {
            if (Registers[a] == b)
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
        private int eqir(int a, int b, int c)
        {
            if (a == Registers[b])
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
        private int gtrr(int a, int b, int c)
        {
            if (Registers[a] > Registers[b])
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
        private int gtri(int a, int b, int c)
        {
            if (Registers[a] > b)
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
        private int gtir(int a, int b, int c)
        {
            if (a > Registers[b])
                Registers[c] = 1;
            else
                Registers[c] = 0;
            return Registers[c];
        }

        //(set immediate) stores value A into register C. (Input B is ignored.)
        private int seti(int a, int b, int c)
        {
            Registers[c] = a;
            return a;
        }

        //(set register) copies the contents of register A into register C. (Input B is ignored.)
        private int setr(int a, int b, int c)
        {
            Registers[c] = Registers[a];
            return Registers[c];
        }

        //(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
        private int bori(int a, int b, int c)
        {
            Registers[c] = Registers[a] | b;
            return Registers[c];
        }

        //(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
        private int borr(int a, int b, int c)
        {
            Registers[c] = Registers[a] | Registers[b];
            return Registers[c];
        }

        //(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
        private int bani(int a, int b, int c)
        {
            Registers[c] = Registers[a] & b;
            return Registers[c];
        }

        //(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
        private int banr(int a, int b, int c)
        {
            Registers[c] = Registers[a] & Registers[b];
            return Registers[c];
        }

        //(multiply immediate) stores into register C the result of multiplying register A and value B.
        private int muli(int a, int b, int c)
        {
            Registers[c] = Registers[a] * b;
            return Registers[c];
        }

        //(multiply register) stores into register C the result of multiplying register A and register B.
        private int mulr(int a, int b, int c)
        {
            Registers[c] = Registers[a] * Registers[b];
            return Registers[c];
        }

        //(add immediate) stores into register C the result of adding register A and value B.
        private int addi(int a, int b, int c)
        {
            Registers[c] = Registers[a] + b;
            return Registers[c];
        }

        //(add register) stores into register C the result of adding register A and register B.
        private int addr(int a, int b, int c)
        {
            Registers[c] = Registers[a] + Registers[b];
            return Registers[c];
        }

        internal void Execute(int opCodeNum, int a, int b, int c)
        {
            OpCodes[opCodeNum](a, b, c);
        }
    }
}
