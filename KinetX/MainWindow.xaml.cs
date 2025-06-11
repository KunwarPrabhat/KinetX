using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinetX.Calculation;
using KinetX.Collision;
using KinetX.Objects;
using System.Printing;

namespace KinetX
{
    public partial class MainWindow : Window
    {
        private Stopwatch stopwatch = new Stopwatch();
        private List<(Shape visual, Body physics)> objects = new List<(Shape, Body)>();
        private bool isAnimating = false;
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Animate;
            CollideButton.Click += CollideButton_Click;

        }
        private void CollideButton_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Start();
            MainCanvas.Children.Clear();
            objects.Clear();

            //It reads shape types
            string shapeA = ((ComboBoxItem)ObjectAComboBox.SelectedItem).Content.ToString();
            string shapeB = ((ComboBoxItem)ObjectBComboBox.SelectedItem).Content.ToString();


            //It reads the Restitution of both the objects.
            double restitutionA = RestitutionASlider.Value;
            double restitutionB = RestitutionBSlider.Value;


            //It reads mass of both the objects
            double MassOfA = double.Parse(MassA.Text);
            double MassOfB = double.Parse(MassB.Text);

            //It reads velocities
            double ax = double.Parse(VelocityAX.Text);
            double ay = double.Parse(VelocityAY.Text);

            double bx = double.Parse(VelocityBX.Text);
            double by = double.Parse(VelocityBY.Text);

            //It reads object counts
            int ObjectACount = (int)ObjectACountSlider.Value;
            int ObjectBCount = (int)ObjectBCountSlider.Value;


