using supervisorioMMS.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace supervisorioMMS.Views
{
    public partial class PrincipalView : UserControl
    {
        public ObservableCollection<SynopticItem> SynopticItems { get; set; }
        private Point _startPoint;

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
            if (sender is ContentPresenter item && item.DataContext is SynopticItem)
            {
                _startPoint = e.GetPosition(item);
                DragDrop.DoDragDrop(item, item.DataContext, DragDropEffects.Move);
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
                if (EditorItemsControl.ActualWidth > 50) // Evita que o item "fuja" para a direita
                    newX = Math.Min(newX, EditorItemsControl.ActualWidth - 50);
                if (EditorItemsControl.ActualHeight > 50) // Evita que o item "fuja" para baixo
                    newY = Math.Min(newY, EditorItemsControl.ActualHeight - 50);

                itemToMove.X = newX;
                itemToMove.Y = newY;
            }
            // Caso 2: Criando um novo item arrastado da toolbox
            else if (e.Data.GetData(DataFormats.StringFormat) is string componentType)
            {
                SynopticItem? newItem = null;
                if (componentType == "Motor") newItem = new SynopticMotor();
                else if (componentType == "Sensor") newItem = new SynopticSensor();

                if (newItem != null)
                {
                    newItem.X = dropPosition.X;
                    newItem.Y = dropPosition.Y;
                    SynopticItems.Add(newItem);
                }
            }
        }
    }
}