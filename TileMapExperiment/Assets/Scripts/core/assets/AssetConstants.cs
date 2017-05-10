/// ---------------------------------------------------------------------------
/// AssetConstants.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>May 7th, 2017</date>
/// ---------------------------------------------------------------------------

namespace core.assets
{
    /// <summary>
    /// Contains constants related to assets and asset loading
    /// </summary>
    public class AssetConstants
    {
        /// <summary>
        /// Mapdata for the default map
        /// </summary>
        public const string MAP_DEFAULT_MAP = "MapData/spacemap1";

        /// <summary>
        /// Material for the blue grid overlay over the map
        /// </summary>
        public const string MAT_GRID_OVERLAY = "Materials/gridOverlay";

        /// <summary>
        /// Material for the movement arrows when selecting unit movement destination
        /// </summary>
        public const string MAT_PATHING_ARROWS = "Materials/pathingArrows";

        /// <summary>
        /// Material that represents the selected tile/unit
        /// </summary>
        public const string MAT_SELECTED_TILE = "Materials/occupied";

        /// <summary>
        /// The default material for a map
        /// </summary>
        public const string MAT_DEFAULT_MAP_TILES = "Materials/spacetiles";

        /// <summary>
        /// The prefab for the enemy in the void beacon
        /// </summary>
        public const string PF_BEACON = "Prefabs/Beacon";

        /// <summary>
        /// The sprite for the red seleciton tile. 
        /// </summary>
        public const string SPR_SELECTION_TILE = "selectionTile";

        /// <summary>
        /// The texture for the asteroids
        /// </summary>
        public const string TEX_ASTEROID = "Textures/asteroid";

        /// <summary>
        /// The texture for the boss ship
        /// </summary>
        public const string TEX_BOSS = "Textures/boss";

        /// <summary>
        /// Texture for the pathing arrows
        /// </summary>
        public const string TEX_PATHING_ARROWS = "Textures/pathingArrows";

        /// <summary>
        /// Texture for the selection tile
        /// </summary>
        public const string TEX_SELECTION_TILE = "Textures/selectionTile";

        /// <summary>
        /// Texture for all of the ships
        /// </summary>
        public const string TEX_SHIPS = "Textures/ShipAssets";
    }
}