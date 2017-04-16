/// ---------------------------------------------------------------------------
/// PathArrowTileNode.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 15th, 2017</date>
/// ---------------------------------------------------------------------------

namespace core.tilesys.pathing
{
    public class PathArrowTileNode
    {
        public int x;
        public int y;
        public PathArrowTileEnum type;

        public PathArrowTileNode(int x, int y, PathArrowTileEnum type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}