
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
               
                //if You're thinking why are we using squared values than normally comparing the distance between the circles to the sum of the radius
                //We are actually using a clever Optimization technique to minimize computation by doing Math.sqrt only when collision is happening.
                //Imagine it this way , we have 500 circles and only 100 of them is colliding at a frame so instead of doing Math.sqrt for 500 times 
                //we are just doing it for 100 circles and that saves a whole lot of computation.
                
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
                    //finding penetration depth here.
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
                    //and here.
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

                //contact point
                result.ContactPoint = new Vector2D(
                   Math.Max(topLeftA.X, topLeftB.X) + overlapX / 2,
                   Math.Max(topLeftA.Y, topLeftB.Y) + overlapY / 2
                );
            }
            return result;
        }
        public static double Clamp(double value, double min, double max)
        {

            if (value < min) return min;
            if (value > max) return max;
            return value;
        }


        public static CollisionResult CircleToAABBCollision(Vector2D center, double radius, Vector2D bottomLeft, Vector2D topRight)
        {
            var result = new CollisionResult();

            double closestX = Clamp(center.X, bottomLeft.X, topRight.X);
            double closestY = Clamp(center.Y, bottomLeft.Y, topRight.Y);

            double dx = center.X - closestX;
            double dy = center.Y - closestY;
            double distanceSquared = dx * dx + dy * dy;
            double radiusSquared = radius * radius;

            if (distanceSquared <= radiusSquared)
            {
                result.IsColliding = true;

                if (distanceSquared < 1e-6)
                {
                    double left = center.X - bottomLeft.X;
                    double right = topRight.X - center.X;
                    double bottom = center.Y - bottomLeft.Y;
                    double top = topRight.Y - center.Y;

                    double minDist = Math.Min(Math.Min(left, right), Math.Min(bottom, top));

                    if (minDist == left)
                    {
                        result.Normal = new Vector2D(-1, 0);
                        result.PenetrationDepth = radius + left;
                    }
                    else if (minDist == right)
                    {
                        result.Normal = new Vector2D(1, 0);
                        result.PenetrationDepth = radius + right;
                    }
                    else if (minDist == bottom)
                    {
                        result.Normal = new Vector2D(0, -1); 
                        result.PenetrationDepth = radius + bottom;
                    }
                    else // top
                    {
                        result.Normal = new Vector2D(0, 1); 
                        result.PenetrationDepth = radius + top;
                    }

                    result.ContactPoint = new Vector2D(closestX, closestY);
                }
                else
                {
                    double distance = Math.Sqrt(distanceSquared);
                    result.PenetrationDepth = radius - distance;

                    result.Normal = new Vector2D(dx / distance, dy / distance);
                    result.ContactPoint = new Vector2D(closestX, closestY);
                }
            }
            return result;
        }


    }
}
