using supervisorioMMS.ViewModels;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class UsuariosView : UserControl
    {
        public UsuariosView(UsuariosViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