            for (int i = 0; i < ObjectACount; i++)
            {
                //for creating shapes 
                var objA = CreateShape(shapeA);

                //Random start position
                RandomSpawn(objA, MainCanvas);

                //Adding it to canvas
                MainCanvas.Children.Add(objA);

                //we are storing references here which is crucial for animation.
                double centerX = Canvas.GetLeft(objA) + objA.Width / 2;
                double centerY = Canvas.GetTop(objA) + objA.Height / 2;

                var body = new Body(
                    new Vector2D(centerX, centerY), //this is the position of the physical object.
                    new Vector2D(ax, ay), //and this is the velocity of the physisal object.
                    MassOfA,
                    restitutionA
                );
                objects.Add((objA, body));
            }
            //same goes for Object B as well.
            for (int i = 0; i < ObjectBCount; i++)
            {
                var objB = CreateShape(shapeB);

                RandomSpawn(objB, MainCanvas);

                MainCanvas.Children.Add(objB);

                double centerX = Canvas.GetLeft(objB) + objB.Width / 2;
                double centerY = Canvas.GetTop(objB) + objB.Height / 2;

                var body = new Body(
                    new Vector2D(centerX, centerY), //this is the position of the physical object.
                    new Vector2D(ax, ay), //and this is the velocity of the physisal object.
                    MassOfB,
                    restitutionB
                );
                objects.Add((objB, body));
            }
            StartAnimation();
        }
        private void RandomSpawn(UIElement element, Canvas canvas)
        {
            Random rand = new Random();
            double x = rand.Next(0, (int)canvas.ActualWidth - 50);
            double y = rand.Next(0, (int)canvas.ActualHeight - 50);

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }
        private void StartAnimation()
        {
            if (!isAnimating)
            {
                isAnimating = true;
                CompositionTarget.Rendering += Animate;
            }
        }
        private void StopAnimation()
        {
            isAnimating = false;
            CompositionTarget.Rendering -= Animate;
        }
        private Shape CreateShape(string type)
        {
            Shape shape;

            if (type == "Circle")
            {
                shape = new Ellipse();
            }
            else
            {
                shape = new Rectangle();
            }

            shape.Width = 60;
            shape.Height = 60;
            Color randomColor = GenerateSoftColor();
            shape.Fill = new SolidColorBrush(randomColor);
            return shape;
        }

        //This method is responsible for generating random softer colours for the Objects A and B.
        private Color GenerateSoftColor()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // Base tone range: mid-tone colors with some vibrance
            byte r = (byte)rand.Next(80, 160);
            byte g = (byte)rand.Next(80, 160);
            byte b = (byte)rand.Next(80, 160);

            int boost = 40;
            int channel = rand.Next(3);
            switch (channel)
            {
                case 0: r = (byte)Math.Min(255, r + boost); break;
                case 1: g = (byte)Math.Min(255, g + boost); break;
                case 2: b = (byte)Math.Min(255, b + boost); break;
            }

            return Color.FromRgb(r, g, b);
        }
        //We have to do the following now.
        //  For each shape:
        //- Get current position
        //- Add velocity
        //- Check if it hits wall
        //- If yes, reverse velocity
        //- Set new position

        private void Animate(object sender, EventArgs e)
        {
            double rate = 5.5f;
            double deltaTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            if (!isAnimating) return;

            // First update all positions
            foreach (var (shape, body) in objects)
            {
                // Apply velocity to position
                body.Position += body.Velocity * deltaTime * rate;

                // Update visual position
                Canvas.SetLeft(shape, body.Position.X - shape.Width / 2);
                Canvas.SetTop(shape, body.Position.Y - shape.Height / 2);
            }

            // Then handle wall collisions
            foreach (var (shape, body) in objects)
            {
                double halfWidth = shape.Width / 2;
                double halfHeight = shape.Height / 2;

                double left = body.Position.X - halfWidth;
                double right = body.Position.X + halfWidth;
                double top = body.Position.Y - halfHeight;
                double bottom = body.Position.Y + halfHeight;

                // Left wall collision
                if (left < 0)
                {
                    body.Position = new Vector2D(halfWidth, body.Position.Y);
                    body.Velocity = new Vector2D(-body.Velocity.X * body.Restitution, body.Velocity.Y);
                    Canvas.SetLeft(shape, 0);
                }
                // Right wall collision
                else if (right > MainCanvas.ActualWidth)
                {
                    body.Position = new Vector2D(MainCanvas.ActualWidth - halfWidth, body.Position.Y);
                    body.Velocity = new Vector2D(-body.Velocity.X * body.Restitution, body.Velocity.Y);
                    Canvas.SetLeft(shape, MainCanvas.ActualWidth - shape.Width);
                }

                // Top wall collision
                if (top < 0)
                {
                    body.Position = new Vector2D(body.Position.X, halfHeight);
                    body.Velocity = new Vector2D(body.Velocity.X, -body.Velocity.Y * body.Restitution);
                    Canvas.SetTop(shape, 0);
                }
                // Bottom wall collision
                else if (bottom > MainCanvas.ActualHeight)
                {
                    body.Position = new Vector2D(body.Position.X, MainCanvas.ActualHeight - halfHeight);
                    body.Velocity = new Vector2D(body.Velocity.X, -body.Velocity.Y * body.Restitution);
                    Canvas.SetTop(shape, MainCanvas.ActualHeight - shape.Height);
                }
            }

            // Then handle object collisions
            for (int i = 0; i < objects.Count; i++)
            {
                var (shapeA, bodyA) = objects[i];
                for (int j = i + 1; j < objects.Count; j++)
                {
                    var (shapeB, bodyB) = objects[j];

                    if (shapeA is Ellipse && shapeB is Ellipse)
                    {
                        double radiusA = shapeA.Width / 2;
                        double radiusB = shapeB.Width / 2;

                        var result = CollisionDetector.CircleToCircle(bodyA.Position, radiusA, bodyB.Position, radiusB);

                        if (result.IsColliding)
                        {
                            ImpulseResolver.ResolveCollision(bodyA, bodyB, result);

                            // Small position correction
                            var fix = result.Normal * 0.5;
                            bodyA.Position -= fix;
                            bodyB.Position += fix;

                            // Update visual positions
                            Canvas.SetLeft(shapeA, bodyA.Position.X - radiusA);
                            Canvas.SetTop(shapeA, bodyA.Position.Y - radiusA);
                            Canvas.SetLeft(shapeB, bodyB.Position.X - radiusB);
                            Canvas.SetTop(shapeB, bodyB.Position.Y - radiusB);
                        }
                    }
                }
            }
        }
    }
}