using KinetX.Models;
using KinetX.Models.KinetX.Models;

namespace KinetX.Physics
{
    public static class CollisionDetector
    {
        public static CollisionResult CheckCollision(PhysicsObject a, PhysicsObject b)
        {
            if (a.Shape == ShapeType.Circle && b.Shape == ShapeType.Circle)
            {
                return CircleToCircle(a, b);
            }
            else if (a.Shape == ShapeType.Circle)
            {
                return CircleToPolygon(a, b);
            }
            else if (b.Shape == ShapeType.Circle)
            {
                return CircleToPolygon(b, a); // Note: flipped
            }
            else
            {
                return PolygonToPolygon(a, b);
            }
        }
        private static CollisionResult CircleToCircle(PhysicsObject a, PhysicsObject b)
        {
            var result = new CollisionResult { ObjectA = a, ObjectB = b };
            var diff = b.Position - a.Position;
            var distance = diff.Magnitude;
            var totalRadius = a.Radius + b.Radius;

            result.IsColliding = distance < totalRadius;
            if (result.IsColliding)
            {
                result.Depth = totalRadius - distance;
                result.Normal = diff.Normalized;
            }

            return result;
        }
        private static CollisionResult CircleToPolygon(PhysicsObject circle, PhysicsObject polygon)
        {
            var result = new CollisionResult { ObjectA = circle, ObjectB = polygon };
            double minOverlap = double.MaxValue;
            Vector2D smallestAxis = new(0, 0);

            foreach (var edge in GetEdges(polygon.Vertices))
            {
                var axis = edge.Normalized;
                ProjectVertices(axis, polygon.Vertices, out double minB, out double maxB);
                ProjectCircle(axis, circle.Position, circle.Radius, out double minA, out double maxA);

                double overlap = GetOverlap(minA, maxA, minB, maxB);
                if (overlap <= 0)
                    return result; // No collision

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    smallestAxis = axis;
                }
            }

            result.IsColliding = true;
            result.Depth = minOverlap;
            result.Normal = smallestAxis;
            return result;
        }
        private static CollisionResult PolygonToPolygon(PhysicsObject a, PhysicsObject b)
        {
            var result = new CollisionResult { ObjectA = a, ObjectB = b };
            double minOverlap = double.MaxValue;
            Vector2D smallestAxis = new(0, 0);

            var axes = GetEdges(a.Vertices).Concat(GetEdges(b.Vertices));

            foreach (var edge in axes)
            {
                var axis = edge.Normalized.Perpendicular();
                ProjectVertices(axis, a.Vertices, out double minA, out double maxA);
                ProjectVertices(axis, b.Vertices, out double minB, out double maxB);

                double overlap = GetOverlap(minA, maxA, minB, maxB);
                if (overlap <= 0)
                    return result; // No collision

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    smallestAxis = axis;
                }
            }

            result.IsColliding = true;
            result.Depth = minOverlap;
            result.Normal = smallestAxis;
            return result;
        }

        private static IEnumerable<Vector2D> GetEdges(List<Vector2D> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var p1 = vertices[i];
                var p2 = vertices[(i + 1) % vertices.Count];
                yield return p2 - p1;
            }
        }
        private static void ProjectVertices(Vector2D axis, List<Vector2D> vertices, out double min, out double max)
        {
            min = max = axis.Dot(vertices[0]);
            foreach (var v in vertices.Skip(1))
            {
                double proj = axis.Dot(v);
                min = Math.Min(min, proj);
                max = Math.Max(max, proj);
            }
        }
        private static void ProjectCircle(Vector2D axis, Vector2D center, double radius, out double min, out double max)
        {
            double centerProj = axis.Dot(center);
            min = centerProj - radius;
            max = centerProj + radius;
        }

        private static double GetOverlap(double minA, double maxA, double minB, double maxB)
        {
            return Math.Min(maxA, maxB) - Math.Max(minA, minB);
        }

        private static Vector2D Perpendicular(this Vector2D v) => new(-v.Y, v.X);
    }
}
