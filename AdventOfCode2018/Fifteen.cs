using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018
{
    public class Fifteen
    {
        public static void Part1()
        {
            Input = TestInput;
            Input = TestInput2;
            Input = TestInput3;
            Input = TestInput4;
            Input = TestInput5;
            //Input = TestInput99;
        
            Input = FullInput;
            //Input = FullInputAlt;
            Map map = Map.GetMap(Input);
            Console.Write(map);

            int round = 0;
            while (map.AnyFightLeft)
            {
                round++;
                int creaturesDone = 0;
                bool roundComplete = true;
                IOrderedEnumerable<KeyValuePair<Point, Creature>> creatures = map.Creatures.OrderBy(kvp => kvp.Key);
                foreach (var kvp in creatures)
                {
                    //did this creature already die?
                    if (!map.Creatures.Values.Contains(kvp.Value)) continue;

                    Point moveTarget = GetMoveTarget(kvp.Key, kvp.Value, map);
                    if (moveTarget != null)
                    {
                        MoveToward(kvp.Key, moveTarget, kvp.Value, map);
                    }
                    else
                    {
                        moveTarget = kvp.Key;
                    }
                    Creature enemy = GetAdjacentEnemyToAttack(moveTarget, kvp.Value, map);
                    if (enemy != null) Attack(kvp.Value, enemy, map);
                    creaturesDone++;
                    if (!map.AnyFightLeft)
                    {
                        if(creaturesDone != map.Creatures.Count) roundComplete = false;
                        break;
                    }
                }
                Console.WriteLine((roundComplete ? "After" : "During") + $" Round {round}");
                Console.Write(map);
            }
            Console.WriteLine("------------- FINAL! ------------------");
            Console.WriteLine("------------- FINAL! ------------------");
            Console.WriteLine("------------- FINAL! ------------------");
            Console.WriteLine("------------- FINAL! ------------------");
            Console.WriteLine("------------- FINAL! ------------------");
            Console.Write(map);

            int answer = round * map.Creatures.Sum(c => c.Value.HitPoints);
            Console.WriteLine($"{round} * {map.Creatures.Sum(c => c.Value.HitPoints)} = {answer}");
            answer = (round-1) * map.Creatures.Sum(c => c.Value.HitPoints);
            Console.WriteLine($"{round - 1} * {map.Creatures.Sum(c => c.Value.HitPoints)} = {answer}");
        }

        public static void Part2()
        {
            Input = TestInput;
            Input = TestInput2;
            Input = TestInput3;
            Input = TestInput4;
            Input = TestInput5;
            //Input = TestInput99;

            Input = FullInput;
            //Input = FullInputAlt;

            for (int pwrLvl = 4; pwrLvl < 100; pwrLvl++)
            {
                Map map = Map.GetMap(Input, pwrLvl);
                int startingElfCount = map.Creatures.Count(kvp => kvp.Value is Elf);

                Console.Write(map);

                int round = 0;
                bool abort = false;
                while (map.AnyFightLeft && !abort)
                {
                    round++;
                    int creaturesDone = 0;
                    bool roundComplete = true;
                    IOrderedEnumerable<KeyValuePair<Point, Creature>> creatures = map.Creatures.OrderBy(kvp => kvp.Key);
                    foreach (var kvp in creatures)
                    {
                        //did this creature already die?
                        if (!map.Creatures.Values.Contains(kvp.Value)) continue;

                        Point moveTarget = GetMoveTarget(kvp.Key, kvp.Value, map);
                        if (moveTarget != null)
                        {
                            MoveToward(kvp.Key, moveTarget, kvp.Value, map);
                        }
                        else
                        {
                            moveTarget = kvp.Key;
                        }
                        Creature enemy = GetAdjacentEnemyToAttack(moveTarget, kvp.Value, map);
                        if (enemy != null)
                        {
                            Attack(kvp.Value, enemy, map);
                            int elfCount = map.Creatures.Count(kvp2 => kvp2.Value is Elf);
                            if (elfCount < startingElfCount) abort = true;
                        }
                        if (abort) break;
                        creaturesDone++;
                        if (!map.AnyFightLeft)
                        {
                            if (creaturesDone != map.Creatures.Count) roundComplete = false;
                            break;
                        }
                    }
                    if (abort)
                    {
                        Console.WriteLine("An elf died!  Need more power!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine((roundComplete ? "After" : "During") + $" Round {round}");
                        Console.Write(map);
                    }
                }
                if (!abort)
                {
                    Console.WriteLine("------------- FINAL! ------------------");
                    Console.WriteLine("------------- FINAL! ------------------");
                    Console.WriteLine("------------- FINAL! ------------------");
                    Console.WriteLine("------------- FINAL! ------------------");
                    Console.WriteLine("------------- FINAL! ------------------");
                    Console.Write(map);

                    int answer = round * map.Creatures.Sum(c => c.Value.HitPoints);
                    Console.WriteLine($"{round} * {map.Creatures.Sum(c => c.Value.HitPoints)} = {answer}");
                    answer = (round - 1) * map.Creatures.Sum(c => c.Value.HitPoints);
                    Console.WriteLine($"{round - 1} * {map.Creatures.Sum(c => c.Value.HitPoints)} = {answer}");
                    break;
                }
            }
        }

        private static void Attack(Creature attacker, Creature enemy, Map map)
        {
            enemy.HitPoints -= attacker.AttackPower;
            if(enemy.HitPoints <= 0)
            {
                Point enemyLoc = map.Creatures.Where(kvp => kvp.Value.Equals(enemy)).Single().Key;
                map.Creatures.Remove(enemyLoc);
                map.BuildGraph();
            }
        }

        private static Creature GetAdjacentEnemyToAttack(Point myPoint, Creature myself, Map map)
        {
            List<(Point, Creature)> adjacentCreatures = new List<(Point, Creature)>();
            foreach(Point p in AdjacentPoints(myPoint).OrderBy(p => p))
            {
                if(map.Creatures.ContainsKey(p) && map.Creatures[p].EnemyType == myself.GetType())
                {
                    adjacentCreatures.Add((p, map.Creatures[p]));
                }
            }

            if(!adjacentCreatures.Any()) return null;

            return adjacentCreatures.OrderBy(c => c.Item2.HitPoints).ThenBy(c => c.Item1).First().Item2;
        }

        private static Point GetMoveTarget(Point from, Creature movingCreature, Map map)
        {
            // Is this creature already next to an enemy?
            List<Point> myNeighbors = AdjacentPoints(from);
            if (map.Creatures.Where(kvp => myNeighbors.Contains(kvp.Key) && kvp.Value.GetType() == movingCreature.EnemyType).Any()) return null;

            // find a spot to move to
            HashSet<Point> spots = new HashSet<Point>();
            foreach(var enemyKvp in map.Creatures.Where(kvp => kvp.Value.GetType() == movingCreature.EnemyType).OrderBy(kvp => ManhattanDist(from, kvp.Key)))
            {
                List<Point> moveTargets = AdjacentPoints(enemyKvp.Key);
                foreach(Point p in moveTargets)
                {
                    if (map.Walls.Contains(p)) continue;
                    if (map.Creatures.Keys.Contains(p)) continue;
                    spots.Add(p);
                }
            }

            if (!spots.Any()) return null;
            (Point, int) result = map.GetPathDistPoint_BFS(from, spots);
            return result.Item1;
        }

        public static List<Point> AdjacentPoints(Point p)
        {
            List<Point> ap = new List<Point>();
            ap.Add(new Point(p.X, p.Y - 1));
            ap.Add(new Point(p.X, p.Y + 1));
            ap.Add(new Point(p.X - 1, p.Y));
            ap.Add(new Point(p.X + 1, p.Y));
            return ap;
        }


        private static void MoveToward(Point from, Point to, Creature value, Map map)
        {
            map.Creatures.Remove(from);
            map.Creatures.Add(to, value);
            map.BuildGraph();
        }

        public static int ManhattanDist(Point p1, Point p2) => (Math.Max(p1.X, p2.X) - Math.Min(p1.X, p2.X)) + (Math.Max(p1.Y, p2.Y) - Math.Min(p1.Y, p2.Y));

        public static string Input;

        public static string TestInput =
@"#######
#G..#E#
#E#E.E#
#G.##.#
#...#E#
#...E.#
#######";


        public static string TestInput2 =
@"#######
#E..EG#
#.#G.E#
#E.##E#
#G..#.#
#..E#.#
#######";

        public static string TestInput3 =
@"#######
#E.G#.#
#.#G..#
#G.#.G#
#G..#.#
#...E.#
#######";

        public static string TestInput4 =
@"#######
#.E...#
#.#..G#
#.###.#
#E#G#G#
#...#G#
#######";

        public static string TestInput5 =
@"#########
#G......#
#.E.#...#
#..##..G#
#...##..#
#...#...#
#.G...G.#
#.....G.#
#########";

        public static string TestInput99 =
@"#######
#######
#.E..G#
#.#####
#G#####
#######
#######";

        public static string FullInput =
@"################################
######......###...##..##########
######....#G###G..##.G##########
#####...G##.##.........#########
##....##..#.##...........#######
#....#G.......##.........G.#####
##..##GG....G.................##
##.......G............#.......##
###.....G.....G#......E.......##
##......##....................##
#.....####......G.....#...######
#.#########.G....G....#E.#######
###########...#####......#######
###########..#######..E.......##
###########.#########......#.###
########..#.#########.........##
#######G....#########........###
##.##.#.....#########...EE#..#.#
#...GG......#########.#...##..E#
##...#.......#######..#...#....#
###.##........#####......##...##
###.........................#..#
####.............##........###.#
####............##.........#####
####..##....###.#...#.....######
########....###..............###
########..G...##.###...E...E.###
#########...G.##.###.E....E.####
#########...#.#######.......####
#############..########...######
##############.########.########
################################";



        public static string FullInputAlt =
                    @"################################
###.GG#########.....#.....######
#......##..####.....G.G....#####
#..#.###...######..........#####
####...GG..######..G.......#####
####G.#...########....G..E.#####
###.....##########.........#####
#####..###########..G......#####
######..##########.........#####
#######.###########........###.#
######..########G.#G.....E.....#
######............G..........###
#####..G.....G#####...........##
#####.......G#######.E......#..#
#####.......#########......E.###
######......#########........###
####........#########.......#..#
#####.......#########.........##
#.#.E.......#########....#.#####
#............#######.....#######
#.....G.G.....#####.....########
#.....G.................########
#...G.###.....#.....############
#.....####E.##E....##.##########
##############.........#########
#############....#.##..#########
#############.#######...########
############.E######...#########
############..####....##########
############.####...E###########
############..####.E.###########
################################";

    }

    public class Map
    {
        private Map() { }

        public static Map GetMap(string input, int elfPower=3)
        {
            Map map = new Map();
            int x = 0, y = 0;
            foreach(char c in input)
            {
                if (c == '\n') { x = 0; y++; continue; }
                else if (c == 'G') map.Creatures.Add(new Point(x, y), new Goblin());
                else if (c == 'E') map.Creatures.Add(new Point(x, y), new Elf(elfPower));
                else if (c == '#') map.Walls.Add(new Point(x, y));

                x++;
            }
            map.BoundingRect = new Rect(0,0,x,y+1);
            map.BuildGraph();

            return map;
        }

        public bool AnyFightLeft => Creatures.Any(kvp => kvp.Value.Symbol == 'E') && Creatures.Any(kvp => kvp.Value.Symbol == 'G');

        public Dictionary<Point, Creature> Creatures { get; set; } = new Dictionary<Point, Creature>();
        public HashSet<Point> Walls { get; set; } = new HashSet<Point>();
        public Rect BoundingRect { get; set; }
        public Dictionary<Point, List<Point>> AdjacencyList;

        public (Point, int) GetPathDistPoint_BFS(Point from, HashSet<Point> spots)
        {
            Dictionary<int, List<Point>> distancePoints = new Dictionary<int, List<Point>>() { { 0, new List<Point> { from } } };
            Queue<Point> openSet = new Queue<Point>();
            HashSet<Point> closedSet = new HashSet<Point>();
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
            cameFrom[from] = null;
            openSet.Enqueue(from);

            bool found = false;
            Point closestSpot = null;
            while (openSet.Any())
            {
                Point currentRoot = openSet.Dequeue();
                if (spots.Contains(currentRoot))
                {
                    found = true;
                }

                foreach (Point p in Fifteen.AdjacentPoints(currentRoot).OrderBy(p => p))
                {
                    if (Walls.Contains(p) || Creatures.ContainsKey(p)) continue;
                    if (closedSet.Contains(p)) continue;

                    if (!openSet.Contains(p))
                    {
                        cameFrom[p] = currentRoot;
                        openSet.Enqueue(p);
                    }
                }
                closedSet.Add(currentRoot);
            }

            if (found)
            {
                spots.RemoveWhere(p => !cameFrom.ContainsKey(p));

                Dictionary<int, List<Point>> results = new Dictionary<int, List<Point>>();
                foreach (Point spot in spots)
                {
                    int dist = 0;
                    Point current = spot;
                    List<Point> totalPath = new List<Point>() { current };
                    while (current != null && cameFrom.ContainsKey(current))
                    {
                        dist++;
                        current = cameFrom[current];
                        totalPath.Add(current);
                    }
                    if (!results.ContainsKey(dist)) results.Add(dist, new List<Point>());
                    results[dist].Add(totalPath[totalPath.Count - 3]);
                }

                int minDist = results.Keys.Min();
                Point minPoint = results[minDist].OrderBy(p => p).First();
                return (minPoint, minDist);
            }
            else
            {
                return (null, int.MaxValue);
            }

        }

        public (Point, int) GetPathDistPoint_AStar(Point from, Point to)
        {
            List<Point> closedSet = new List<Point>();
            List<Point> openSet = new List<Point>() { from };
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
            Dictionary<Point, int> gScore = new Dictionary<Point, int>() { { from, 0 } };
            Dictionary<Point, int> fScore = new Dictionary<Point, int>() { { from, Fifteen.ManhattanDist(from, to) } };

            bool found = false;
            while(openSet.Any())
            {
                Point current = openSet.OrderBy(p => fScore[p]).First();
                if (current.Equals(to)) { found = true; break; }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach(Point p in Fifteen.AdjacentPoints(current).OrderBy(p => p))
                {
                    if (Walls.Contains(p) || Creatures.ContainsKey(p)) continue;
                    if (closedSet.Contains(p)) continue;
                    int tentativeGScore = gScore[current] + 1;
                    if (!openSet.Contains(p)) openSet.Add(p);
                    else if (tentativeGScore >= gScore[p]) continue;

                    cameFrom[p] = current;
                    gScore[p] = tentativeGScore;
                    fScore[p] = Fifteen.ManhattanDist(p, to);
                    //fScore[p] = tentativeGScore + Fifteen.ManhattanDist(p, to) + p.X + p.Y;
                }
            }

            if(found)
            {
                int dist = 0;
                Point current = to;
                List<Point> totalPath = new List<Point>() { current };
                while(cameFrom.ContainsKey(current))
                {
                    dist++;
                    current = cameFrom[current];
                    totalPath.Add(current);
                }
                return (totalPath[totalPath.Count - 2], dist);
            }
            else
            {
                return (null, int.MaxValue);
            }
        }

        public void BuildGraph()
        {
            AdjacencyList = new Dictionary<Point, List<Point>>();
            for (int y = 0; y < BoundingRect.Height; y++)
            {
                for (int x = 0; x < BoundingRect.Width; x++)
                {
                    Point p = new Point(x, y);
                    if (Walls.Contains(p) || Creatures.ContainsKey(p)) continue;

                    AdjacencyList.Add(p, new List<Point>());
                    List<Point> adjacentList = Fifteen.AdjacentPoints(p);
                    foreach(Point ap in adjacentList)
                    {
                        if (Walls.Contains(ap) || Creatures.ContainsKey(ap)) continue;
                        AdjacencyList[p].Add(ap);
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCreatures = new StringBuilder();
            for (int y = 0; y < BoundingRect.Height; y++)
            {
                for (int x = 0; x < BoundingRect.Width; x++)
                {
                    Point p = new Point(x,y);
                    if (Walls.Contains(p)) sb.Append("#");
                    else if (Creatures.ContainsKey(p))
                    {
                        sb.Append(Creatures[p].Symbol);
                        sbCreatures.Append($"{Creatures[p].Symbol}({Creatures[p].HitPoints}),");
                    }
                    else sb.Append(".");
                }
                sb.Append("\t");
                sb.Append(sbCreatures);
                sbCreatures.Clear();
                sb.Append("\n");
            }
            //foreach (var kvp in Creatures) sb.Append($"{kvp.Key} ({kvp.Value.Symbol}): HP {kvp.Value.HitPoints}\n");

            return sb.ToString();
        }
    }

    public abstract class Creature
    {
        protected static int _nextId = 0;
        public Creature() { ID = _nextId++; }

        public int ID { get; protected set; }
        public int HitPoints { get; set; } = 200;
        public int AttackPower { get; set; } = 3;
        public abstract char Symbol { get; }
        public abstract char EnemySymbol { get; }
        public abstract Type EnemyType { get; }

        public override bool Equals(object obj)
        {
            Creature creature = obj as Creature;
            if (obj == null) return false;
            return base.Equals(creature);
        }

        public bool Equals(Creature other) => other.ID == ID;

        public override int GetHashCode() => ID.GetHashCode();
    }

    public class Elf : Creature
    {
        public Elf(int power)
        {
            AttackPower = power;
        }
        public override char Symbol => 'E';
        public override char EnemySymbol => 'G';

        public override Type EnemyType => typeof(Goblin);
    }
    public class Goblin : Creature
    {
        public override char Symbol => 'G';
        public override char EnemySymbol => 'E';
        public override Type EnemyType => typeof(Elf);
    }

}
