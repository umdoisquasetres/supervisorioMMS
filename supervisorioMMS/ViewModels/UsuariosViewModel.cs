using supervisorioMMS.Models;
using supervisorioMMS.Views; // Required for the User class
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace supervisorioMMS.ViewModels
{
    public class UsuariosViewModel : BaseViewModel
    {
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private User? _editingUser;

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _accessLevel = "Operador";
        public string AccessLevel
        {
            get => _accessLevel;
            set { _accessLevel = value; OnPropertyChanged(); }
        }

        public string[] AccessLevels { get; } = { "Operador", "Administrador" };

        private string _formTitle = "Adicionar Usuário";
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

        public UsuariosViewModel()
        {
            _users = new ObservableCollection<User>
            {
                new User { Name = "admin", Password = "admin", AccessLevel = "Administrador" },
                new User { Name = "operador1", Password = "op1", AccessLevel = "Operador" }
            };

            AddOrSaveCommand = new RelayCommand(_ => AddOrSaveUser());
            EditCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
            RemoveCommand = new RelayCommand(_ => RemoveUser(), _ => SelectedUser != null);
        }

        private void AddOrSaveUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(AccessLevel))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_editingUser != null)
            {
                if (Users.Any(u => u.Name.Equals(Name, System.StringComparison.OrdinalIgnoreCase) && u != _editingUser))
                {
                    MessageBox.Show("Um usuário com este nome já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _editingUser.Name = Name;
                _editingUser.Password = Password;
                _editingUser.AccessLevel = AccessLevel;
            }
            else
            {
                if (Users.Any(u => u.Name.Equals(Name, System.StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Um usuário com este nome já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var newUser = new User { Name = Name, Password = Password, AccessLevel = AccessLevel };
                Users.Add(newUser);
            }

            ClearAndResetForm();
        }

        private void EditUser()
        {
            if (SelectedUser == null) return;

            _editingUser = SelectedUser;
            Name = _editingUser.Name;
            Password = _editingUser.Password;
            AccessLevel = _editingUser.AccessLevel;

            FormTitle = "Editar Usuário";
            AddButtonContent = "Salvar Alterações";
        }

        private void RemoveUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Tem certeza que deseja remover o usuário '{SelectedUser.Name}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Users.Remove(SelectedUser);
                ClearAndResetForm();
            }
        }

        private void ClearAndResetForm()
        {
            _editingUser = null;
            Name = string.Empty;
            Password = string.Empty;
            AccessLevel = "Operador";
            SelectedUser = null;
            FormTitle = "Adicionar Usuário";
            AddButtonContent = "Adicionar";
        }
    }
}
