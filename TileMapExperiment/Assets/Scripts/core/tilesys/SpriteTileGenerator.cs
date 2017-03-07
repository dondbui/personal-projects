/**
* SpriteTileGenerator.cs
* 
* <author>Don Duy Bui</author>
* <date>March 6th, 2017</date>
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace core.tilesys
{
    /// <summary>
    /// This is the brute force method of creating tiles as individual objects
    /// and throwing them on screen. Not ideal but is meant as a quick proof 
    /// of concept to understand the basics of the Tiled Data Structure
    /// </summary>
    public class SpriteTileGenerator
    {
        private const int TILE_WIDTH = 1;

        Dictionary<string, Sprite> tileDictionary;

        public SpriteTileGenerator(MapData mapdata)
        {
            // Load up all of the sprites
            Sprite[] tiles = Resources.LoadAll<Sprite>("Textures/Tiles");

            // add them to a dictionary for quicker look up later
            tileDictionary = new Dictionary<string, Sprite>();
            for (int i = 0, count = tiles.Length; i < count; i++)
            {
                tileDictionary[tiles[i].name] = tiles[i];
            }

            for (int y = 0, height = mapdata.GetHeight(); y < height; y++)
            {
                for (int x = 0, width = mapdata.GetWidth(); x < width; x++)
                {
                    string tileID = mapdata.GetTileAt(x, y);

                    AddTileToWorld(x, y, tileID);
                }
            }

            
        }
       
        public void AddTileToWorld(int x, int y, string tileID)
        {
            GameObject tileObj = new GameObject();

            SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
            sr.sprite = tileDictionary["Tiles_0" + tileID];

            tileObj.name = "Tile" + x + "_" + y;

            tileObj.transform.position = new Vector3(x * TILE_WIDTH, -y * TILE_WIDTH, 0);
                
                
        }
    }
}