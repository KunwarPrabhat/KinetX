

namespace KinetX.Models
{
    namespace KinetX.Models
    {
        public class CollisionResult
        {
            public bool IsColliding { get; set; }
            public Vector2D Normal { get; set; } = default!;
            public double Depth { get; set; }
            public PhysicsObject ObjectA { get; set; } = default!;
            public PhysicsObject ObjectB { get; set; } = default!;
        }
    }
}
