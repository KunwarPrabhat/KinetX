using KinetX.Calculation;

namespace KinetX.Objects
{
    public class Body
    {
        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; }
        public double Mass { get; set; }
        public double Restitution { get; set; } //(0 = inelastic, 1 = perfectly elastic){I always forget it lol}

        public Body(Vector2D position, Vector2D velocity, double mass, double restitution)
        {
            Position = position;
            Velocity = velocity;
            Mass = mass;
            Restitution = restitution;
        }
    }
}
