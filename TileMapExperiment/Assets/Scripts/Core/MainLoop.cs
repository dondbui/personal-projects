/// ---------------------------------------------------------------------------
/// Mainloop.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 5th, 2017</date>
/// ---------------------------------------------------------------------------

using core.anim;
using core.assets;
using core.tilesys;
using core.units;
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

    private Vector3 lastPosition;
    private Vector3 currPosition;

    private Vector3 endPosition = new Vector3(0,0,-10);

    private float zoomLevel = 5.0f;

    private Vector3 moveDelta;

    private GameObject selectionTile;

    void Start ()
    {
        DateTime startDate = DateTime.Now;
        Debug.Log("Game Initializing");

        PreloadAssets();

        MapController.GetInstance().LoadMapData("MapData/spacemap1");

        zoomLevel = Camera.main.orthographicSize;

        Debug.Log("ZOOM LEVEL: " + zoomLevel);

        // Create the selection tile
        CreateSelectionTile();

        // Create the units
        UnitController uc = UnitController.GetInstance();
        GameObject ship = uc.PlaceNewUnit("ship", "shipAssets_77");

        RandomlyPlaceEnemies();

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

        UnitController.GetInstance().Update();
        AnimController.GetInstance().Update();
    }

    private void PreloadAssets()
    {
        AssetManager assetManager = AssetManager.GetInstance();

        // Preload some assets.
        assetManager.PreloadSpriteSheet("Textures/ShipAssets");
        assetManager.PreloadSpriteSheet("Textures/selectionTile");
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

        Rect cameraBounds = MapController.CAMERA_BOUND;

        float endXPos = Mathf.Clamp(predictedX, cameraBounds.xMin, cameraBounds.xMax);
        float endYPos = Mathf.Clamp(predictedY, cameraBounds.yMin, cameraBounds.yMax);

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

        Vector2 tilePos = MapCoordinateUtils.GetTilePosFromClickPos(clickPos);

        GameObject ship = UnitController.GetInstance().GetUnitByID("ship");
        AnimController.GetInstance().ForceMoveUnitToTile(ship, tilePos);

        // Set the selection tile to the clicked position
        selectionTile.SetActive(true);
        selectionTile.transform.position = MapCoordinateUtils.GetTileToWorldPosition(tilePos);
    }

    private void CreateSelectionTile()
    {
        selectionTile = new GameObject();
        SpriteRenderer sr = selectionTile.AddComponent<SpriteRenderer>();
        sr.sprite = AssetManager.GetInstance().GetPreloadedSprite("selectionTile");

        selectionTile.name = "selectionTile";

        selectionTile.transform.position = new Vector3(0.5f, -0.5f, 0);
        selectionTile.SetActive(false);
    }

    private void RandomlyPlaceEnemies()
    {
        System.Random rand = new System.Random();
        Vector2 enemyPos = new Vector2();

        int width = MapController.GetInstance().currentMap.GetWidth();
        int height = MapController.GetInstance().currentMap.GetHeight();

        for (int i = 0; i < 10; i++)
        {
            int randX = rand.Next(0, width);
            int randY = rand.Next(0, height);

            enemyPos.x = randX;
            enemyPos.y = randY;

            UnitController.GetInstance().PlaceNewUnit("enemy" + i, "shipAssets_4", enemyPos);
        }
    }
}
