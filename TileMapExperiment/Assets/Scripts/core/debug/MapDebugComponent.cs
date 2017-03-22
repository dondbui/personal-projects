/// ---------------------------------------------------------------------------
/// MapDebugComponent.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 21, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Component which contains all of the debugging tools. 
/// </summary>
[CustomEditor(typeof(MapDebugComponent))]
public class MapDebugComponent : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // This button draws the debug lines for occupied tiles
        if (GUILayout.Button("Show Occupied Tiles"))
        {
            MapController.GetInstance().PrintOutOccupiedTiles();
        }
    }
}
