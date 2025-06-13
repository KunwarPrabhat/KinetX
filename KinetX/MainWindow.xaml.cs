#pragma warning disable CS8632 //I was fed up with the warnings.
#nullable disable
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinetX.Calculation;
using KinetX.Collision;
using KinetX.Objects;

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
        }
        private void CollideButton_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Start();
            MainCanvas.Children.Clear();
            objects.Clear();

            //To get the shape from the combobox.
            ComboBoxItem? itemA = ObjectAComboBox.SelectedItem as ComboBoxItem;
            ComboBoxItem? itemB = ObjectBComboBox.SelectedItem as ComboBoxItem;

            //Handling Null execption.
            if (itemA?.Content is not string shapeA || itemB?.Content is not string shapeB)
            //A fashionable way to  cast itemA.Content to a string and, if successful, assigns it to shapeA & same to shapeB (C#9 things):
            {
                MessageBox.Show("Please select both shapes before continuing.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //this is throwing null reference warning so we are using diff method to get the shape up above.
            //string shapeA = ((ComboBoxItem)ObjectAComboBox.SelectedItem!).Content.ToString();
            //string shapeB = ((ComboBoxItem)ObjectBComboBox.SelectedItem!).Content.ToString(); 
            //string shapeA = itemA.Content.ToString();
            //string shapeB = itemB.Content.ToString();


            //It reads the Restitution of both the objects.
            double restitutionA = RestitutionASlider.Value;
            double restitutionB = RestitutionBSlider.Value;


            //It reads velocities, if The entered value is other than number the if block becomes true and throws a message.
            if (!double.TryParse(VelocityAX.Text, out double ax) ||
                !double.TryParse(VelocityAY.Text, out double ay) ||
                !double.TryParse(VelocityBX.Text, out double bx) ||
                !double.TryParse(VelocityBY.Text, out double by))
            {
                MessageBox.Show("Please enter valid numeric values for all velocity fields.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //It reads mass of both the objects
            if (!double.TryParse(MassA.Text, out double MassOfA) ||
               !double.TryParse(MassB.Text, out double MassOfB))
            {
                MessageBox.Show("Please enter valid numeric values for the Mass fields.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //It reads object counts
            int ObjectACount = (int)ObjectACountSlider.Value;
            int ObjectBCount = (int)ObjectBCountSlider.Value;

            //It will read the scaling factor
            double ScaleA = ScaleASlider.Value;
            double ScaleB = ScaleBSlider.Value;



            for (int i = 0; i < ObjectACount; i++)
            {
                //for creating shapes 
                var objA = CreateShape(shapeA, ScaleA);

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
                var objB = CreateShape(shapeB, ScaleB);

                RandomSpawn(objB, MainCanvas);

                MainCanvas.Children.Add(objB);

                double centerX = Canvas.GetLeft(objB) + objB.Width / 2;
                double centerY = Canvas.GetTop(objB) + objB.Height / 2;

                var body = new Body(
                    new Vector2D(centerX, centerY), //this is the position of the physical object.
                    new Vector2D(bx, by), //and this is the velocity of the physisal object.
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
        private Shape CreateShape(string type, double ScalingFactor)
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

            shape.Width = 60*ScalingFactor;
            shape.Height = 60*ScalingFactor;
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
                    if (shapeA is Rectangle && shapeB is Rectangle)
                    {
                        // Get the top-left position directly from the canvas
                        Vector2D positionA = new Vector2D(Canvas.GetLeft(shapeA), Canvas.GetTop(shapeA));
                        Vector2D positionB = new Vector2D(Canvas.GetLeft(shapeB), Canvas.GetTop(shapeB));

                        Vector2D sizeA = new Vector2D(shapeA.Width, shapeA.Height);
                        Vector2D sizeB = new Vector2D(shapeB.Width, shapeB.Height);

                        var result = CollisionDetector.AABBCollision(positionA, sizeA, positionB, sizeB);

                        if (result.IsColliding)
                        {
                            // Resolve collision using impulse resolver
                            ImpulseResolver.ResolveCollision(bodyA, bodyB, result);

                            // Correct positions to prevent sticking
                            var fix = result.Normal * result.PenetrationDepth * 0.5; // Use penetration depth for accurate correction
                            bodyA.Position -= fix;
                            bodyB.Position += fix;

                            // Update visual positions based on corrected body positions
                            Canvas.SetLeft(shapeA, bodyA.Position.X - shapeA.Width / 2);
                            Canvas.SetTop(shapeA, bodyA.Position.Y - shapeA.Height / 2);
                            Canvas.SetLeft(shapeB, bodyB.Position.X - shapeB.Width / 2);
                            Canvas.SetTop(shapeB, bodyB.Position.Y - shapeB.Height / 2);
                        }
                    }
                }
            }
        }
    }
}