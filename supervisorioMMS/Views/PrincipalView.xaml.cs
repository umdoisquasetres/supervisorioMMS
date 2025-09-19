using supervisorioMMS.Models;
using supervisorioMMS.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace supervisorioMMS.Views
{
    public partial class PrincipalView : UserControl, INotifyPropertyChanged
    {
        private readonly TagService _tagService;
        public ObservableCollection<SynopticItem> SynopticItems { get; set; }

        private SynopticItem? _selectedSynopticItem;
        public SynopticItem? SelectedSynopticItem
        {
            get => _selectedSynopticItem;
            set
            {
                if (_selectedSynopticItem != value)
                {
                    // Deseleciona o item anterior
                    if (_selectedSynopticItem != null)
                    {
                        _selectedSynopticItem.IsSelected = false;
                    }
                    _selectedSynopticItem = value;
                    // Seleciona o novo item
                    if (_selectedSynopticItem != null)
                    {
                        _selectedSynopticItem.IsSelected = true;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Point _startPoint;
        private int _motorCounter = 0;
        private int _sensorCounter = 0;
        private int _valveCounter = 0;
        private int _displayCounter = 0;

        public PrincipalView(TagService tagService)
        {
            InitializeComponent();
            _tagService = tagService;
            this.DataContext = this;
            SynopticItems = new ObservableCollection<SynopticItem>();
            ToolboxList.ItemsSource = new[] { "Motor", "Sensor", "Válvula", "Display de Valor" };
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
                SelectedSynopticItem = synopticItem;
                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Move);
            }
        }

        private void EditorCanvas_Drop(object sender, DragEventArgs e)
        {
            Point dropPosition = e.GetPosition(EditorItemsControl);
            object data = e.Data.GetData(typeof(SynopticMotor)) ??
                          e.Data.GetData(typeof(SynopticSensor)) ??
                          e.Data.GetData(typeof(SynopticValve)) ??
                          e.Data.GetData(typeof(SynopticValueDisplay));

            if (data is SynopticItem itemToMove)
            {
                double newX = dropPosition.X - _startPoint.X;
                double newY = dropPosition.Y - _startPoint.Y;

                newX = Math.Max(0, newX);
                newY = Math.Max(0, newY);
                if (EditorItemsControl.ActualWidth > 50)
                    newX = Math.Min(newX, EditorItemsControl.ActualWidth - 80); // Subtrai a largura para não cortar
                if (EditorItemsControl.ActualHeight > 50)
                    newY = Math.Min(newY, EditorItemsControl.ActualHeight - 80); // Subtrai a altura para não cortar

                itemToMove.X = newX;
                itemToMove.Y = newY;
            }
            else if (e.Data.GetData(DataFormats.StringFormat) is string componentType)
            {
                SynopticItem? newItem = null;
                if (componentType == "Motor")
                {
                    _motorCounter++;
                    newItem = new SynopticMotor(_tagService) { Label = $"Motor {_motorCounter}" };
                }
                else if (componentType == "Sensor")
                {
                    _sensorCounter++;
                    newItem = new SynopticSensor(_tagService) { Label = $"Sensor {_sensorCounter}" };
                }
                else if (componentType == "Válvula")
                {
                    _valveCounter++;
                    newItem = new SynopticValve(_tagService) { Label = $"Válvula {_valveCounter}" };
                }
                else if (componentType == "Display de Valor")
                {
                    _displayCounter++;
                    newItem = new SynopticValueDisplay(_tagService) { Label = $"Display {_displayCounter}", Unit = "N/A" };
                }

                if (newItem != null)
                {
                    newItem.X = dropPosition.X;
                    newItem.Y = dropPosition.Y;
                    SynopticItems.Add(newItem);
                    SelectedSynopticItem = newItem;
                }
            }
        }

        #region Click Handlers
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

        private void ValveOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is SynopticValve valve)
            {
                valve.Open();
            }
        }

        private void ValveClose_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is SynopticValve valve)
            {
                valve.Close();
            }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
