using System.Windows;

namespace KinetX.Models
{
    public enum ShapeType { Circle, Square, Triangle, Polygon }

    public class PhysicsObject
    {
        public ShapeType Shape { get; set; }
        public Vector2D Position { get; set; } = new Vector2D();
        public Vector2D Velocity { get; set; } = new Vector2D();
        public double Mass { get; set; }
        public double Restitution { get; set; }
        public double Rotation { get; set; }
        public double Radius { get; set; } // For circles
        public List<Vector2D> Vertices { get; set; } = new();
        public UIElement? Visual { get; set; } 
    }
}
