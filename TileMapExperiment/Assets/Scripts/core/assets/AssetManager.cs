
/// ---------------------------------------------------------------------------
/// UnitController.cs
/// 
/// <author>Don Duy Bui</author>
/// <date>March 13th, 2017</date>
/// ---------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace core.assets
{
    /// <summary>
    /// This class handles the loading, unloading, storing, and looking up of
    /// assets so we don't have to pepper the code base with heavy loading
    /// logic. 
    /// </summary>
    public class AssetManager
    {
        /// The singleton instance for AssetManager
        private static AssetManager instance;

        /// The dictionary containing all of the preloaded sprites. 
        private Dictionary<string, Sprite> spriteDirectory;

        private AssetManager()
        {
            spriteDirectory = new Dictionary<string, Sprite>();
        }

        public static AssetManager GetInstance()
        {
            if (instance == null)
            {
                instance = new AssetManager();
            }

            return instance;
        }

        /// <summary>
        /// Fetches a sprte from the sprite directory. 
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetPreloadedSprite(string spriteName)
        {
            if (!spriteDirectory.ContainsKey(spriteName))
            {
                Debug.LogError("Trying to fetch a sprite which has not been" +
                    "loaded yet: " + spriteName);
                return null;
            }

            return spriteDirectory[spriteName];
        }

        /// <summary>
        /// Preloads a multisprite sheet and then stores each of it's sprites
        /// in the sprite directory for quicker look up later.
        /// </summary>
        /// <param name="sheetPath"></param>
        public void PreloadMultiSpriteSheet(string sheetPath)
        {
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>(sheetPath);

            for (int i = 0, count = loadedSprites.Length; i < count; i++)
            {
                Sprite sprite = loadedSprites[i];

                if (spriteDirectory.ContainsKey(sprite.name))
                {
                    // No need to load the same asset name twice
                    continue;
                }

                spriteDirectory[sprite.name] = sprite;
            }
        }

        /// <summary>
        /// Preloads the given sprite sheet and then returns the sprite with
        /// the given name. 
        /// </summary>
        /// <param name="sheetPath"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadFromMultiSpriteSheet(string sheetPath, string spriteName)
        {
            PreloadMultiSpriteSheet(sheetPath);

            return GetPreloadedSprite(spriteName);
        }
    }
}