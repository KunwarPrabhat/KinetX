#pragma warning disable CS8632 
#nullable disable
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using KinetX.Calculation;
using KinetX.Gravitation;
using static KinetX.Gravitation.Solver;
using System.Diagnostics;
using System.Windows;

namespace KinetX.Gravitation
{
    public static class AnimateGravity
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static double lastSpawnTime = 0;
        //private static double spawnInterval = 0.1;

        private static double diameter;
        private static double spawnInterval;
        private static double Gravity;
        private static double objSize;

        private static int totalBallsToSpawn = 244; 
        //private static double diameter = 40;

        private static List<VerletObject> verletObjects = new List<VerletObject>();
        private static List<VerletObject> spawnQueue = new List<VerletObject>();

        private static Canvas canvas;
        private static Solver solver;

        public static void GetInfo(Canvas targetCanvas, double numberOfBalls, double ballSize, double spawnTime, double gravityStrength, double SpeedMultiplier)
        {
            canvas = targetCanvas;
            stopwatch.Restart();
            lastSpawnTime = 0;
            objSize = ballSize;


            diameter = ballSize*2;
            spawnInterval = spawnTime/SpeedMultiplier;
            Gravity = gravityStrength;


            spawnQueue.Clear();
            verletObjects.Clear();
            double offsetX = 240;
            Vector2D startPos = new Vector2D(
                canvas.ActualWidth / 2 - offsetX,
                canvas.ActualHeight / 2
            );

            for (int i = 0; i < numberOfBalls; i++)
            {
                VerletObject obj = new VerletObject
                {
                    position_current = startPos,
                    position_old = startPos,
                    acceleration = new Vector2D(0, 0),
                    Visual = CreateCircle(diameter)
                };
                spawnQueue.Add(obj);
            }

            canvas.Children.Clear();
            PaintCanvas.paint(canvas, ConstraintSettings.Radius);

            solver = new Solver(verletObjects, canvas, gravityStrength);
        }

        public static void Animate(object sender, EventArgs e)
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;

            // Spawn based on time
            if (spawnQueue.Count > 0 && (currentTime - lastSpawnTime) >= spawnInterval)
            {
                var obj = spawnQueue[0];
                spawnQueue.RemoveAt(0);

                verletObjects.Add(obj);
                canvas.Children.Add(obj.Visual);
                Canvas.SetLeft(obj.Visual, obj.position_current.X - diameter / 2);
                Canvas.SetTop(obj.Visual, obj.position_current.Y - diameter / 2);

                lastSpawnTime = currentTime;
            }

            // Physics update
            int subSteps = 8;
            float dt = 1.0f / 60f / subSteps;

            for (int i = 0; i < subSteps; i++)
            {
                solver.Update(dt, objSize);

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
        public static void StopAnimation()
        {
            CompositionTarget.Rendering -= Animate;

        }
        private static Ellipse CreateCircle(double diameter)
        {
            Color baseColor1 = GenerateSoftColor();
            Color baseColor2 = GenerateSoftColor();

            var gradient = new RadialGradientBrush
            {
                RadiusX = 10,
                RadiusY = 10,
                Center = new System.Windows.Point(0.5, 0.5),
                GradientOrigin = new System.Windows.Point(0.5, 0.5)
            };

            gradient.GradientStops.Add(new GradientStop(baseColor1, 0.0));
            gradient.GradientStops.Add(new GradientStop(baseColor2, 1.0));

            return new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Fill = gradient,
            };
        }
        private static Color GenerateSoftColor()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // Shifted base range: slightly more saturated
            byte r = (byte)rand.Next(200, 240);
            byte g = (byte)rand.Next(100, 150);
            byte b = (byte)rand.Next(80, 180);

            // Slightly stronger boost
            int boost = 50;
            int channel = rand.Next(3);
            switch (channel)
            {
                case 0: r = (byte)Math.Min(255, r + boost); break;
                case 1: g = (byte)Math.Min(255, g + boost); break;
                case 2: b = (byte)Math.Min(255, b + boost); break;
            }

            return Color.FromRgb(r, g, b);
        }
        public static void ClearBalls()
        {
            foreach (var obj in verletObjects)
            {
                if (obj.Visual is UIElement element && canvas.Children.Contains(element))
                {
                    canvas.Children.Remove(element);
                }
            }
            verletObjects.Clear();
        }


    }
}
