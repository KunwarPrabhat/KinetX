using KinetX.Calculation;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinetX.Gravitation
{
    public class Solver
    {
        private readonly Canvas canvas;
        private Vector2D gravity;
        private List<VerletObject> objects;

        public Solver(List<VerletObject> objects, Canvas canvas, double gravityStrength)
        {
            this.objects = objects;
            this.canvas = canvas;
            this.gravity = new Vector2D(0f, 50 * gravityStrength);
        }

        public void Update(float dt, double objSize)
        {
            ApplyGravity();
            ApplyConstraint(objSize);
            SolveCollisions(objSize);
            UpdatePositions(dt);
        }

        public void UpdatePositions(float dt)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                obj.UpdatePosition(dt);
                objects[i] = obj; 
            }
        }

        private void ApplyGravity()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                obj.Accelerate(gravity);
                objects[i] = obj; // again, struct = value type
            }
        }
        public static class ConstraintSettings
        {
            public const double Radius = 350;
        }
        public void ApplyConstraint(double Radius)
        {
            double centerX = canvas.ActualWidth / 2.0;
            double centerY = canvas.ActualHeight / 2.0;
            double radius = ConstraintSettings.Radius;
            double objectRadius = Radius;
            Vector2D position = new Vector2D(centerX, centerY);

            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                Vector2D center_to_obj = obj.position_current - position;
                float dist = (float)center_to_obj.Magnitude();

                if (dist > radius - objectRadius)
                {
                    Vector2D n = center_to_obj / dist; // normalize
                    obj.position_current = position + n * (radius - objectRadius);

                }

                objects[i] = obj;
            }
        }
        //public void SolveCollisions()
        //{
        //    float radius = 20f;
        //    float combinedRadius = 2 * radius;

        //    int objectCount = objects.Count;

        //    for (int i = 0; i < objectCount; i++)
        //    {
        //        var object1 = objects[i];

        //        for (int k = i + 1; k < objectCount; k++)
        //        {
        //            var object2 = objects[k];

        //            Vector2D collisionAxis = object1.position_current - object2.position_current;
        //            double dist = collisionAxis.Magnitude();

        //            if (dist < combinedRadius)
        //            {
        //                Vector2D n = collisionAxis / dist; // normalize
        //                double delta = combinedRadius - dist;

        //                object1.position_current += 0.5f * delta * n;
        //                object2.position_current -= 0.5f * delta * n;

        //                objects[i] = object1;
        //                objects[k] = object2;
        //            }
        //        }
        //    }
        //}


        //The above function has O(n²) time complexity, which is brutal for our cpu if the objects are 200 + 
        // meaning it is doing roughly 20k + collision checks per frame and min of 1200k+ collision checks per second at 60fps.

        //So we are using a technique called Spatial Partitioning to reduce the time complexity to O(n) or O(k.n).
        // aslo known as 2D spatial hashing collision detection.
        public void SolveCollisions(double Radius)
        {
            double radius = Radius;
            double combinedRadius = 2 * radius;
            double cellSize = combinedRadius;

            Dictionary<(int, int), List<int>> spatialGrid = new Dictionary<(int, int), List<int>>();

            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                int cellX = (int)(obj.position_current.X / cellSize);
                int cellY = (int)(obj.position_current.Y / cellSize);
                var key = (cellX, cellY);

                if (!spatialGrid.ContainsKey(key))
                    spatialGrid[key] = new List<int>();

                spatialGrid[key].Add(i);
            }

            foreach (var kvp in spatialGrid)
            {
                var (cx, cy) = kvp.Key;
                var indices = kvp.Value;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        var neighborKey = (cx + dx, cy + dy);
                        if (!spatialGrid.ContainsKey(neighborKey)) continue;

                        var neighborIndices = spatialGrid[neighborKey];

                        foreach (int i in indices)
                        {
                            var obj1 = objects[i];

                            foreach (int j in neighborIndices)
                            {
                                if (j <= i) continue; 

                                var obj2 = objects[j];
                                Vector2D axis = obj1.position_current - obj2.position_current;
                                double dist = axis.Magnitude();

                                if (dist < combinedRadius && dist > 0.0001)
                                {
                                    Vector2D n = axis / dist;
                                    double delta = combinedRadius - dist;

                                    obj1.position_current += 0.5f * delta * n;
                                    obj2.position_current -= 0.5f * delta * n;

                                    objects[i] = obj1;
                                    objects[j] = obj2;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
