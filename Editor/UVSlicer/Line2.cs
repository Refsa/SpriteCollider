
using UnityEngine;

namespace Refsa.UVSlicer
{
    public struct Line2
    {
        public Vector2 Point1;
        public Vector2 Point2;

        public Line2(Vector2 p1, Vector2 p2)
        {
            Point1 = p1;
            Point2 = p2;
        }

        public float DistanceToPoint(Vector2 point)
        {
            float vAPx = point.x - Point1.x;
            float vAPy = point.y - Point1.y;
            float vABx = Point2.x - Point1.x;
            float vABy = Point2.y - Point1.y;

            float sqDistanceAB = (Point2 - Point1).sqrMagnitude;

            float ABAPproduct = (vABx * vAPx) + (vABy * vAPy);
            float amount = ABAPproduct / sqDistanceAB;

            if (amount < 0f) amount = 0f;
            else if (amount > 1f) amount = 1f;

            float nx = (amount * (Point2.x - Point1.x)) + Point1.x;
            float ny = (amount * (Point2.y - Point1.y)) + Point1.y;

            return Vector2.Distance(point, new Vector2(nx, ny));
        }
    }
}
