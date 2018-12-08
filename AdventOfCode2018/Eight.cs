using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Eight
    {
        public static void Part1()
        {
            List<int> Input = EightInput.FullInput;
            EightTree tree = BuildTree(Input);
            Console.WriteLine($"Sum of all metadata: {tree.Sum}");
        }

        internal static void Part2()
        {
            List<int> Input = EightInput.FullInput;
            EightTree tree = BuildTree(Input);
            Console.WriteLine($"Value of root node: {tree.RootValue}");
        }

        private static EightTree BuildTree(List<int> Input)
        {
            //int nodeLength = 1 + metadataLength + subnodeLengths;
            int frontIndex = 0;
            int sum = 0;

            EightNode root = new EightNode(Input[frontIndex++], Input[frontIndex++]);
            EightNode currentNode = root;
            do
            {
                if (currentNode.ChildCount > 0 && currentNode.Children.Count != currentNode.ChildCount)
                {
                    // "recurse"
                    EightNode child = new EightNode(Input[frontIndex++], Input[frontIndex++]);
                    child.Parent = currentNode;
                    currentNode.Children.Add(child);
                    currentNode = child;
                }
                else if (currentNode.MetadataCount > 0 && currentNode.Metadata.Count != currentNode.MetadataCount)
                {
                    // read metadata
                    for (int i = 0; i < currentNode.MetadataCount; i++) currentNode.Metadata.Add(Input[frontIndex++]);
                    sum += currentNode.Metadata.Sum();
                    currentNode = currentNode.Parent;
                }
                else
                {
                    currentNode = currentNode.Parent;
                }

            }
            while (frontIndex < Input.Count);
            EightTree tree = new EightTree(root);
            tree.Sum = sum;
            return tree;
        }

        public class EightTree
        {
            public EightTree(EightNode root)
            {
                Root = root;
            }

            public EightNode Root { get; private set; }
            public int Sum { get; set; }
            public int RootValue => Root.NodeValue;
        }

        public class EightNode
        {
            public EightNode(int childCount, int metadataCount)
            {
                ChildCount = childCount;
                MetadataCount = metadataCount;
            }
            public int ChildCount { get; set; }
            public int MetadataCount { get; set; }

            public List<int> Metadata { get; set; } = new List<int>();

            public EightNode Parent { get; set; }
            public List<EightNode> Children { get; set; } = new List<EightNode>();

            private int _nodeValue = -1;
            public int NodeValue
            { 
                get
                {
                    if (_nodeValue == -1)
                    {
                        if (ChildCount == 0)
                        {
                            _nodeValue = Metadata.Sum();
                        }
                        else
                        {
                            _nodeValue = 0;
                            foreach (int index in Metadata)
                            {
                                if (ChildCount >= index) _nodeValue += Children[index - 1].NodeValue;
                            }
                        }
                    }
                    return _nodeValue;
                }
            }
        }

    }

}
