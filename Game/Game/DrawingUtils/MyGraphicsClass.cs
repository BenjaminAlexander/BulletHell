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

namespace MyGame.DrawingUtils
{
    public class MyGraphicsClass
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera camera;

        private static Texture2D line;
        private static Texture2D point;
        private static Texture2D circle;
        private static SpriteFont font;

        public static SpriteFont Font
        {
            get { return font; }
        }

        public GraphicsDeviceManager getGraphics()
        {
            return graphics;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public MyGraphicsClass(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Camera camera)
        {
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.camera = camera;
        }

        public static void LoadContent(ContentManager content)
        {
            line = content.Load<Texture2D>("line");
            point = content.Load<Texture2D>("point");
            circle = content.Load<Texture2D>("circleS");
            font = content.Load<SpriteFont>("SpriteFont1");
        }

        public void DrawCircle(Vector2 center, double radius, Color color, double depth)
        {
            int maxSegments = 50;
            //int segmentSize = 5;
            int segments = maxSegments;

            for (int i = 0; i < segments; i++)
            {
                Vector2 point1 = Vector2Utils.ConstructVectorFromPolar(radius, i * (2 * Math.PI / segments)) + center;
                Vector2 point2 = Vector2Utils.ConstructVectorFromPolar(radius, ((i + 1) % segments) * (2 * Math.PI / segments)) + center;
                DrawLine(point1, point2, color, depth);
            }
        }

        public void DrawLine(Vector2 point1, Vector2 point2, Color color, double depth)
        {
            Vector2 scale = new Vector2((float)Math.Sqrt((point2 - point1).LengthSquared()), 1);
            float rotation = (float)Vector2Utils.Vector2Angle(point2 - point1);
            spriteBatch.Draw(point, point1, null, color, rotation, new Vector2(0), scale, SpriteEffects.None, (float)depth);
        }

        public void DrawRectangle(Vector2 position, Vector2 size, Vector2 center, double rotation, Color color,double  depth)
        {
            Vector2 point1 = position + Vector2Utils.RotateVector2(-center, rotation);
            Vector2 point2 = point1 + Vector2Utils.RotateVector2(new Vector2(size.X, 0), rotation);
            Vector2 point3 = point1 + Vector2Utils.RotateVector2(size, rotation);
            Vector2 point4 = point1 + Vector2Utils.RotateVector2(new Vector2(0, size.Y), rotation);

            DrawLine(point1, point2, color, depth);
            DrawLine(point2, point3, color, depth);
            DrawLine(point3, point4, color, depth);
            DrawLine(point4, point1, color, depth);
        }

        public void DrawSolidRectangle(Vector2 position, Vector2 size, Vector2 center, double rotation, Color color, double depth)
        {
            Vector2 point1 = position + Vector2Utils.RotateVector2(-center, rotation);
            spriteBatch.Draw(point, point1, null, color, (float)rotation, new Vector2(0), size, SpriteEffects.None, (float)depth);
        }

        public void DrawSolidCircle(Vector2 center, double radius, Color color, double depth)
        {
            spriteBatch.Draw(circle, center, null, color, 0, new Vector2(circle.Width / 2, circle.Height / 2), (float)(radius/ ((float)circle.Width / 2)), SpriteEffects.None, (float)depth);
        }

        public void DrawSolidCircle(Vector2 center, Vector2 radius, Color color, double depth)
        {
            spriteBatch.Draw(circle, center, null, color, 0, new Vector2(circle.Width / 2, circle.Height / 2), new Vector2((float)(radius.X / ((float)circle.Width / 2)), (float)(radius.Y / ((float)circle.Height / 2))), SpriteEffects.None, (float)depth);
        }

        public void DrawDebugFont(string str, Vector2 position, float depth)
        {
            spriteBatch.DrawString(font, str, position, Color.Black,0,new Vector2(0),1, SpriteEffects.None, depth);
        }

        public void BeginWorld()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetWorldToScreenTransformation()); // greater on top
        }

        public void EndWorld()
        {
            spriteBatch.End();
        }

        public void Begin(Matrix matrix)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, matrix); // greater on top
        }

        public void End()
        {
            spriteBatch.End();
        }
    }
}
