using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace supervisorioMMS.Views
{
    public partial class PrincipalView : UserControl
    {
        public PrincipalView()
        {
            InitializeComponent();
        }

        private void BtnMotorOn_Click(object sender, RoutedEventArgs e)
        {
            MotorIndicator.Fill = new SolidColorBrush(Colors.Green);
        }

        private void BtnMotorOff_Click(object sender, RoutedEventArgs e)
        {
            MotorIndicator.Fill = new SolidColorBrush(Colors.Red);
        }
    }
}
