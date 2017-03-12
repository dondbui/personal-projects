/// ---------------------------------------------------------------------------
/// Mainloop.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 5th, 2017</date>
/// ---------------------------------------------------------------------------

using core.tilesys;
using UnityEngine;

/// <summary>
/// This class handles the main game loop of the system. 
/// </summary>
public class MainLoop : MonoBehaviour
{
    private const int DRAG_SPEED = 10;

    private Vector3 lastPosition;
    private Vector3 currPosition;

    private Vector3 moveDelta;

    void Start ()
    {
        Debug.Log("Game Initializing");

        TiledCSVParser csvParser = TiledCSVParser.GetInstance();
        string[,] rawData = csvParser.ReadTiledCSVFile("MapData/map1");

        MapData mapData = new MapData(rawData);
        Debug.Log("Tile at 0,0: " + mapData.GetTileAt(0, 0));
        Debug.Log("Tile at 0,31: " + mapData.GetTileAt(0, 31));

        // SpriteTileGenerator stg = new SpriteTileGenerator(mapData);

        new TileMeshGenerator(mapData);
    }
    
    /// <summary>
    /// All of the core game update loop checks should occur here. It's nice
    /// because it'll be our core central point of updates for most of the 
    /// game. 
    /// </summary>
    void Update ()
    {
        CheckCameraPan();
    }

    /// <summary>
    /// Checks to see if the user is panning the camera. If so then handle the 
    /// mouse movement deltas and properly handle the moving of the camera. 
    /// </summary>
    private void CheckCameraPan()
    {
        // Mouse isn't down then we don't care. 
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = currPosition = Input.mousePosition;
            return;
        }

        // set the last current position to the last one since now we're on a
        // a new update and positions have possibly changed
        lastPosition = currPosition;

        // set our current position.
        currPosition = Input.mousePosition;

        // figure out our view port delta. 
        Vector3 viewPortPos = Camera.main.ScreenToViewportPoint(currPosition - lastPosition);

        // Update the moveDelta positions so we can just re-use this guy
        // instead of constantly making new ones. 
        moveDelta.x = -viewPortPos.x * DRAG_SPEED;
        moveDelta.y = -viewPortPos.y * DRAG_SPEED;

        Camera.main.transform.Translate(moveDelta, Space.World);

        // TODO: add inertia to this camera panning later. 

        // TODO: add bounding to the camera movement so we don't move into the void 
        // and get lost. 
    }
}
