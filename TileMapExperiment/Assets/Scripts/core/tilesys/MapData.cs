/**
* MapData.cs
* 
* <author>Don Duy Bui</author>
* <date>March 6th, 2017</date>
*/
using UnityEngine;

namespace core.tilesys
{
    public class MapData
    {
        private string[,] tileData;

        public MapData(string[,] data)
        {
            tileData = data;

            Debug.Log("Width: " + GetWidth().ToString());
            Debug.Log("Height: " + GetHeight().ToString());

        }

        public string GetTileAt(int x, int y)
        {
            return tileData[x,y];
        }

        public int GetHeight()
        {
            return tileData.GetLength(1);
        }

        public int GetWidth()
        {
            return tileData.GetLength(0);
        }
    }
    
}
