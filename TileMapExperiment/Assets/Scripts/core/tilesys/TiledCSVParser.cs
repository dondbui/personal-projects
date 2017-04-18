/// ---------------------------------------------------------------------------
/// CSVParser.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 5th, 2017</date>
/// ---------------------------------------------------------------------------

using System;
using UnityEngine;

namespace core.tilesys
{
    /// <summary>
    /// Handles the parsing of the tiled map data which is in CSV format.
    /// </summary>
    public class TiledCSVParser
    {
        private static TiledCSVParser instance = null;

        private TiledCSVParser()
        {
            
        }

        public static TiledCSVParser GetInstance()
        {
            if (instance == null)
            {
                instance = new TiledCSVParser();
            }

            return instance;
        }
        /// <summary>
        /// Reads the given csv file path name in. Reminder though, since this
        /// uses the 
        /// </summary>
        public string[,] ReadTiledCSVFile(string filePath)
        {
            Debug.Log("ReadTiledCSVFile Parsing: " + filePath);
            TextAsset data = Resources.Load(filePath) as TextAsset;
            Debug.Log("Raw Data: \n" + data.text);

            string[] newLineSplit = new string[] { System.Environment.NewLine };

            // Since this is a grid, split it by the new line first
            string[] rows = data.text.Split(newLineSplit, StringSplitOptions.None);

            // The height of the tile map. We need to subtract 1 since Tiled
            // tends to add an extra new line in there at the end. 
            int tileHeight = rows.Length - 1;

            string[,] tileMapLayout = null;

            // Go through from the bottom up since Unity likes a right up coordinate
            // system and tile prefers a right down we gotta compensate for that. 
            for (int y = tileHeight - 1; y >= 0; y--)
            {
                // Used to handle an empty row due to the extra new line
                // added by Tiled
                if (rows[y].Length == 0)
                {
                    continue;
                }

                string[] columns = rows[y].Split(',');

                // If we haven't made the map yet we can do it now since at this 
                // point we know the full dimensions of the map.
                if (tileMapLayout == null)
                {
                    int tileWidth = columns.Length;

                    tileMapLayout = new string[tileWidth, tileHeight];

                }

                // Now go through the row, column by column, and add their tile
                // to the two dimensional array. 
                for (int x = 0, hCount = columns.Length; x < hCount; x++)
                {
                    tileMapLayout[x, tileHeight - y - 1] = columns[x];
                }
            }

            return tileMapLayout;
        }
    }
}

