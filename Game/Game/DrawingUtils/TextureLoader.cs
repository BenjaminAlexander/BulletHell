using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.DrawingUtils
{
    // This class acts as a dictionary for CollisionTextures, only loading each texture into a CollisionTexture once.
    static class TextureLoader
    {
        private static Dictionary<String, LoadedTexture> textures = new Dictionary<string, LoadedTexture>();
        private static ContentManager content;

        public static void Initialize(ContentManager c)
        {
            content = c;
        }

        // Returns a reference to the CollisionTexture with the name textureName.
        //TODO: All textures should be loaded before runtime.
        public static LoadedTexture GetTexture(String textureName)
        {
            if (content == null)
            {
                throw new Exception("Texture Loader has not been initialized with a content manager.");
            }

            if (textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }

            // If this is the first time, we need to load the texture!
            Texture2D texture;
            try
            {
                texture = content.Load<Texture2D>(textureName);
            }
            catch (Exception)
            {
                throw new Exception("There was a problem loading the texture.");
            }

            // Have CollisionTexture initialize and pre-calcuate and add the texture to the dictionary.
            LoadedTexture collisionTexture = new LoadedTexture(texture);
            textures[textureName] = collisionTexture;
            return collisionTexture;
        }
    }
}
