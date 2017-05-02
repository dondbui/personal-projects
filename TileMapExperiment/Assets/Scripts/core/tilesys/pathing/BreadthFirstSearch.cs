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
        private PathGraphNode[,] graph;

        public BreadthFirstSearch()
        {
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
                        leftNode.right = node;
                        node.left = leftNode;
                        
                    }

                    if (y > 0)
                    {
                        PathGraphNode topNode = graph[x, y - 1];
                        topNode.down = node;
                        node.up = topNode;
                    }
                }
            }

            Debug.Log("Graph Node Made");
        }
    }
}