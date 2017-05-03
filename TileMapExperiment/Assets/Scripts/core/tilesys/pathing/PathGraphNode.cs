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
        

        public int x;
        public int y;

        public PathGraphNode[] adjacentNodes;

        public PathGraphNode(int x, int y)
        {
            this.x = x;
            this.y = y;

            // it's a tile so we know it's gonna be connecting pieces at most
            adjacentNodes = new PathGraphNode[4];
        }

        public PathGraphNode Up
        {
            get
            {
                return adjacentNodes[0];
            }
            set
            {
                adjacentNodes[0] = value;
            }
        }
        public PathGraphNode Down
        {
            get
            {
                return adjacentNodes[1];
            }
            set
            {
                adjacentNodes[1] = value;
            }
        }
        public PathGraphNode Left
        {
            get
            {
                return adjacentNodes[2];
            }
            set
            {
                adjacentNodes[2] = value;
            }
        }
        public PathGraphNode Right
        {
            get
            {
                return adjacentNodes[3];
            }
            set
            {
                adjacentNodes[3] = value;
            }
        }
    }
}