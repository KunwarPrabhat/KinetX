

namespace KinetX.Models
{
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Vector2D()
        {
            X = 0;
            Y = 0;
        }
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(Vector2D a, double scalar) => new(a.X * scalar, a.Y * scalar);

        public double Magnitude => Math.Sqrt(X * X + Y * Y);

        //This gives you a unit vector (same direction, length = 1). If magnitude is 0 (zero vector), it returns (0,0).
        public Vector2D Normalized => Magnitude > 0? this * (1 / Magnitude) : new Vector2D(0, 0);
        public double Dot(Vector2D other) => X * other.X + Y * other.Y;
    }
}
