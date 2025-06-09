using KinetX.Models.KinetX.Models;

namespace KinetX.Physics
{
    public static class CollisionResolver
    {
        public static void Resolve(CollisionResult collision)
        {
            if (!collision.IsColliding) return;

            var a = collision.ObjectA;
            var b = collision.ObjectB;
            var normal = collision.Normal;

            // Calculate relative velocity
            var relativeVelocity = b.Velocity - a.Velocity;
            var velocityAlongNormal = relativeVelocity.Dot(normal);

            // Do not resolve if objects are separating
            if (velocityAlongNormal > 0) return;

            // Calculate restitution
            var restitution = Math.Min(a.Restitution, b.Restitution);

            // Calculate impulse scalar
            var totalMass = a.Mass + b.Mass;
            var impulseScalar = -(1 + restitution) * velocityAlongNormal / totalMass;

            // Apply impulse
            var impulse = normal * impulseScalar;
            a.Velocity -= impulse * a.Mass;
            b.Velocity += impulse * b.Mass;

            // Positional correction to prevent sinking
            const double percent = 0.2;
            const double slop = 0.01;
            var correction = normal * (Math.Max(collision.Depth - slop, 0) / totalMass * percent);
            a.Position -= correction * a.Mass;
            b.Position += correction * b.Mass;
        }
    }
}
