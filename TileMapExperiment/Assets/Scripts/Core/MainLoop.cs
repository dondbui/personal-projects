/// ---------------------------------------------------------------------------
/// Mainloop.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 5th, 2017</date>
/// ---------------------------------------------------------------------------

using core;
using core.assets;
using core.tilesys;
using System;
using UnityEngine;

/// <summary>
/// This class handles the main game loop of the system. 
/// </summary>
public class MainLoop : MonoBehaviour
{
    private const int DRAG_SPEED = 10;

    private const float ZOOM_MIN = 0.5f;
    private const float ZOOM_MAX = 10f;
    private const float SCROLL_MODIFIER = 1;

    /// Stores the bounding box of the world. 
    private static Rect CAMERA_BOUND = new Rect();

    private Vector3 lastPosition;
    private Vector3 currPosition;

    private Vector3 endPosition = new Vector3(0,0,-10);

    private float zoomLevel = 5.0f;

    private Vector3 moveDelta;

    void Start ()
    {
        DateTime startDate = DateTime.Now;
        Debug.Log("Game Initializing");

        PreloadAssets();

        TiledCSVParser csvParser = TiledCSVParser.GetInstance();
        string[,] rawData = csvParser.ReadTiledCSVFile("MapData/map1");

        MapData mapData = new MapData(rawData);
        Debug.Log("Tile at 0,0: " + mapData.GetTileAt(0, 0));
        Debug.Log("Tile at 0,31: " + mapData.GetTileAt(0, 31));

        // Setup the bounds of the world
        CAMERA_BOUND.x = -1;
        CAMERA_BOUND.y = -1 - mapData.GetHeight();
        CAMERA_BOUND.width = mapData.GetWidth();
        CAMERA_BOUND.height = mapData.GetHeight();

        zoomLevel = Camera.main.orthographicSize;

        Debug.Log("ZOOM LEVEL: " + zoomLevel);

        new TileMeshGenerator(mapData);

        // Create the units
        UnitController uc = UnitController.GetInstance();
        uc.PlaceNewUnit("ship", "shipAssets_0");
        uc.MoveUnitToTile("ship", 4, 0);

        DateTime endDate = DateTime.Now;

        Debug.Log("Total Initialization Time: " + 
            endDate.Subtract(startDate).TotalMilliseconds + " MS");
    }
    
    /// <summary>
    /// All of the core game update loop checks should occur here. It's nice
    /// because it'll be our core central point of updates for most of the 
    /// game. 
    /// </summary>
    void Update ()
    {
        bool isPanning = CheckCameraPan();
        CheckForZoomUpdate();
        
        // If we're panning then no need to check for mouse clicks
        if (!isPanning)
        {
            CheckMouseReleased();
        }

        UnitController uc = UnitController.GetInstance();
        uc.Update();
    }

    private void PreloadAssets()
    {
        AssetManager assetManager = AssetManager.GetInstance();

        // Preload some assets.
        assetManager.PreloadMultiSpriteSheet("Textures/ShipAssets");
    }

    /// <summary>
    /// Checks to see if the user is panning the camera. If so then handle the 
    /// mouse movement deltas and properly handle the moving of the camera. 
    /// </summary>
    private bool CheckCameraPan()
    {
        // Mouse isn't down then we don't care. 
        if (!Input.GetMouseButton(0))
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = currPosition = Input.mousePosition;
            return false;
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

        // Attempt to predict the final position
        float predictedX = moveDelta.x + Camera.main.transform.position.x;
        float predictedY = moveDelta.y + Camera.main.transform.position.y;

        float endXPos = Mathf.Clamp(predictedX, CAMERA_BOUND.xMin, CAMERA_BOUND.xMax);
        float endYPos = Mathf.Clamp(predictedY, CAMERA_BOUND.yMin, CAMERA_BOUND.yMax);

        endPosition.x = endXPos;
        endPosition.y = endYPos;

        Camera.main.transform.position = endPosition;

        return true;

        // TODO: add inertia to this camera panning later. 
    }

    /// <summary>
    /// Checks to see if the user has scrolled the wheel in an attempt to zoom the map.
    /// </summary>
    private void CheckForZoomUpdate()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollDelta) < 0)
        {
            return;
        }

        // Let's try to delta the orthographicSize to adjust the zoom.
        // 
        // We need to clamp the results so the user is not able to zoom 
        // themselve into oblivion. 
        Camera.main.orthographicSize = 
            Mathf.Clamp(Camera.main.orthographicSize + scrollDelta, ZOOM_MIN, ZOOM_MAX);
    }

    /// <summary>
    /// Checks to see if the user has their mouse down. 
    /// </summary>
    private void CheckMouseReleased()
    {
        // Mouse isn't down then we don't care. 
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        Vector3 clickPos = Input.mousePosition;

        GetTilePosFromClickPos(clickPos);
    }

    /// <summary>
    /// Given a click position this returns to you the position of the click in
    /// tile space. 
    /// </summary>
    /// <param name="clickPos"></param>
    /// <returns></returns>
    private Vector2 GetTilePosFromClickPos(Vector3 clickPos)
    {
        Vector2 returnPos = new Vector2();

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null)
        {
            // The raw position. 
            // Debug.Log(hit.collider.name + ": " + hit.point.x + ", " + hit.point.y);

            returnPos.x = Mathf.FloorToInt(hit.point.x);
            returnPos.y = Mathf.CeilToInt(hit.point.y);
        }

        Debug.Log("returnPos: " + returnPos.ToString());
        return returnPos;
    }
}
