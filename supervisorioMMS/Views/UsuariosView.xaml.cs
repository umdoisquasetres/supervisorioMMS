using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class UsuariosView : UserControl
    {
        private ObservableCollection<User> _users;
        private User _editingUser = null; // O usuário atualmente em edição

        public UsuariosView()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            _users = new ObservableCollection<User>
            {
                new User { Name = "admin", Password = "admin", AccessLevel = "Administrador" },
                new User { Name = "operador1", Password = "op1", AccessLevel = "Operador" }
            };
            UsersGrid.ItemsSource = _users;
        }

        private void AddOrSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password) || AccessLevelComboBox.SelectedItem == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Se estivermos editando um usuário existente
            if (_editingUser != null)
            {
                if (_users.Any(u => u.Name.Equals(NameTextBox.Text, System.StringComparison.OrdinalIgnoreCase) && u != _editingUser))
                {
                    MessageBox.Show("Um usuário com este nome já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _editingUser.Name = NameTextBox.Text;
                _editingUser.Password = PasswordBox.Password;
                _editingUser.AccessLevel = (AccessLevelComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                UsersGrid.Items.Refresh();
            }
            // Se estivermos adicionando um novo usuário
            else
            {
                if (_users.Any(u => u.Name.Equals(NameTextBox.Text, System.StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Um usuário com este nome já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newUser = new User
                {
                    Name = NameTextBox.Text,
                    Password = PasswordBox.Password,
                    AccessLevel = (AccessLevelComboBox.SelectedItem as ComboBoxItem).Content.ToString()
                };
                _users.Add(newUser);
            }

            ClearAndResetForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                _editingUser = selectedUser;
                NameTextBox.Text = _editingUser.Name;
                PasswordBox.Password = _editingUser.Password;
                AccessLevelComboBox.Text = _editingUser.AccessLevel;

                AddUserButton.Content = "Salvar Alterações";
                FormGroupBox.Header = "Editar Usuário";
            }
            else
            {
                MessageBox.Show("Por favor, selecione um usuário para editar.", "Nenhum Usuário Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show($"Tem certeza que deseja remover o usuário '{selectedUser.Name}'?", "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _users.Remove(selectedUser);
                    ClearAndResetForm();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um usuário para remover.", "Nenhum Usuário Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearAndResetForm()
        {
            _editingUser = null;
            NameTextBox.Clear();
            PasswordBox.Clear();
            AccessLevelComboBox.SelectedIndex = -1;
            AddUserButton.Content = "Adicionar";
            FormGroupBox.Header = "Adicionar Usuário";
        }
    }

        public class User : System.ComponentModel.INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _password;
        public string Password // Em um aplicativo real, isso deve ser tratado com mais segurança
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private string _accessLevel;
        public string AccessLevel
        {
            get { return _accessLevel; }
            set
            {
                if (_accessLevel != value)
                {
                    _accessLevel = value;
                    OnPropertyChanged(nameof(AccessLevel));
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}