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

namespace MyGame.Utils
{
    public class Vector2Utils
    {
        public static Vector2 ConstructVectorFromPolar(double magnitude, double angle)
        {
            return new Vector2((float)(magnitude * Math.Cos(angle)), (float)(magnitude * Math.Sin(angle)));
        }

        public static float Vector2Angle(Vector2 point)
        {
            if (point.X == 0)
                return RestrictAngle(Math.PI / 2 * (point.Y / Math.Abs(point.Y)));
            else if (point.X > 0)
                return RestrictAngle(Math.Atan(point.Y / point.X));
            else
                return RestrictAngle(Math.Atan(point.Y / point.X) + Math.PI);
        }

        public static Vector2 RotateVector2(Vector2 vector, double rotation)
        {
            return Vector2.Transform(vector, Matrix.CreateRotationZ((float)rotation));
        }

        public static float RestrictAngle(double angle)
        {
            while (angle > Math.PI * 2)
            {
                angle = angle - (float)Math.PI * 2;
            }

            while (angle < 0)
            {
                angle = angle + (float)Math.PI * 2;
            }

            return (float)angle;
        }

        public static float RestrictAngle(float angle)
        {
            while (angle > Math.PI * 2)
            {
                angle = angle - (float)Math.PI * 2;
            }

            while (angle < 0)
            {
                angle = angle + (float)Math.PI * 2;
            }

            return angle;
        }

        public static float AngleDistance(float a, float b)
        {
            a = RestrictAngle(a);
            b = RestrictAngle(b);

            return (float)Math.Min(Math.Abs(a - b), Math.Abs(Math.Abs(a - b) - 2 * Math.PI));
        }
    } 
}
