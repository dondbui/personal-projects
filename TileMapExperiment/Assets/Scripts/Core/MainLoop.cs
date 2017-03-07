/**
* Mainloop.cs
* 
* <author>Don Duy Bui</author>
* <date>March 5th, 2017</date>
*/

using core.tilesys;
using UnityEngine;

/// <summary>
/// This class handles the main game loop of the system. 
/// </summary>
public class MainLoop : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        Debug.Log("Game Initializing");

        TiledCSVParser csvParser = TiledCSVParser.GetInstance();
        string[,] rawData = csvParser.ReadTiledCSVFile("MapData/map1");

        MapData mapData = new MapData(rawData);
        Debug.Log("Tile at 0,0: " + mapData.GetTileAt(0,0));
        Debug.Log("Tile at 0,31: " + mapData.GetTileAt(0, 31));

        SpriteTileGenerator stg = new SpriteTileGenerator(mapData);
    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }
}
