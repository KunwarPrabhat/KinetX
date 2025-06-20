#pragma warning disable CS8632 
#nullable disable
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinetX.Calculation;
using KinetX.Collision;
using KinetX.Objects;
using KinetX.Gravitation;
    
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
            this.Loaded += MainWindow_Loaded;
            CompositionTarget.Rendering += Animate;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MaxAllowedText.Text = $"Max allowed: {maxAllowedBalls}";
        }


        private void ShowPanel(Border panelToShow)
        {
            foreach (var child in SettingsContainer.Children)
            {
                if (child is Border border)
                {
                    if (border != panelToShow)
                        border.Visibility = Visibility.Collapsed;
                }
            }

            panelToShow.Visibility = Visibility.Visible;

        }

        private const double ConstraintArea = 384845; //Calculated using π × 350² where radius is fixed 350.
        private int maxAllowedBalls = 230; //global declaration.

        private void BallSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int radius = (int)BallSizeSlider.Value;
            int roughArea = (int)(ConstraintArea / (Math.PI * radius * radius));

            double Efficiency = 0.84;
            maxAllowedBalls = (int)(roughArea * Efficiency);
            ValidateBallCount();
        }
        private void BallCountInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateBallCount();
        }
        private void ValidateBallCount()
        {
            if (BallCountInput == null || MaxAllowedText == null)
                return;
            if (int.TryParse(BallCountInput.Text, out int userCount))
            {
                if (userCount > maxAllowedBalls)
                {
                    BallCountInput.BorderBrush = Brushes.Red;
                    MaxAllowedText.Text = $"⚠️ Max allowed: {maxAllowedBalls}. Reduce or your CPU will fry.";
                }
                else
                {
                    BallCountInput.BorderBrush = Brushes.DeepSkyBlue;
                    MaxAllowedText.Text = $"Max allowed: {maxAllowedBalls}";
                }
            }
            else
            {
                BallCountInput.BorderBrush = Brushes.OrangeRed;
                MaxAllowedText.Text = "⚠️ Invalid input!";
            }
        }


        private void Gravity_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();
            ShowPanel(GravitySettingsBorder);
            MainCanvas.Children.Clear();
            objects.Clear();

            PaintCanvas.paint(MainCanvas, 350);
        }
        private void GravityButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(BallCountInput.Text, out double NumberOfBall))
            {
                MessageBox.Show("Please enter valid numeric values for the Count field.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(NumberOfBall > maxAllowedBalls)
            {
                MessageBox.Show("Please enter valid numeric values for the Count field.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double SpeedAnimation = SpeedSlider.Value;

            double BallSize = BallSizeSlider.Value;

            double SpawnTime = SpawnTimeSlider.Value;

            double GravityStrength = GravityStrengthSlider.Value;

            CompositionTarget.Rendering -= AnimateGravity.Animate;
            AnimateGravity.GetInfo(MainCanvas, NumberOfBall, BallSize, SpawnTime, GravityStrength, SpeedAnimation);
            CompositionTarget.Rendering += AnimateGravity.Animate;

        }

        private void BreakAnimationButton_Click(Object sender, RoutedEventArgs e)
        {
            AnimateGravity.StopAnimation();
            AnimateGravity.ClearBalls();
        }

        private void Collision_Click(object sender, RoutedEventArgs e)
        {
            AnimateGravity.StopAnimation();
            ShowPanel(CollisionSettingsBorder);
            MainCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0b2545"));
            MainCanvas.Children.Clear();
            objects.Clear();

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

        private void Animate(object sender, EventArgs e)
        {
            double deltaTime = stopwatch.Elapsed.TotalSeconds;

            //Adding something experimental
            int substeps = 8;
            double stepDeltaTime = stopwatch.Elapsed.TotalSeconds / substeps;
            stopwatch.Restart();

            if (!isAnimating) return;

            //experimental line, update - Substepping actually worked and now the simulations are more precise.
            for (int step = 0; step < substeps; step++)
            {
                //if You're worried about the time complexity then no it's not O(n³) it's O(n² + 2n) basically same O(n²).

                foreach (var (shape, body) in objects)
                {
                    // Apply velocity to position
                    body.Position += body.Velocity * deltaTime;

                    // Update visual position - kicked this outside the function because now don't need to paint the canvas 8 times
                    // just doing once after the calculation is enough.
                    //Canvas.SetLeft(shape, body.Position.X - shape.Width / 2);
                    //Canvas.SetTop(shape, body.Position.Y - shape.Height / 2);
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
                                var fix = result.Normal * result.PenetrationDepth * 0.5;
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
                            Vector2D positionA = bodyA.Position - new Vector2D(shapeA.Width / 2, shapeA.Height / 2);
                            Vector2D positionB = bodyB.Position - new Vector2D(shapeB.Width / 2, shapeB.Height / 2);


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
                        if (shapeA is Ellipse && shapeB is Rectangle || shapeA is Rectangle && shapeB is Ellipse)
                        {
                            if (shapeA is Ellipse && shapeB is Rectangle)
                            {
                                double radiusA = shapeA.Width / 2;

                                //bodyB.Position is the center of the rectangle.
                                double halfWidth = shapeB.Width / 2;
                                double halfHeight = shapeB.Height / 2;

                                Vector2D bottomLeft = new Vector2D(
                                    bodyB.Position.X - halfWidth,
                                    bodyB.Position.Y - halfHeight
                                );
                                Vector2D topRight = new Vector2D(
                                    bodyB.Position.X + halfWidth,
                                    bodyB.Position.Y + halfHeight
                                );

                                var result = CollisionDetector.CircleToAABBCollision(bodyA.Position, radiusA, bottomLeft, topRight);

                                if (result.IsColliding)
                                {
                                    result.Normal = new Vector2D(-result.Normal.X, -result.Normal.Y);

                                    double correctionPercent = 0.4;
                                    double slop = 0.02;

                                    double correctionMagnitude = Math.Max(result.PenetrationDepth - slop, 0.0) * correctionPercent;
                                    Vector2D correction = result.Normal * correctionMagnitude;

                                    bodyA.Position -= correction * 0.5;  // Circle moves opposite to normal (away from rectangle)
                                    bodyB.Position += correction * 0.5;  // Rectangle moves along normal (away from circle)

                                    ImpulseResolver.ResolveCollision(bodyA, bodyB, result);

                                    Canvas.SetLeft(shapeA, bodyA.Position.X - shapeA.Width / 2);
                                    Canvas.SetTop(shapeA, bodyA.Position.Y - shapeA.Height / 2);
                                    Canvas.SetLeft(shapeB, bodyB.Position.X - shapeB.Width / 2);
                                    Canvas.SetTop(shapeB, bodyB.Position.Y - shapeB.Height / 2);
                                }
                            }
                            else // Rectangle is A, Ellipse is B
                            {
                                double radiusB = shapeB.Width / 2;

                                double halfWidth = shapeA.Width / 2;
                                double halfHeight = shapeA.Height / 2;

                                Vector2D bottomLeft = new Vector2D(
                                    bodyA.Position.X - halfWidth,
                                    bodyA.Position.Y - halfHeight
                                );
                                Vector2D topRight = new Vector2D(
                                    bodyA.Position.X + halfWidth,
                                    bodyA.Position.Y + halfHeight
                                );

                                var result = CollisionDetector.CircleToAABBCollision(bodyB.Position, radiusB, bottomLeft, topRight);

                                if (result.IsColliding)
                                {

                                    double correctionPercent = 0.4;
                                    double slop = 0.02;

                                    double correctionMagnitude = Math.Max(result.PenetrationDepth - slop, 0.0) * correctionPercent;
                                    Vector2D correction = result.Normal * correctionMagnitude;

                                    bodyA.Position -= correction * 0.5;  // Rectangle moves opposite to normal
                                    bodyB.Position += correction * 0.5;  // Circle moves along normal

                                    ImpulseResolver.ResolveCollision(bodyA, bodyB, result);

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
            foreach (var (shape, body) in objects)
            {
                Canvas.SetLeft(shape, body.Position.X - shape.Width / 2);
                Canvas.SetTop(shape, body.Position.Y - shape.Height / 2);
            }
        }   
    }
}