using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinetX.Calculation;
using KinetX.Collision;

namespace KinetX
{
    public partial class MainWindow : Window
    {
        private Stopwatch stopwatch = new Stopwatch();

        private List<UIElement> shapes = new List<UIElement>();
        private List<Vector> velocities = new List<Vector>();
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
            shapes.Clear();
            velocities.Clear();

            //To Read shape types
            string shapeA = ((ComboBoxItem)ObjectAComboBox.SelectedItem).Content.ToString();
            string shapeB = ((ComboBoxItem)ObjectBComboBox.SelectedItem).Content.ToString();

            //To Read velocities
            double ax = double.Parse(VelocityAX.Text);
            double ay = double.Parse(VelocityAY.Text);

            double bx = double.Parse(VelocityBX.Text);
            double by = double.Parse(VelocityBY.Text);

            //Read object count 
            int ObjectACount = (int)ObjectACountSlider.Value;
            int ObjectBCount = (int)ObjectBCountSlider.Value;

            for (int i = 0; i < ObjectACount; i++)
            {
                //creating shapes 
                var objA = CreateShape(shapeA);

                //Random start position
                RandomSpawn(objA, MainCanvas);

                //Adding it to canvas
                MainCanvas.Children.Add(objA);

                //Store references , crucial for animation.
                shapes.Add(objA);
                velocities.Add(new Vector(ax, ay));
            }
            for (int i = 0; i < ObjectBCount; i++)
            {
                //creating shapes 
                var objB = CreateShape(shapeB);


                //Random start position
                RandomSpawn(objB, MainCanvas);

                //Adding it to canvas
                MainCanvas.Children.Add(objB);

                //Store references , crucial for animation.
                shapes.Add(objB);
                velocities.Add(new Vector(bx, by));
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

            shape.Width = 50;
            shape.Height = 50;
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
            double deltaTime = stopwatch.Elapsed.TotalSeconds; // how long since last frame
            stopwatch.Restart();
            if (!isAnimating) return;

            for (int i = 0; i < shapes.Count; i++)
            {
                var shape = (Shape)shapes[i];
                var velocity = velocities[i];

                double x = Canvas.GetLeft(shape); //how far from the left edge of the canvas the shape is.
                double y = Canvas.GetTop(shape); //how far from the top.

                double newX = x + velocity.X * deltaTime * rate;
                double newY = y + velocity.Y * deltaTime * rate;

                if (newX < 0 || newX + shape.Width > MainCanvas.ActualWidth)
                {
                    velocity.X *= -1;
                    newX = x + velocity.X * deltaTime * rate;
                }
                if (newY < 0 || newY + shape.Height > MainCanvas.ActualHeight)
                {
                    velocity.Y *= -1;
                    newY = y + velocity.Y * deltaTime * rate;
                }
                Canvas.SetLeft(shape, newX);
                Canvas.SetTop(shape, newY);
                velocities[i] = velocity; //It update the new velocity
            }
            for (int i = 0;i < shapes.Count;i++)
            {
                for (int j = i + 1; j < shapes.Count;j++)
                {
                    var shapeA = (Shape)shapes[i];
                    var shapeB = (Shape)shapes[j];

                    if(shapeA is Ellipse && shapeB is Ellipse)
                    {
                        //getting the center points of both the circle.
                        double ax = Canvas.GetLeft(shapeA) + shapeA.Width / 2;
                        double ay = Canvas.GetTop(shapeA) + shapeA.Height / 2;
                        double bx = Canvas.GetLeft(shapeB) + shapeB.Width / 2;
                        double by = Canvas.GetTop(shapeB) + shapeB.Height / 2;

                        //converting the center to vector2D for calculations.
                        var centerA = new Vector2D(ax, ay);
                        var centerB = new Vector2D(bx, by);

                        //getting the radius of both the circle.
                        double radiusA = shapeA.Width / 2;
                        double radiusB = shapeB.Width / 2;

                        var result = CollisionDetector.CircleToCircle(centerA, radiusA, centerB, radiusB);

                        if (result.IsColliding)
                        {
                            velocities[i] *= -1;
                            velocities[j] *= -1;

                            // Moving the circles apart based on the penetration depth.
                            var correction = result.Normal * result.PenetrationDepth / 2;
                            Canvas.SetLeft(shapeA, Canvas.GetLeft(shapeA) + correction.X);
                            Canvas.SetTop(shapeA, Canvas.GetTop(shapeA) + correction.Y);
                            Canvas.SetLeft(shapeB, Canvas.GetLeft(shapeB) - correction.X);
                            Canvas.SetTop(shapeB, Canvas.GetTop(shapeB) - correction.Y);
                        }
                    }
                }
            }
        }
    }
}