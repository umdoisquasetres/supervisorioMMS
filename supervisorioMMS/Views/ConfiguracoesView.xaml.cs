using supervisorioMMS.ViewModels;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class ConfiguracoesView : UserControl
    {
        public ConfiguracoesView(ConfiguracoesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}