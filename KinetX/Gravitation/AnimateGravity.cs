using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using KinetX.Calculation;
using KinetX.Gravitation;
using static KinetX.Gravitation.Solver;

namespace KinetX.Gravitation
{
    public static class AnimateGravity
    {
        private static List<VerletObject> verletObjects = new List<VerletObject>();
        private static Canvas canvas;
        private static Solver solver;

        public static void GetInfo(Canvas targetCanvas)
        {
            canvas = targetCanvas;

            double diameter = 40;
            double offsetX = 240;
            Vector2D startPos = new Vector2D(
                canvas.ActualWidth / 2  - offsetX,
                canvas.ActualHeight / 2 
            );

            VerletObject obj = new VerletObject
            {
                position_current = startPos,
                position_old = startPos, 
                acceleration = new Vector2D(0, 0),
                Visual = CreateCircle(diameter, Brushes.Red)
            };

            canvas.Children.Clear();
            PaintCanvas.paint(canvas, ConstraintSettings.Radius);
            canvas.Children.Add(obj.Visual);

            Canvas.SetLeft(obj.Visual, obj.position_current.X - diameter); 
            Canvas.SetTop(obj.Visual, obj.position_current.Y - diameter);

            verletObjects.Clear();
            verletObjects.Add(obj);

            solver = new Solver(verletObjects, targetCanvas);
        }

        public static void Animate(object sender, EventArgs e)
        {
            int subSteps = 8;
            float dt = 1.0f / subSteps;

            for (int i = 0; i < subSteps; i++)
            {
                solver.Update(dt);
            }

            // Update visuals
            foreach (var obj in verletObjects)
            {
                Ellipse circle = (Ellipse)obj.Visual;
                double radius = circle.Width / 2.0;
                Canvas.SetLeft(circle, obj.position_current.X - radius);
                Canvas.SetTop(circle, obj.position_current.Y - radius);

            }
        }

        private static Ellipse CreateCircle(double diameter, Brush color)
        {
            return new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Fill = color
            };
        }
    }
}
