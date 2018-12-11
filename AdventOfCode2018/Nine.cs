using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Nine
    {
        public static void Part1()
        {
            MarbleCircle mc = new MarbleCircle(466, 7143600);
            mc.PlayGame();
            double winningScore = mc.Scores.Max(ss => ss);
            int winningPlayer = 0;
            for (int p = 1; p <= mc.Players; p++) if (mc.Scores[p] == winningScore) winningPlayer = p;
            if(winningPlayer == 0) 
                Console.WriteLine($"Something went wrong");
            else
                Console.WriteLine($"Player {winningPlayer} wins with a score of {winningScore}");
        }
    }

    public class MarbleCircle
    {
        public MarbleCircle(int players, int lastMarbleNum)
        {
            FinalMarbleNum = lastMarbleNum;
            Players = players;
            CurrentPlayer = 1;
            Scores = new double[Players + 1];
            CurrentNode = new LLNode(0);
            CurrentNode.ClockwiseNode = CurrentNode;
            CurrentNode.CounterClockNode = CurrentNode;
            NextMarbleNum = 1;
        }

        public int FinalMarbleNum { get; private set; }
        public int NextMarbleNum { get; private set; }
        public int Players { get; private set; }
        public int CurrentPlayer { get; private set; }
        public double[] Scores { get; private set; }

        public LLNode CurrentNode { get; set; }

        public void PlayGame()
        {
            while(NextMarbleNum <= FinalMarbleNum)
            {
                Add();
            }
        }

        public void Add()
        {
            if(NextMarbleNum % 23 == 0)
            {
                Scores[CurrentPlayer] += NextMarbleNum++;
                for (int i = 0; i < 7; i++) CurrentNode = CurrentNode.CounterClockNode;
                Scores[CurrentPlayer] += CurrentNode.Value;
                Remove(CurrentNode);
            }
            else
            {
                CurrentNode = AddNodeClockwise(CurrentNode, NextMarbleNum++);
            }
            //Console.WriteLine(ToString());
            if (++CurrentPlayer > Players) CurrentPlayer = 1;
        }

        private LLNode AddNodeClockwise(LLNode node, int marbleNum)
        {
            LLNode newNode = new LLNode(marbleNum);
            LLNode cw = node.ClockwiseNode;
            LLNode ccw = node.ClockwiseNode.ClockwiseNode;
            newNode.CounterClockNode = cw;
            cw.ClockwiseNode = newNode;
            newNode.ClockwiseNode = ccw;
            ccw.CounterClockNode = newNode;
            return newNode;
        }

        private void Remove(LLNode node)
        {
            LLNode counterClockNode = node.CounterClockNode;
            LLNode clockNode = node.ClockwiseNode;
            counterClockNode.ClockwiseNode = clockNode;
            clockNode.CounterClockNode = counterClockNode;
            node.ClockwiseNode = null;
            node.CounterClockNode = null;
            CurrentNode = clockNode;
        }

        public override string ToString()
        {
            LLNode printNode = CurrentNode;
            LLNode startNode = CurrentNode;
            StringBuilder sb = new StringBuilder($"[{CurrentPlayer}] ({printNode}) ");
            printNode = printNode.ClockwiseNode;
            while (printNode != startNode)
            {
                sb.Append($"{printNode} ");
                printNode = printNode.ClockwiseNode;
            }
            return sb.ToString();
        }
    }

    public class LLNode
    {
        public LLNode(int v) => Value = v;

        public LLNode ClockwiseNode { get; set; }
        public LLNode CounterClockNode { get; set; }
        public int Value { get; set; }

        public override string ToString() => $"{Value}";
    }

}
