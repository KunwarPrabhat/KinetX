using KinetX.Calculation;

namespace KinetX.Objects
{
    public class Body
    {
        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; }
        public double Mass { get; set; }
        public double Restitution { get; set; } //(0 = inelastic, 1 = perfectly elastic){I always forget it lol}

        //public double InvMass => Mass == 0 ? 0 : 1.0 / Mass; //for future implementation on Baumgarte stabilization.


        public Body(Vector2D position, Vector2D velocity, double mass, double restitution)
        {
            Position = position;
            Velocity = velocity;
            Mass = mass;
            Restitution = restitution;
        }
    }
}
