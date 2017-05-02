/// ---------------------------------------------------------------------------
/// PathGraphNode.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>May 1st, 2017</date>
/// ---------------------------------------------------------------------------

namespace core.tilesys.pathing
{
    /// <summary>
    /// The graph node for each pathing tile. Contains references to the tiles 
    /// adjacent to it. 
    /// </summary>
    public class PathGraphNode
    {
        public PathGraphNode up;
        public PathGraphNode down;
        public PathGraphNode left;
        public PathGraphNode right;

        public int x;
        public int y;

        public PathGraphNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}