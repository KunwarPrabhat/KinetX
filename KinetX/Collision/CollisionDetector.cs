
using KinetX.Calculation;

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
    }
}
