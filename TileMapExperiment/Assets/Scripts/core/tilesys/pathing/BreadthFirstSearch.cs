

using System.Collections.Generic;
/// ---------------------------------------------------------------------------
/// BreadthFirstSearch.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>May 1st, 2017</date>
/// ---------------------------------------------------------------------------
using UnityEngine;

namespace core.tilesys.pathing
{
    /// <summary>
    /// The implementation for the Bread First pathfinding algorithm
    /// </summary>
    public class BreadthFirstSearch
    {
        /// <summary>
        /// The graph of tiles
        /// </summary>
        private PathGraphNode[,] graph;

        /// <summary>
        /// The array of nodes that have been already visited.
        /// 
        /// if x,y is not equal null then we have a value for it
        /// 
        /// the value will be the PathGraphNode that connects to
        /// this x,y
        /// 
        /// </summary>
        private PathGraphNode[,] visitedNodes;

        public BreadthFirstSearch()
        {
        }

        public void Calculate()
        {
            int startX = 15;
            int startY = 15;

            int endX = 15;
            int endY = 20;

            visitedNodes = new PathGraphNode[32, 32];

            Queue<PathGraphNode> queue = new Queue<PathGraphNode>();

            // add the starting node to the queue.
            queue.Enqueue(graph[startX, startY]);

            // Let's start pulling stuff off the queue
            while (queue.Count > 0)
            {
                PathGraphNode node = queue.Dequeue();

                for (int i = 0, numSides = 4; i < numSides; i++)
                {
                    PathGraphNode adjacent = node.adjacentNodes[i];

                    // If no adjacent node is defined for this side then bounce.
                    if (adjacent == null)
                    {
                        continue;
                    }

                    // Check if we've already seen it.

                    int x = adjacent.x;
                    int y = adjacent.y;

                    if (visitedNodes[x, y] == null)
                    {
                        queue.Enqueue(adjacent);

                        visitedNodes[x, y] = node;
                    }
                }

                if (node.x == endX && node.y == endY)
                {
                    break;
                }
            }

            // Now let's generate the step by step tile path from the destination to the starting point.
            int currX = endX;
            int currY = endY;

            List<PathGraphNode> path = new List<PathGraphNode>();

            PathGraphNode currNode = graph[endX, endY];

            path.Add(currNode);

            // Back trace the steps back to the starting position
            while (currX != startX || currY != startY)
            {
                PathGraphNode nextNode = visitedNodes[currX, currY];

                path.Add(nextNode);

                currX = nextNode.x;
                currY = nextNode.y;
            }

            // Print out the path to the starting point. 
            for (int i = 0, pathLength = path.Count; i < pathLength; i++)
            {
                PathGraphNode step = path[i];

                Debug.Log("Path Step " + i + " : " + step.x + ", " + step.y);
            }

        }

        public void GenerateMapGraph(MapData map)
        {
            int width = map.GetWidth();
            int height = map.GetHeight();

            graph = new PathGraphNode[width, height];

            // iterate through all of the tiles
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    PathGraphNode node = new PathGraphNode(x, y);
                    graph[x, y] = node;

                    // Link them to the one above and to the left
                    // which are the guaranteed previous nodes. 
                    if (x > 0)
                    {
                        PathGraphNode leftNode = graph[x - 1, y];
                        leftNode.Right = node;
                        node.Left = leftNode;
                        
                    }

                    if (y > 0)
                    {
                        PathGraphNode topNode = graph[x, y - 1];
                        topNode.Down = node;
                        node.Up = topNode;
                    }
                }
            }

            Debug.Log("Graph Node Made");
        }
    }
}