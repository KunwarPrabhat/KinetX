using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinetX.Gravitation
{
    public class PaintCanvas
    {
        public static void paint(Canvas canvas, double radius)
        {
            // Set a deep bluish background instead of dull gray
            canvas.Background = new SolidColorBrush(Color.FromRgb(15, 18, 40));

            double centerX = canvas.ActualWidth / 2.0;
            double centerY = canvas.ActualHeight / 2.0;

            Ellipse constraintArea = new Ellipse
            { 
                Width = radius * 2,
                Height = radius * 2,
                Fill = new SolidColorBrush(Color.FromArgb(60, 100, 200, 255)), // Brighter translucent blue
                Stroke = new SolidColorBrush(Color.FromRgb(100, 200, 255)),    // Vibrant border color
                StrokeThickness = 2.0,
                IsHitTestVisible = false              
            };

            Canvas.SetLeft(constraintArea, centerX - radius);
            Canvas.SetTop(constraintArea, centerY - radius);

            canvas.Children.Insert(0, constraintArea);
        }
    }
}
