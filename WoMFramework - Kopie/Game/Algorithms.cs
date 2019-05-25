using System.Collections.Generic;
using GoRogue;
using GoRogue.MapViews;
using Priority_Queue;
using WoMFramework.Game.Generator.Dungeon;

namespace WoMFramework.Game
{
    public static class Algorithms
    {
        private static FastPriorityQueue<AStarNode> _cachedQueue;
        private static AStarNode[] _cache;
        private static Map _cachedMap;

        private class AStarNode : FastPriorityQueueNode
        {
            public readonly Coord Position;
            public bool Closed;

            // Whether or not the node has been closed
            public float F;

            // (Partly estimated) distance to end point going thru this node
            public float G;

            public AStarNode Parent;
            // (Known) distance from start to this node, by shortest known path

            public AStarNode(Coord position, AStarNode parent = null)
            {
                Parent = parent;
                Position = position;
                Closed = false;
                F = G = float.MaxValue;
            }

            public void Clear()
            {
                Parent = null;
                Closed = false;
                F = G = float.MaxValue;
            }
        }

        public static Coord[] AStar(Coord start, Coord goal, Map map)
        {
            AStarNode[] nodes;
            FastPriorityQueue<AStarNode> open;  // Currently discovered nodes that are not evaluated yet
            Distance euclidiean = Distance.EUCLIDEAN;
            ArrayMap<bool> walkabilityMap = map.WalkabilityMap;
            bool isGoalWalkable = true;

            if (_cachedMap?.Guid == map.Guid)
            {
                nodes = _cache;
                for (int i = 0; i < nodes.Length; i++)
                    nodes[i].Clear();
                open = _cachedQueue;
            }
            else
            {
                int length = walkabilityMap.Width * walkabilityMap.Height;
                nodes = new AStarNode[length];
                for (int i = 0; i < nodes.Length; i++)
                    nodes[i] = new AStarNode(Coord.ToCoord(i, walkabilityMap.Width));
                _cache = nodes;
                _cachedMap = map;
                open = new FastPriorityQueue<AStarNode>(length);
                _cachedQueue = open;
            }

            if (start == goal)
                return new[] { goal };

            if (!walkabilityMap[goal])
            {
                isGoalWalkable = false;
                walkabilityMap[goal] = true;
            }

            var result = new Stack<Coord>();
            int startIndex = start.ToIndex(walkabilityMap.Width);
            AStarNode startNode = nodes[startIndex];
            startNode.G = 0;
            startNode.F = (float)euclidiean.Calculate(start, goal);
            open.Enqueue(startNode, startNode.F);

            while (open.Count != 0)
            {
                AStarNode current = open.Dequeue();
                current.Closed = true;
                if (current.Position == goal)
                {
                    open.Clear();

                    if (!isGoalWalkable)
                    {
                        walkabilityMap[goal] = false;
                        current = current.Parent;
                    }

                    do
                    {
                        result.Push(current.Position);
                        current = current.Parent;
                    } while (current != null);

                    return result.ToArray();
                }

                Coord[] neighbours = GetReachableNeighbours(walkabilityMap, current.Position);
                foreach (Coord coord in neighbours)
                {
                    int index = coord.ToIndex(walkabilityMap.Width);
                    AStarNode node = nodes[index];

                    if (node.Closed) continue;

                    float distance = current.G + (float)euclidiean.Calculate(current.Position, coord);
                    if (distance >= node.G) continue;

                    node.Parent = current;
                    node.G = distance;
                    node.F = distance + (float) euclidiean.Calculate(coord, goal);

                    if (open.Contains(node))
                        open.UpdatePriority(node, node.F);
                    else
                        open.Enqueue(node, node.F);
                }
            }

            return null;
        }

        public static Coord[] GetReachableNeighbours(IMapView<bool> walkabilityMap, Coord current)
        {
            var result = new List<Coord>(8);

            int x = current.X;
            int y = current.Y;
            bool
                right = false,
                up = false,
                left = false,
                down = false;

            // Cardinals
            if (x < walkabilityMap.Width && walkabilityMap[x + 1, y])
            {
                result.Add(current.Translate(1, 0));
                right = true;
            }
   
            if (y < walkabilityMap.Height && walkabilityMap[x, y + 1])
            {
                result.Add(current.Translate(0, 1));
                up = true;
            }

            if (x > 0 && walkabilityMap[x - 1, y])
            {
                result.Add(current.Translate(-1, 0));
                left = true;
            }

            if (y > 0 && walkabilityMap[x, y - 1])
            {
                result.Add(current.Translate(0, -1));
                down = true;
            }

            // Diagonals
            // Can't move diagonally past a corner
            if (right)
            {
                if (up && walkabilityMap[x + 1, y + 1])
                    result.Add(current.Translate(1, 1));
                if (down && walkabilityMap[x + 1, y - 1])
                    result.Add(current.Translate(1, -1));
            }

            if (left)
            {
                if (up && walkabilityMap[x - 1, y + 1])
                    result.Add(current.Translate(-1, 1));
                if (down && walkabilityMap[x - 1, y - 1])
                    result.Add(current.Translate(-1, -1));
            }

            return result.ToArray();
        }

        //public static Coord DepthFirstExplore(Coord start, ISettableMapView<int> explorationMap,
        //    IMapView<bool> walkabilityMap, FOV fov)
        //{
        //    // exploration map
        //    // 0 : not visited;
        //    // 1 : observed;
        //    // 2 : visited node;
        //    // -1 : grey node;




        //    fov.Calculate(start, 5);

        //    foreach (var coordInSight in fov.CurrentFOV)
        //    {

        //    }
        //}
    }
}
