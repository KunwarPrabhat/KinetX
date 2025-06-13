
using KinetX.Calculation;
using System.Diagnostics;

namespace KinetX.Collision
{
    public static class CollisionDetector
    {
        //This is a Circle To Circle Collision detection method.
        public static CollisionResult CircleToCircle(Vector2D centerA, double radiusA, Vector2D centerB, double radiusB)
        {
            var result  = new CollisionResult();

            Vector2D VectorDifference = centerA - centerB;
            double SqDistance = VectorDifference.MagnitudeSquared();

            double RadiiSum = radiusA + radiusB;

            //checking for collision
            if(SqDistance < RadiiSum * RadiiSum )
            {
                result.IsColliding = true;
                double DistanceBetweenCentres = Math.Sqrt(SqDistance); //Actual distance between centers.

                result.PenetrationDepth = RadiiSum - DistanceBetweenCentres; //to find penetration depth.

                result.Normal = Vector2D.Normal(centerA, centerB);

                result.ContactPoint = centerA + result.Normal * radiusA;
            }
            return result;
        }
        public static CollisionResult AABBCollision(Vector2D topLeftA, Vector2D sizeA, Vector2D topLeftB, Vector2D sizeB)
        {
            var result = new CollisionResult();

            double rightA = topLeftA.X + sizeA.X;
            double bottomA = topLeftA.Y + sizeA.Y;
            double rightB = topLeftB.X + sizeB.X;
            double bottomB = topLeftB.Y + sizeB.Y;

            bool colliding = !(rightA < topLeftB.X || topLeftA.X > rightB || bottomA < topLeftB.Y || topLeftA.Y > bottomB);

            if (colliding)
            {
                result.IsColliding = true;

                //for finding penetration depth.
                double overlapX = Math.Min(rightA, rightB) - Math.Max(topLeftA.X, topLeftB.X);
                double overlapY = Math.Min(bottomA, bottomB) - Math.Max(topLeftA.Y, topLeftB.Y);


                //for finding the normal.
                Vector2D normal = new Vector2D(0,0);
                if (overlapX < overlapY)
                {
                    result.PenetrationDepth = overlapX;

                    if (topLeftA.X < topLeftB.X)
                    {
                        normal = new Vector2D(1, 0);
                    }
                    else
                    {
                        normal = new Vector2D(-1, 0);
                    }
                }
                else
                {
                    result.PenetrationDepth = overlapY;
                    // Collision is more on the Y-axis (top or bottom)
                    if (topLeftA.Y < topLeftB.Y)
                    {
                        normal = new Vector2D(0, 1);
                    }
                    else
                    {
                        normal = new Vector2D(0, -1);
                    }
                }
                result.Normal = normal;

                result.ContactPoint = new Vector2D(
                   Math.Max(topLeftA.X, topLeftB.X) + overlapX / 2,
                   Math.Max(topLeftA.Y, topLeftB.Y) + overlapY / 2
                );
            }
            return result;
        }
    }
}
