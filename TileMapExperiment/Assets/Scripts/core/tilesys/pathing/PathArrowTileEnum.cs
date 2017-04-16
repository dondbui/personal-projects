/// ---------------------------------------------------------------------------
/// PathArrowTileEnum.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>April 15th, 2017</date>
/// ---------------------------------------------------------------------------

namespace core.tilesys.pathing
{
    public enum PathArrowTileEnum
    {
        /// <summary>
        /// Indicates the tile is a vertical straight
        /// </summary>
        Vertical,

        /// <summary>
        /// Indicates that the tile should be a horizontal straight
        /// </summary>
        Horizontal,

        /// <summary>
        /// Indicates that the tile should be a top angle going up and
        /// to the right.
        /// </summary>
        TopToRight,

        /// <summary>
        /// Indicates that the tile should be a top angle going up and
        /// to the left.
        /// </summary>
        TopToLeft,

        /// <summary>
        /// Indicates that the tile should be a bottom angle going down
        /// and to the right.
        /// </summary>
        BottomToRight,

        /// <summary>
        /// Indicates that tile should be a bottom angle going down and 
        /// to the left.
        /// </summary>
        BottomToLeft,

        /// <summary>
        /// Indicates that the tile is a destination point and we should
        /// be showing the arrow head pointing up.
        /// </summary>
        EndArrowUp,

        /// <summary>
        /// Indicates that the tile is a destination point and we should
        /// be showing the arrow head pointing down.
        /// </summary>
        EndArrowDown,

        /// <summary>
        /// Indicates that the tile is a destination point and we should
        /// be showing the arrow head pointing left.
        /// </summary>
        EndArrowLeft,

        /// <summary>
        /// Indicates that the tile is a destination point and we should
        /// be showing the arrow head pointing right.
        /// </summary>
        EndArrowRight,
    }
}