using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Medicine.MedicineSet;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow
{
    /// <summary>
    /// MedicineSetWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineSetWindow : Window
    {
        private MedicineSetViewModel medicineSetViewModel;

        public MedicineSetWindow(MedicineSetMode mode, MedicineSet selected = null)
        {
            InitializeComponent();
            medicineSetViewModel = new MedicineSetViewModel(mode, selected);
            DataContext = medicineSetViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseMedicineSetWindow"))
                {
                    Close();
                }
            });
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (!(row?.Item is Medicine med)) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<Medicine>(this, med, nameof(MedicineSetWindow)));
        }

        private void MedicineID_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            if (MedicineSetGrid.CurrentCell.Item is null) return;
            if (MedicineSetGrid.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
            {
                var itemsCount = MedicineSetGrid.Items.Count;
                (DataContext as MedicineSetViewModel)?.AddMedicine.Execute(textBox.Text);
                textBox.Text = string.Empty;

                if (MedicineSetGrid.Items.Count != itemsCount)
                    MedicineSetGrid.CurrentCell = new DataGridCellInfo(MedicineSetGrid.Items[MedicineSetGrid.Items.Count - 2], MedicineSetGrid.Columns[3]);
            }
            else if (MedicineSetGrid.CurrentCell.Item is Medicine med)
            {
                if (!med.ID.Equals(textBox.Text))
                    ((MedicineSetViewModel)DataContext).AddMedicine.Execute(textBox.Text);

                var textBoxList = new List<TextBox>();
                NewFunction.FindChildGroup(MedicineSetGrid, "MedicineID", ref textBoxList);
                var index = textBoxList.IndexOf((TextBox)sender);
                if (!((Medicine)MedicineSetGrid.Items[index]).ID.Equals(textBox.Text))
                    textBox.Text = ((Medicine)MedicineSetGrid.Items[index]).ID;

                MedicineSetGrid.CurrentCell = new DataGridCellInfo(MedicineSetGrid.Items[index], MedicineSetGrid.Columns[3]);
            }
            MedicineSetGrid.SelectedItem = MedicineSetGrid.CurrentCell.Item;

            var focusedCell = MedicineSetGrid.CurrentCell.Column.GetCellContent(MedicineSetGrid.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                t.CaretIndex++;
            }
        }

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(MedicineSetGrid, textBox.Name, ref textBoxList);
            var index = textBoxList.IndexOf((TextBox)sender);
            MedicineSetGrid.SelectedItem = (MedicineSetGrid.Items[index] as Medicine);
        }

        private void InputTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void MedicineSetGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter) return;
            TextBox t = sender as TextBox;
            e.Handled = true;
            MoveFocusNext(sender);
        }

        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (MedicineSetGrid.CurrentCell.Column is null) return;

            var focusedCell =
                MedicineSetGrid.CurrentCell.Column.GetCellContent(MedicineSetGrid.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }

                    if ((child is TextBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell =
                    MedicineSetGrid.CurrentCell.Column.GetCellContent(MedicineSetGrid.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is TextBlock) && firstChild.Focusable)
            {
                firstChild.Focus();
                if (firstChild is TextBox t)
                    t.SelectAll();
            }
        }

        private void MedicineTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var focusIndex = GetCurrentRowIndex(sender) + 1;
                FocusDataGridCell("MedicineID", MedicineSetGrid, focusIndex);
            }
        }

        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                    {
                        var temp = new List<TextBox>();
                        NewFunction.FindChildGroup(MedicineSetGrid, textBox.Name, ref temp);
                        for (var x = 0; x < temp.Count; x++)
                        {
                            if (temp[x].Equals(textBox))
                                return x;
                        }
                        break;
                    }
            }
            return -1;
        }

        private void FocusDataGridCell(string controlName, DataGrid focusGrid, int rowIndex)
        {
            var dataGridCells = new List<TextBox>();
            NewFunction.FindChildGroup(focusGrid, controlName, ref dataGridCells);
            if (controlName.Equals("MedicineID") && rowIndex >= dataGridCells.Count)
                rowIndex = dataGridCells.Count - 1;
            if (rowIndex >= dataGridCells.Count) return;
            dataGridCells[rowIndex].Focus();
            dataGridCells[rowIndex].SelectAll();
            focusGrid.SelectedIndex = rowIndex;
        }
    }
}