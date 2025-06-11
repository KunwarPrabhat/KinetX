using KinetX.Calculation;
using KinetX.Objects;

namespace KinetX.Collision
{
    public static class ImpulseResolver //this is a headache.
    {
        public static void ResolveCollision(Body a, Body b, CollisionResult result) 
            //Body a and b contains all the information like velocity, mass restitution etc.
            //Also the Collision result gives us these - IsColliding, Normal, PenetrationDepth & ContactPoint
        {
            if (!result.IsColliding)
                return;

            //I am commenting each steps for future understanding, I am clearly not a memory capsule lol.

            // 1. we are calculating relative velocity
            Vector2D relativeVelocity = b.Velocity - a.Velocity;

            // 2. we are calculating velocity along the collision normal
            double velocityAlongNormal = Vector2D.Dot(relativeVelocity, result.Normal);

            // 3. If objects are moving away, no need to resolve
            if (velocityAlongNormal > 0)
                return;

            // 4. Finally calculating the restitution.
            double restitution = Math.Min(a.Restitution, b.Restitution);

            // 5. Calculating the impulse scalar (basically magnitude)
            double impulseMagnitude = -(1 + restitution) * velocityAlongNormal;
            impulseMagnitude /= (1 / a.Mass) + (1 / b.Mass); //don't ask where it came from [Pick up your physics notes]

            // 6. And finally apply impulse force to both bodies
            Vector2D impulse = result.Normal * impulseMagnitude;

            a.Velocity -= impulse * (1 / a.Mass);
            b.Velocity += impulse * (1 / b.Mass);
        }
    }
}
