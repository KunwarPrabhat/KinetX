using KinetX.Models;
using KinetX.Physics;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
namespace KinetX.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly PhysicsEngine _physicsEngine;
        private readonly Canvas _canvas;

        public ObservableCollection<string> ShapeTypes { get; } = new()
        {
            "Circle", "Square", "Triangle", "Polygon"
        };

        public MainWindowViewModel(Canvas canvas)
        {
            _canvas = canvas;
            _physicsEngine = new PhysicsEngine();
            _physicsEngine.Start();
        }

        public void StartCollisionSimulation(
            string objectAType, string objectBType,
            int countA, int countB,
            double restitutionA, double restitutionB,
            double velocityAX, double velocityAY,
            double velocityBX, double velocityBY,
            double massA, double massB)
        {
            _physicsEngine.ClearObjects();
            _canvas.Children.Clear();

            // Create Object A instances
            for (int i = 0; i < countA; i++)
            {
                var obj = CreatePhysicsObject(objectAType, restitutionA, massA);
                obj.Velocity = new Vector2D(velocityAX, velocityAY);
                obj.Position = new Vector2D(100 + i * 50, 100);
                AddObjectToSimulation(obj);
            }

            // Create Object B instances
            for (int i = 0; i < countB; i++)
            {
                var obj = CreatePhysicsObject(objectBType, restitutionB, massB);
                obj.Velocity = new Vector2D(velocityBX, velocityBY);
                obj.Position = new Vector2D(400 + i * 50, 100);
                AddObjectToSimulation(obj);
            }
        }

        private PhysicsObject CreatePhysicsObject(string type, double restitution, double mass)
        {
            var obj = new PhysicsObject
            {
                Mass = mass,
                Restitution = restitution,
                Shape = Enum.Parse<ShapeType>(type)
            };

            switch (obj.Shape)
            {
                case ShapeType.Circle:
                    obj.Radius = 20;
                    obj.Visual = new Ellipse
                    {
                        Width = obj.Radius * 2,
                        Height = obj.Radius * 2,
                        Fill = Brushes.Blue,
                        Stroke = Brushes.DarkBlue,
                        StrokeThickness = 2
                    };
                    break;
                case ShapeType.Square:
                    obj.Visual = new Rectangle
                    {
                        Width = 40,
                        Height = 40,
                        Fill = Brushes.Red,
                        Stroke = Brushes.DarkRed,
                        StrokeThickness = 2
                    };
                    // Set vertices for collision detection
                    obj.Vertices = new List<Vector2D>
                    {
                        new(-20, -20), new(20, -20),
                        new(20, 20), new(-20, 20)
                    };
                    break;
                    // Add cases for Triangle and Polygon
            }

            return obj;
        }

        private void AddObjectToSimulation(PhysicsObject obj)
        {
            _canvas.Children.Add(obj.Visual);
            _physicsEngine.AddObject(obj);
        }
    }
}