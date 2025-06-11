
using KinetX.Calculation;

namespace KinetX.Collision
{
    //This class holds all the info about a collision, whether it happened or not, where and how deep.
    public class CollisionResult
    {
        public bool IsColliding { get; set; } = false;
        public Vector2D Normal { get; set; } = new Vector2D(0, 0);
        public double PenetrationDepth { get; set; } = 0;
        public Vector2D ContactPoint { get; set; } = new Vector2D(0, 0);

        // Default constructor: initializes all properties to their default values.
        public CollisionResult() { }


        public CollisionResult(bool isColliding, Vector2D normal, double depth, Vector2D contact)
        {
            IsColliding = isColliding;
            Normal = normal;
            PenetrationDepth = depth;
            ContactPoint = contact;
        }
    }


}
