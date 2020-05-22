using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
    public static class MathUtils
    {
        public static Vector2 RandomPointInSquare(float side)
        {
            return RandomPointInRectangle(side, side);
        }


        public static Vector2 RandomPointInRectangle(float x, float y)
        {
            return RandomPointInRectangle(new Vector2(x,y));
        }

        public static Vector2 RandomPointInRectangle(Vector2 range)
        {
            return new Vector2(Random.Range(-range.x,range.x), 
                               Random.Range(-range.x,range.y));
        }

        public static Vector2 RandomPointInOval(float x, float y)
        {
            return RandomPointInOval(new Vector2(x,y));
        }

        public static Vector2 RandomPointInOval(Vector2 radiuses)
        {
            return PointInOval(radiuses, Random.Range(0,360.0f));
        }

        public static Vector2 PointInCircle(float radius, float angleInDegrees)
        {
            return PointInOval(radius, radius, angleInDegrees);
        }

        public static Vector2 PointInOval(float radiusX, float radiusY, float angleInDegrees)
        {
            return PointInOval(new Vector2(radiusX, radiusY), angleInDegrees);
        }

        public static Vector2 PointInOval(Vector2 radiuses, float angleInDegrees)
        {
            float angleInRadians = Mathf.Deg2Rad + angleInDegrees;
            return new Vector2(radiuses.x * Mathf.Cos(angleInRadians),
                               radiuses.y * Mathf.Sin(angleInRadians));
        }
    }
}