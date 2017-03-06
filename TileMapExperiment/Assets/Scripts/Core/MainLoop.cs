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
        string[,] mapData = csvParser.ReadTiledCSVFile("MapData/map1");

        
    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }
}
