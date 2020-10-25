using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
    public static class GeometryUtils
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
                               Random.Range(-range.y,range.y));
        }
        
        public static Vector2 RandomPointInBounds(Bounds bounds)
        {
            return (Vector2)bounds.center + 
                        RandomPointInRectangle((Vector2)bounds.extents);
        }
        public static Vector2 RandomPointInOval(float x, float y)
        {
            return RandomPointInOval(new Vector2(x,y));
        }

        public static Vector2 RandomPointInOval(Vector2 radiuses)
        {
            return PointInOval(radiuses, Random.Range(0,360.0f));
        }


        // Ratio in Arc 0->1
        public static float AngleInArc(float arcStartInDegrees, float arcEndInDegrees, float ratioInArc)
        {
            float angle = arcStartInDegrees + (arcEndInDegrees - arcStartInDegrees) * Mathf.Clamp01(ratioInArc);
            return angle;  
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
            float angleInRadians = Mathf.Deg2Rad * angleInDegrees;
            return new Vector2(radiuses.x * Mathf.Cos(angleInRadians),
                               radiuses.y * Mathf.Sin(angleInRadians));
        }

        public static Vector2 PointOutsideInvalidRange(Bounds validRange, 
                                                       Bounds invalidRange,
                                                       out bool found)
        {
            if(!validRange.Intersects(invalidRange))
            {
                found = true;
                return RandomPointInBounds(validRange);
            }
            float[] boundsX = {Mathf.Min(validRange.min.x, invalidRange.min.x),
                               Mathf.Max(validRange.min.x, invalidRange.min.x),
                               Mathf.Min(validRange.max.x, invalidRange.max.x),
                               Mathf.Max(validRange.max.x, invalidRange.max.x)};
            float[] boundsY = {Mathf.Min(validRange.min.y, invalidRange.min.y),
                               Mathf.Max(validRange.min.y, invalidRange.min.y),
                               Mathf.Min(validRange.max.y, invalidRange.max.y),
                               Mathf.Max(validRange.max.y, invalidRange.max.y)};

            List<Bounds> bounds = new List<Bounds>();
            for(int i = 0; i < boundsX.Length-1; i++)
            {
                for(int j = 0; j < boundsY.Length-1; j++)
                {
                    Vector2 min = new Vector2(boundsX[i], boundsY[j]);
                    Vector2 max = new Vector2(boundsX[i+1], boundsY[j+1]);
                    Vector2 center = (max-min)/2.0f;
                    if((max-min).magnitude > 0 
                        && !invalidRange.Contains(center) 
                        && validRange.Contains(center))
                    {
                        bounds.Add(new Bounds(center, max-min));
                        
                    }
                }
            }

            if(bounds.Count == 0)
            {
                found = false;
                return validRange.center;
            } 
            
            found = true;
            return RandomPointInBounds(bounds[Random.Range(0,bounds.Count)]);
        }
        
    }
}