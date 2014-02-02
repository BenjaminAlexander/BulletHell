using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGame.Utils;
using MyGame.Geometry;
namespace MyGame.DrawingUtils
{
    public class Collidable : Drawable
    {

        //float scale = 1;
        CollisionTexture texture;

        public Collidable(CollisionTexture texture, Color color, Vector2 origin, float depth)
            : base(texture.Texture, color, origin, depth)
        {
            this.texture = texture;
        }

        public Boolean CollidesWith(Vector2 position, float rotation, Collidable other, Vector2 otherPosition, float otherRotation)
        {
            //return Vector2.Distance(other.position, position) < 200;

            Rectangle tb = this.BoundingRectangle(position, rotation);
            Rectangle ob = other.BoundingRectangle(otherPosition, otherRotation);

            Circle thisCirlce = Circle.CreateBounding(tb);
            Circle otherCirlce = Circle.CreateBounding(ob);

            if (thisCirlce.Intersects(otherCirlce) && tb.Intersects(ob))
            {

                Color[] thisTextureData = texture.Data;

                Color[] otherTextureData = other.texture.Data;

                /*if (IntersectPixels(this.GetWorldTransformation(), this.Texture.Width,
                                        this.Texture.Height, thisTextureData,
                                        other.GetWorldTransformation(), other.Texture.Width,
                                        other.Texture.Height, otherTextureData))
                //if(MyIntersectPixels(this, other))
                {
                    return true;
                }*/
                if (MyIntersectPixels(this, other, position, rotation, otherPosition, otherRotation))
                {
                    return true;
                }
            }
            return false;
        }

        /*
        public Boolean CollidesWith(Vector2 point1, Vector2 point2)
        {
            
            Line line = new Line(point1, point2);
            Rectangle bounding = this.BoundingRectangle();
            Rectangle boundingLine = line.BoundingRectangle();
            return bounding.Intersects(boundingLine);
        }*/

        private class NotEnoughPoints : Exception { };

        //This one is better because it only checks the part the bounding rectangeles that intersect instead of the whole texture
        private static bool MyIntersectPixels(Collidable d1, Collidable d2, Vector2 position1, float rotation1, Vector2 position2, float rotation2)
        {
            Rectangle d1Bound = d1.BoundingRectangle(position1, rotation1);
            Rectangle d2Bound = d2.BoundingRectangle(position2, rotation2);

            Rectangle intersectArea;
            Rectangle.Intersect(ref d1Bound, ref d2Bound, out intersectArea);

            Matrix inversTransform1 = Matrix.Invert(d1.GetWorldTransformation(position1, rotation1));
            Matrix inversTransform2 = Matrix.Invert(d2.GetWorldTransformation(position2, rotation2));

            Color[] data1 = d1.texture.Data;
            Color[] data2 = d2.texture.Data;

            //randomly selecting a pixels to check instead of iterating through rows would improve performance
            for (int worldX = intersectArea.X; worldX < intersectArea.X + intersectArea.Width; worldX++)
            {
                for (int worldY = intersectArea.Y; worldY < intersectArea.Y + intersectArea.Height; worldY++)
                {
                    Vector3 pos = new Vector3(worldX, worldY, 0);

                    Vector3 texture1Pos = Vector3.Transform(pos, inversTransform1);
                    Vector3 texture2Pos = Vector3.Transform(pos, inversTransform2);

                    int x1 = (int)Math.Round(texture1Pos.X);
                    int y1 = (int)Math.Round(texture1Pos.Y);

                    int x2 = (int)Math.Round(texture2Pos.X);
                    int y2 = (int)Math.Round(texture2Pos.Y);

                    
                    if (0 <= x1 && x1 < d1.Texture.Width &&
                        0 <= y1 && y1 < d1.Texture.Height &&
                        0 <= x2 && x2 < d2.Texture.Width &&
                        0 <= y2 && y2 < d2.Texture.Height)
                    {
                        Color color1 = data1[x1 + y1 * d1.Texture.Width];
                        Color color2 = data2[x2 + y2 * d2.Texture.Width];

                        // If both pixels are not completely transparent,
                        if (color1.A != 0 && color2.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public Boolean Contains(Vector2 point, Vector2 position, float rotation)
        {
            if (this.BoundingRectangle(position, rotation).Contains(point))
            {
                Matrix inversTransform = Matrix.Invert(this.GetWorldTransformation(position, rotation));
                Color[] data = this.texture.Data;

                Vector2 texturePos = Vector2.Transform(point, inversTransform);
                int x = (int)Math.Round(texturePos.X);
                int y = (int)Math.Round(texturePos.Y);

                if (0 <= x && x < this.Texture.Width &&
                        0 <= y && y < this.Texture.Height)
                {
                    Color color = data[x + y * this.Texture.Width];
                    if (color.A != 0 )
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Microsoft XNA Community Game Platform
        /// Copyright (C) Microsoft Corporation. All rights reserved.
        /// 
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
    }

}
