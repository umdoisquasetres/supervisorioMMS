using supervisorioMMS.ViewModels;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class TagConfigView : UserControl
    {
        public TagConfigView()
        {
            InitializeComponent();
            DataContext = new TagConfigViewModel();
        }
    }
}