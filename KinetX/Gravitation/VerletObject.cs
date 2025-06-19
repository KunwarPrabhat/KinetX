using KinetX.Calculation;
using System.Windows;

namespace KinetX.Gravitation
{

    public struct VerletObject
    {
        public UIElement Visual;
        public Vector2D position_current;
        public Vector2D position_old;
        public Vector2D acceleration;
        public void UpdatePosition(double dt)
        {
            Vector2D velocity = position_current - position_old;
            Vector2D new_position = position_current + velocity + acceleration * dt * dt;
            position_old = position_current;
            position_current = new_position;

            acceleration = new Vector2D(0, 0);
        }

        public void Accelerate(Vector2D gravity)
        {
            acceleration += gravity;
        }

    }
}
