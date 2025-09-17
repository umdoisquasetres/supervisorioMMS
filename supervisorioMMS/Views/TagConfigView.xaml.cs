using supervisorioMMS.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class TagConfigView : UserControl
    {
        private ModbusTag _editingTag = null;

        public TagConfigView()
        {
            InitializeComponent();
            DataContext = TagService.Instance;
            DataTypeComboBox.ItemsSource = Enum.GetValues(typeof(ModbusDataType));
        }

        private void AddOrSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || 
                !int.TryParse(AddressTextBox.Text, out int address) || 
                DataTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dataType = (ModbusDataType)DataTypeComboBox.SelectedItem;

            if (_editingTag != null)
            {
                _editingTag.Name = NameTextBox.Text;
                _editingTag.Address = address;
                _editingTag.DataType = dataType;
            }
            else
            {
                var newTag = new ModbusTag
                {
                    Name = NameTextBox.Text,
                    Address = address,
                    DataType = dataType
                };
                TagService.Instance.Tags.Add(newTag);
            }

            ClearAndResetForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TagsGrid.SelectedItem is ModbusTag selectedTag)
            {
                _editingTag = selectedTag;
                NameTextBox.Text = _editingTag.Name;
                AddressTextBox.Text = _editingTag.Address.ToString();
                DataTypeComboBox.SelectedItem = _editingTag.DataType;

                AddButton.Content = "Salvar Alterações";
                FormGroupBox.Header = "Editar Tag";
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tag para editar.", "Nenhuma Tag Selecionada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TagsGrid.SelectedItem is ModbusTag selectedTag)
            {
                TagService.Instance.Tags.Remove(selectedTag);
                ClearAndResetForm();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tag para remover.", "Nenhuma Tag Selecionada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearAndResetForm()
        {
            _editingTag = null;
            NameTextBox.Clear();
            AddressTextBox.Clear();
            DataTypeComboBox.SelectedIndex = -1;
            AddButton.Content = "Adicionar";
            FormGroupBox.Header = "Adicionar Tag";
        }
    }
}
