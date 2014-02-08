using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


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

        public static float ShortestAngleDistance(float a, float b)
        {
            a = RestrictAngle(a);
            b = RestrictAngle(b);

            return (float)Math.Min(Math.Abs(a - b), Math.Abs(Math.Abs(a - b) - 2 * Math.PI));
        }

        public static float AngleDistance(float a, float b)
        {
            return Math.Abs(RestrictAngle(b - a));
        }

        public static float MinimizeMagnitude(float angle)
        {
            float rAngle = RestrictAngle(angle);
            float rAngleSmall = (float)(rAngle-Math.PI*2);
            if(Math.Abs(rAngleSmall) < rAngle)
            {
                return rAngleSmall;
            }
            return rAngle;
        }

        public static float Lerp(float draw, float simulation, float smoothing)
        {
            if (smoothing >= 1)
            {
                return simulation;
            }
            if (smoothing <= 0)
            {
                return draw;
            }
            draw = Utils.Vector2Utils.RestrictAngle(draw);
            simulation = Utils.Vector2Utils.RestrictAngle(simulation);
            float difference = Utils.Vector2Utils.MinimizeMagnitude(simulation - draw);
            return draw + smoothing * difference; //MathHelper.Lerp(0, difference, smoothing);
        }
    } 
}
