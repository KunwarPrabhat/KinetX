using KinetX.ViewModels;
using System.Windows;

namespace KinetX
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel(physicsCanvas);
            DataContext = _viewModel;
        }

        private void CollideButton_Click(object sender, RoutedEventArgs e)
        {
            // Get values from UI controls
            var objectAType = ObjectASelector.SelectedItem?.ToString() ?? "Circle";
            var objectBType = ObjectBSelector.SelectedItem?.ToString() ?? "Circle";
            var countA = (int)ObjectACountSlider.Value;
            var countB = (int)ObjectBCountSlider.Value;
            var restitutionA = RestitutionASlider.Value;
            var restitutionB = RestitutionBSlider.Value;
            
            double.TryParse(VelocityAX.Text, out var velocityAX);
            double.TryParse(VelocityAY.Text, out var velocityAY);
            double.TryParse(VelocityBX.Text, out var velocityBX);
            double.TryParse(VelocityBY.Text, out var velocityBY);
            
            double.TryParse(Mass1.Text, out var massA);
            double.TryParse(Mass2.Text, out var massB);

            // Start simulation
            _viewModel.StartCollisionSimulation(
                objectAType, objectBType,
                countA, countB,
                restitutionA, restitutionB,
                velocityAX, velocityAY,
                velocityBX, velocityBY,
                massA, massB);
        }
    }
}