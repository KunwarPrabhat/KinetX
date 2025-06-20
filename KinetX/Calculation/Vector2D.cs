

namespace KinetX.Calculation
{
    //This does all the vector calculations.
    public struct Vector2D
    {
        public double X;
        public double Y;

        public Vector2D(double x, double y) { X = x; Y = y; }

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(Vector2D a, double d) => new Vector2D(a.X * d, a.Y * d);
        public static Vector2D operator /(Vector2D a, double d) => new Vector2D(a.X / d, a.Y / d);
        public static Vector2D operator *(double d, Vector2D a) => new Vector2D(a.X * d, a.Y * d);

        public double MagnitudeSquared() => X * X + Y * Y;
        public double Magnitude() => Math.Sqrt(MagnitudeSquared());

        public Vector2D Normalized()
        {
            double mag = Magnitude();
            return mag == 0 ? new Vector2D(1, 0) : this / mag;
        }


        //I have made this function to return Normal so it can be used anywhere across the project.
        public static Vector2D Normal(Vector2D StartFrom, Vector2D EndTo)
        {
            Vector2D direction = EndTo - StartFrom;

            double length = direction.Magnitude();

            if (length == 0)
                return new Vector2D(1, 0);

            return direction / length;
            //Dividing a vector by its magnitude gives you the unit vector in that direction — a normalized direction vector.

        }
        public static double Dot(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }

}
