using KinetX.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace KinetX.Physics
{
    public class PhysicsEngine
    {
        private readonly List<PhysicsObject> _objects = new();
        private readonly DispatcherTimer _timer;
        private const double FixedDeltaTime = 1.0 / 60.0; // 60 FPS

        public PhysicsEngine()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(FixedDeltaTime)
            };
            _timer.Tick += (s, e) => Update(FixedDeltaTime);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        public void AddObject(PhysicsObject obj) => _objects.Add(obj);
        public void ClearObjects() => _objects.Clear();

        private void Update(double deltaTime)
        {
            // Apply forces (gravity, etc.)
            foreach (var obj in _objects)
            {
                obj.Velocity.Y += 9.8 * deltaTime; // Gravity
            }

            // Detect and resolve collisions
            for (int i = 0; i < _objects.Count; i++)
            {
                for (int j = i + 1; j < _objects.Count; j++)
                {
                    var collision = CollisionDetector.CheckCollision(_objects[i], _objects[j]);
                    CollisionResolver.Resolve(collision);
                }
            }

            // Update positions
            foreach (var obj in _objects)
            {
                obj.Position += obj.Velocity * deltaTime;
                UpdateVisualPosition(obj);
            }
        }

        private void UpdateVisualPosition(PhysicsObject obj)
        {
            if (obj.Visual is not null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(obj.Visual, obj.Position.X);
                    Canvas.SetTop(obj.Visual, obj.Position.Y);
                });
            }
        }
    }
}