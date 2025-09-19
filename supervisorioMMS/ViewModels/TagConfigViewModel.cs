using supervisorioMMS.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace supervisorioMMS.ViewModels
{
    public class TagConfigViewModel : BaseViewModel
    {
        public ObservableCollection<ModbusTag> Tags => TagService.Instance.Tags;

        private ModbusTag? _selectedTag;
        public ModbusTag? SelectedTag
        {
            get => _selectedTag;
            set { _selectedTag = value; OnPropertyChanged(); }
        }

        private ModbusTag? _editingTag;

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private ModbusDataType _dataType;
        public ModbusDataType DataType
        {
            get => _dataType;
            set { _dataType = value; OnPropertyChanged(); }
        }

        public ModbusDataType[] DataTypes { get; } = (ModbusDataType[])Enum.GetValues(typeof(ModbusDataType));

        private string _formTitle = "Adicionar Tag";
        public string FormTitle
        {
            get => _formTitle;
            set { _formTitle = value; OnPropertyChanged(); }
        }

        private string _addButtonContent = "Adicionar";
        public string AddButtonContent
        {
            get => _addButtonContent;
            set { _addButtonContent = value; OnPropertyChanged(); }
        }

        public ICommand AddOrSaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }

        public TagConfigViewModel()
        {
            AddOrSaveCommand = new RelayCommand(_ => AddOrSaveTag());
            EditCommand = new RelayCommand(_ => EditTag(), _ => SelectedTag != null);
            RemoveCommand = new RelayCommand(_ => RemoveTag(), _ => SelectedTag != null);
        }

        private void AddOrSaveTag()
        {
            if (string.IsNullOrWhiteSpace(Name) || !int.TryParse(Address, out int address))
            {
                MessageBox.Show("Por favor, preencha todos os campos corretamente.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_editingTag != null)
            {
                _editingTag.Name = Name;
                _editingTag.Address = address;
                _editingTag.DataType = DataType;
            }
            else
            {
                var newTag = new ModbusTag
                {
                    Name = Name,
                    Address = address,
                    DataType = DataType
                };
                Tags.Add(newTag);
            }

            ClearAndResetForm();
        }

        private void EditTag()
        {
            if (SelectedTag == null) return;

            _editingTag = SelectedTag;
            Name = _editingTag.Name;
            Address = _editingTag.Address.ToString();
            DataType = _editingTag.DataType;

            FormTitle = "Editar Tag";
            AddButtonContent = "Salvar Alterações";
        }

        private void RemoveTag()
        {
            if (SelectedTag == null) return;
            
            Tags.Remove(SelectedTag);
            ClearAndResetForm();
        }

        private void ClearAndResetForm()
        {
            _editingTag = null;
            Name = string.Empty;
            Address = string.Empty;
            DataType = ModbusDataType.Coil;
            SelectedTag = null;
            FormTitle = "Adicionar Tag";
            AddButtonContent = "Adicionar";
        }
    }
}
