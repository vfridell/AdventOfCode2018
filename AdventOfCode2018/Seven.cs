using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Tree
    {
        public Tree(Node root)
        {
            Root = root;
        }

        public Node Root { get; private set; }

        public int NodeCount
        {
            get
            {
                return NodeCountRecursive(Root, new List<Node>());
            }
        }


        int NodeCountRecursive(Node currentNode, List<Node> counted)
        {
            int count = currentNode.Children.Count(n => !counted.Contains(n));
            counted.AddRange(currentNode.Children);
            foreach (Node n in currentNode.Children)
            {
                count += NodeCountRecursive(n, counted);
            }
            return count;
        }

        internal string GetStepOrder()
        {
            List<Node> done = new List<Node>() { Root };
            List<Node> available = GetAvailableStepsRecursive(Root, done);
            while(available.Count > 0)
            {
                done.Add(available.OrderBy(n => n.Name).First());
                available = GetAvailableStepsRecursive(Root, done);
            }
            return done.Select(n => n.Name).Aggregate("", (s, c) => s + $"{c}");
        }

        internal int GetStepOrderWithTime(int workers)
        {
            int nodeCount = NodeCount;
            List<Node> done = new List<Node>() { Root };
            List<Node> working = new List<Node>() { };
            List<Node> available = GetAvailableStepsRecursive(Root, done);
            int time = 0;
            int availableWorkers = workers;
            do
            {
                done.AddRange(working.Where(n => n.CompleteTime <= time).OrderBy(n => n.Name));
                int removed = working.RemoveAll(n => n.CompleteTime <= time);
                availableWorkers += removed;
                available = GetAvailableStepsRecursive(Root, done).Where(n => !working.Contains(n)).ToList();
                foreach (Node n in available.OrderBy(n => n.Name))
                {
                    if (availableWorkers > 0)
                    {
                        availableWorkers--;
                        n.StartTime = time;
                        working.Add(n);
                    }
                }
                available.RemoveAll(n => working.Contains(n));
                Console.WriteLine($"{time}\t{available.Aggregate("", (s, n) => s + $"{n.Name},")} Available\tWorking on {working.Aggregate("", (s, n) => s + $"{n.Name},")}\tFinished {done.Aggregate("", (s, n) => s + $"{n.Name},")}");
                if(working.Any()) time = working.Min(n => n.CompleteTime);
            }
            while (working.Count > 0);
            Console.WriteLine(done.Select(n => n.Name).Aggregate("", (s, c) => s + $"{c}"));
            return time;
        }

        List<Node> GetAvailableStepsRecursive(Node currentNode, List<Node> done)
        {
            List<Node> result = new List<Node>();
            if (done.Contains(currentNode))
            {
                foreach (Node n in currentNode.Children)
                {
                    if(n.Parents.All(p => done.Contains(p))) result.AddRange(GetAvailableStepsRecursive(n, done));
                }
            }
            else
            {
                result.Add(currentNode);
            }
            return result.Distinct().ToList();
        }
    }

    public class Node
    {
        public Node(char name)
        {
            Name = name;
        }

        public char Name { get; private set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public List<Node> Parents { get; set; } = new List<Node>();
        public int TimeToComplete => (Name - 64) + 60;
        public int StartTime { get; set; }
        public int CompleteTime => StartTime + TimeToComplete;

        public override bool Equals(object obj)
        {
            Node other = obj as Node;
            if (other == null) return false;
            return Equals(other);
        }

        public bool Equals(Node other) => Name == other.Name;
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => $"{Name}";
    }

    public class Seven
    {
        public static Regex regex = new Regex("Step ([A-Z]) must be finished before step ([A-Z]) can begin");

        public static void Part1()
        {
            var Input = FullInput;
            Tree tree = GetTree(Input);
            string stepOrder = tree.GetStepOrder();
            Console.WriteLine($"Step order is {stepOrder}");
        }

        public static void Part2()
        {
            var Input = FullInput;
            Tree tree = GetTree(Input);
            int time = tree.GetStepOrderWithTime(5);
            Console.WriteLine($"It took {time} seconds to build");
        }

        private static Tree GetTree(List<string> Input)
        {
            Dictionary<char, List<char>> NodeDependencies = new Dictionary<char, List<char>>();
            HashSet<Node> AllNodes = new HashSet<Node>();
            foreach (string s in Input)
            {
                MatchCollection matchCollection = regex.Matches(s);
                char parentNodeName = matchCollection[0].Groups[1].Value[0];
                char childNodeName = matchCollection[0].Groups[2].Value[0];
                if (!NodeDependencies.ContainsKey(parentNodeName)) NodeDependencies[parentNodeName] = new List<char>() { childNodeName };
                else NodeDependencies[parentNodeName].Add(childNodeName);

                AllNodes.Add(new Node(childNodeName));
                AllNodes.Add(new Node(parentNodeName));
            }
            foreach (Node n in AllNodes)
            {
                n.Children = AllNodes.Where(c => NodeDependencies.ContainsKey(n.Name) && NodeDependencies[n.Name].Contains(c.Name)).ToList();
            }
            foreach (Node n in AllNodes)
            {
                n.Parents = AllNodes.Where(p => p.Children.Contains(n)).ToList();
            }
            List<Node> topLevelNodes = AllNodes.Where(r => !AllNodes.SelectMany(n => n.Children).Contains(r)).ToList();
            Node root = new Node('*');
            root.Children = topLevelNodes;
            root.Parents = new List<Node>();
            Tree tree = new Tree(root);
            return tree;
        }

        public static List<string> TestInput = new List<string>()
        {
            "Step C must be finished before step A can begin.",
"Step C must be finished before step F can begin.",
"Step A must be finished before step B can begin.",
"Step A must be finished before step D can begin.",
"Step B must be finished before step E can begin.",
"Step D must be finished before step E can begin.",
"Step F must be finished before step E can begin.",

        };

        public static List<string> FullInput = new List<string>()
        {
"Step G must be finished before step X can begin.",
"Step X must be finished before step B can begin.",
"Step A must be finished before step I can begin.",
"Step D must be finished before step H can begin.",
"Step O must be finished before step T can begin.",
"Step H must be finished before step C can begin.",
"Step S must be finished before step E can begin.",
"Step U must be finished before step M can begin.",
"Step M must be finished before step Z can begin.",
"Step R must be finished before step N can begin.",
"Step C must be finished before step Q can begin.",
"Step T must be finished before step P can begin.",
"Step I must be finished before step W can begin.",
"Step W must be finished before step N can begin.",
"Step P must be finished before step J can begin.",
"Step N must be finished before step F can begin.",
"Step Y must be finished before step J can begin.",
"Step J must be finished before step L can begin.",
"Step L must be finished before step E can begin.",
"Step E must be finished before step B can begin.",
"Step Q must be finished before step B can begin.",
"Step F must be finished before step K can begin.",
"Step V must be finished before step K can begin.",
"Step Z must be finished before step B can begin.",
"Step B must be finished before step K can begin.",
"Step G must be finished before step U can begin.",
"Step E must be finished before step V can begin.",
"Step A must be finished before step Z can begin.",
"Step C must be finished before step V can begin.",
"Step R must be finished before step B can begin.",
"Step Q must be finished before step Z can begin.",
"Step R must be finished before step K can begin.",
"Step T must be finished before step B can begin.",
"Step L must be finished before step B can begin.",
"Step M must be finished before step K can begin.",
"Step T must be finished before step Z can begin.",
"Step W must be finished before step B can begin.",
"Step I must be finished before step E can begin.",
"Step A must be finished before step M can begin.",
"Step V must be finished before step Z can begin.",
"Step Y must be finished before step B can begin.",
"Step Q must be finished before step F can begin.",
"Step W must be finished before step Y can begin.",
"Step U must be finished before step K can begin.",
"Step D must be finished before step F can begin.",
"Step P must be finished before step F can begin.",
"Step N must be finished before step L can begin.",
"Step H must be finished before step T can begin.",
"Step H must be finished before step L can begin.",
"Step C must be finished before step T can begin.",
"Step H must be finished before step I can begin.",
"Step Z must be finished before step K can begin.",
"Step L must be finished before step Z can begin.",
"Step Y must be finished before step K can begin.",
"Step I must be finished before step V can begin.",
"Step P must be finished before step K can begin.",
"Step P must be finished before step N can begin.",
"Step G must be finished before step D can begin.",
"Step I must be finished before step J can begin.",
"Step H must be finished before step K can begin.",
"Step L must be finished before step Q can begin.",
"Step D must be finished before step M can begin.",
"Step O must be finished before step V can begin.",
"Step R must be finished before step L can begin.",
"Step D must be finished before step W can begin.",
"Step M must be finished before step J can begin.",
"Step O must be finished before step R can begin.",
"Step N must be finished before step Z can begin.",
"Step Y must be finished before step V can begin.",
"Step W must be finished before step L can begin.",
"Step U must be finished before step Y can begin.",
"Step S must be finished before step V can begin.",
"Step M must be finished before step P can begin.",
"Step X must be finished before step A can begin.",
"Step A must be finished before step E can begin.",
"Step A must be finished before step L can begin.",
"Step A must be finished before step R can begin.",
"Step V must be finished before step B can begin.",
"Step P must be finished before step B can begin.",
"Step E must be finished before step F can begin.",
"Step T must be finished before step V can begin.",
"Step S must be finished before step R can begin.",
"Step T must be finished before step F can begin.",
"Step P must be finished before step Y can begin.",
"Step A must be finished before step C can begin.",
"Step J must be finished before step F can begin.",
"Step H must be finished before step B can begin.",
"Step C must be finished before step E can begin.",
"Step P must be finished before step E can begin.",
"Step D must be finished before step I can begin.",
"Step X must be finished before step F can begin.",
"Step T must be finished before step Q can begin.",
"Step J must be finished before step B can begin.",
"Step C must be finished before step B can begin.",
"Step P must be finished before step Q can begin.",
"Step H must be finished before step R can begin.",
"Step F must be finished before step B can begin.",
"Step T must be finished before step J can begin.",
"Step A must be finished before step W can begin.",
"Step N must be finished before step K can begin.",
"Step T must be finished before step E can begin.",

        };

    }
}
