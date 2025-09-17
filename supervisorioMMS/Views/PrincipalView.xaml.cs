using supervisorioMMS.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class PrincipalView : UserControl
    {
        // Nomes das tags que esperamos encontrar no TagService
        private const string MotorStatusTagName = "Motor M-101 Status";
        private const string NivelTanqueTagName = "Nível Tanque 1";

        public PrincipalView()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                SetupBindings();
            }
        }

        private void SetupBindings()
        {
            // Encontra as tags no serviço e define o DataContext para cada elemento
            var motorTag = TagService.Instance.Tags.FirstOrDefault(t => t.Name == MotorStatusTagName);
            if (motorTag != null)
            {
                MotorIndicator.DataContext = motorTag;
            }

            var nivelTag = TagService.Instance.Tags.FirstOrDefault(t => t.Name == NivelTanqueTagName);
            if (nivelTag != null)
            {
                NivelTanqueText.DataContext = nivelTag;
            }
        }

        private void BtnMotorOn_Click(object sender, RoutedEventArgs e)
        {
            TagService.Instance.WriteTagValue(MotorStatusTagName, true);
        }

        private void BtnMotorOff_Click(object sender, RoutedEventArgs e)
        {
            TagService.Instance.WriteTagValue(MotorStatusTagName, false);
        }
    }
}
