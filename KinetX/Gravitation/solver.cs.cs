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
        private Vector2D gravity = new Vector2D(0f, 1f);
        private List<VerletObject> objects;

        public Solver(List<VerletObject> objects, Canvas canvas)
        {
            this.objects = objects;
            this.canvas = canvas;
        }

        public void Update(float dt)
        {
            ApplyGravity();
            ApplyConstraint();
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
        public void ApplyConstraint()
        {
            double centerX = canvas.ActualWidth / 2.0;
            double centerY = canvas.ActualHeight / 2.0;
            double radius = ConstraintSettings.Radius;
            double objectRadius = 20;
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
    }
}
