/// ---------------------------------------------------------------------------
/// UnitType.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 25th, 2017</date>
/// ---------------------------------------------------------------------------

namespace core.units
{
    /// <summary>
    /// The enum contain the unit categories
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// This Unit is player controllable
        /// </summary>
        PLAYER,

        /// <summary>
        /// Indicates this unit is an enemy
        /// </summary>
        ENEMY,

        /// <summary>
        /// This unit represents part of the environment
        /// </summary>
        ENVIRONMENT
    }
}