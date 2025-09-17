using supervisorioMMS.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace supervisorioMMS.Views
{
    public partial class PrincipalView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<SynopticItem> SynopticItems { get; set; }

        private SynopticItem? _selectedSynopticItem;
        public SynopticItem? SelectedSynopticItem
        {
            get => _selectedSynopticItem;
            set
            {
                if (_selectedSynopticItem != value)
                {
                    _selectedSynopticItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private Point _startPoint;
        private int _motorCounter = 0;
        private int _sensorCounter = 0;

        public PrincipalView()
        {
            InitializeComponent();
            this.DataContext = this;
            SynopticItems = new ObservableCollection<SynopticItem>();
            ToolboxList.ItemsSource = new[] { "Motor", "Sensor" };
        }

        private void Toolbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.Content is string componentType)
            {
                DragDrop.DoDragDrop(listBoxItem, componentType, DragDropEffects.Copy);
            }
        }

        private void EditorItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentPresenter item && item.DataContext is SynopticItem synopticItem)
            {
                _startPoint = e.GetPosition(item);
                // Deselect previous item
                if (SelectedSynopticItem != null) SelectedSynopticItem.IsSelected = false;
                // Select current item
                synopticItem.IsSelected = true;
                SelectedSynopticItem = synopticItem;

                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Move);
            }
        }

        private void EditorItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Este evento não é estritamente necessário para drag-drop, pois DoDragDrop é bloqueante
            // e lida com a captura/liberação do mouse internamente.
        }

        private void MotorOn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is SynopticMotor motor)
            {
                motor.TurnOn();
            }
        }

        private void MotorOff_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is SynopticMotor motor)
            {
                motor.TurnOff();
            }
        }

        private void EditorCanvas_Drop(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(EditorItemsControl);
            object data = e.Data.GetData(typeof(SynopticMotor)) ?? e.Data.GetData(typeof(SynopticSensor));

            // Caso 1: Movendo um item existente que foi pego do canvas
            if (data is SynopticItem itemToMove)
            {
                double newX = dropPosition.X - _startPoint.X;
                double newY = dropPosition.Y - _startPoint.Y;

                // Garante que o item não saia dos limites do canvas
                newX = Math.Max(0, newX);
                newY = Math.Max(0, newY);
                if (EditorItemsControl.ActualWidth > 50) 
                    newX = Math.Min(newX, EditorItemsControl.ActualWidth - 50);
                if (EditorItemsControl.ActualHeight > 50) 
                    newY = Math.Min(newY, EditorItemsControl.ActualHeight - 50);

                itemToMove.X = newX;
                itemToMove.Y = newY;
            }
            // Caso 2: Criando um novo item arrastado da toolbox
            else if (e.Data.GetData(DataFormats.StringFormat) is string componentType)
            {
                SynopticItem? newItem = null;
                if (componentType == "Motor")
                {
                    _motorCounter++;
                    newItem = new SynopticMotor { Label = $"Motor {_motorCounter}" };
                }
                else if (componentType == "Sensor")
                {
                    _sensorCounter++;
                    newItem = new SynopticSensor { Label = $"Sensor {_sensorCounter}" };
                }

                if (newItem != null)
                {
                    newItem.X = dropPosition.X;
                    newItem.Y = dropPosition.Y;
                    SynopticItems.Add(newItem);

                    // Seleciona o novo item
                    if (SelectedSynopticItem != null) SelectedSynopticItem.IsSelected = false;
                    newItem.IsSelected = true;
                    SelectedSynopticItem = newItem;
                }
            }
        }

        // Implementação de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}