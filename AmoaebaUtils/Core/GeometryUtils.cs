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

        public static Vector2[] PointsInCircle(float radius, int points)
        {
            if(points <= 0)
            {
                return new Vector2[0];
            }

            Vector2[] ret = new Vector2[points];
            for(int i = 0; i < points; i++)
            {
                float angle = (360.0f /(float)points) * i;
                ret[i] = PointInCircle(radius, angle);
            }

            return ret;
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
        
        public static Vector2Int NormalizedMaxValueVector(Vector2 dir, bool yOnTie = true)
        {
            if(dir.x == dir.y)
            {
                return Vector2Int.zero;
            }
        

            if(Mathf.Abs(dir.x) != Mathf.Abs(dir.y))
            {
                return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)? 
                                Mathf.RoundToInt(Mathf.Sign(dir.x)) * Vector2Int.right 
                                : Mathf.RoundToInt(Mathf.Sign(dir.y)) * Vector2Int.up;
            }

            return yOnTie? Mathf.RoundToInt(Mathf.Sign(dir.y)) * Vector2Int.up :
                           Mathf.RoundToInt(Mathf.Sign(dir.x)) * Vector2Int.right;
        }

        public static bool IsCircleCollision(float circle1X,
                                             float circle1Y,
                                             float circle1Radius,
                                             float circle2X,
                                             float circle2Y,
                                             float circle2Radius)
        {
            return IsCircleCollision(new Vector2(circle1X, circle1Y),
                                     circle1Radius,
                                     new Vector2(circle2X, circle2Y),
                                     circle2Radius);
        }

        public static bool IsCircleCollision(Vector2 pos1,
                                             float radius1,
                                             Vector2 pos2,
                                             float radius2)
        {
            return Vector2.Distance(pos1, pos2) <= radius1 + radius2;
        }

        public static bool IsCircleRectCollision(Bounds circleBounds, Bounds rectBounds) 
        {
            return IsCircleRectCollision(circleBounds.center.x, 
                                         circleBounds.center.y,
                                         circleBounds.size.x,
                                         rectBounds.center.x,
                                         rectBounds.center.y,
                                         rectBounds.size.x,
                                         rectBounds.size.y);
        }

        public static bool IsCircleRectCollision(float circleX, 
                                                 float circleY, 
                                                 float radius, 
                                                 float rectX, 
                                                 float rectY, 
                                                 float rectWidth, 
                                                 float rectHeight) 
        {

            // temporary variables to set edges for testing
            float testX = circleX;
            float testY = circleY;

            // which edge is closest?
            if (circleX < rectX)         testX = rectX;      // test left edge
            else if (circleX > rectX+rectWidth) testX = rectX+rectWidth;   // right edge
            if (circleY < rectY)         testY = rectY;      // top edge
            else if (circleY > rectY+rectHeight) testY = rectY+rectHeight;   // bottom edge

            // get distance from closest edges
            float distX = circleX-testX;
            float distY = circleY-testY;
            float distance = Mathf.Sqrt( (distX*distX) + (distY*distY) );

            // if the distance is less than the radius, collision!
            if (distance <= radius) 
            {
                return true;
            }
            return false;
        }
    }
}