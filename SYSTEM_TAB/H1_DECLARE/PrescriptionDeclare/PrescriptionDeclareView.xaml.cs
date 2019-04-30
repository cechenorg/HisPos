using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using DataGrid = System.Windows.Controls.DataGrid;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    /// <summary>
    /// PrescriptionDeclareView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDeclareView
    {
        public PrescriptionDeclareView()
        {
            InitializeComponent();
            DataContext = new PrescriptionDeclareViewModel();
            Messenger.Default.Register<NotificationMessage>("FocusDivision", FocusDivision);
            Messenger.Default.Register<NotificationMessage>("FocusSubDisease", FocusSubDisease);
            Messenger.Default.Register<NotificationMessage>("FocusChronicTotal", FocusChronicTotal);
            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void FocusChronicTotal(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionDeclareViewModel && msg.Notification.Equals("FocusChronicTotal"))
            {
                ChronicTotal.Focus();
                ChronicTotal.SelectionStart = 0;
            }
        }

        private void FocusSubDisease(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionDeclareViewModel && msg.Notification.Equals("FocusSubDisease"))
            {
                SecondDiagnosis.Focus();
                SecondDiagnosis.SelectionStart = 0;
            }
        }

        private void FocusDivision(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionDeclareViewModel && msg.Notification.Equals("FocusDivision"))
                DivisionCombo.Focus();
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void Division_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DivisionCombo.SelectedItem != null)
            {
                MedicalNumber.Focus();
                MedicalNumber.SelectAll();
            }
        }

        private void MedicalNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TreatDate.Focus();
                TreatDate.SelectionStart = 0;
            }
        }

        private void TreatDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdjustDate.Focus();
                AdjustDate.SelectionStart = 0;
            }
        }

        private void AdjustDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainDiagnosis.Focus();
                MainDiagnosis.SelectionStart = 0;
                e.Handled = true;
            }
        }

        private void ChronicTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ChronicSequence.Focus();
                ChronicSequence.SelectionStart = 0;
              
            }
        }

        private void ChronicSequence_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdjustCombo.Focus();
            }
        }

        private void AdjustCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PrescriptionCaseCombo.Focus();
            }
        }
        private void PrescriptionCaseCombo_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CopaymentCombo.Focus();
            }
        }

        private void CopaymentCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PaymentCategoryCombo.Focus();
            }
        }

        private void PaymentCategoryCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SpecialTreatCombo.Focus();
            }
        }

        private void SpecialTreatCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var dataGridTextBox = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID",
                    ref dataGridTextBox);
                dataGridTextBox[0].Focus();
                dataGridTextBox[0].SelectionStart = 0;
                PrescriptionMedicines.SelectedItem = PrescriptionMedicines.Items[0];
            }
        }
        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            MoveFocusNext(sender);
        }
        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionMedicines.CurrentCell.Column is null) return;

            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
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

                    if ((child is TextBox  || child is TextBlock))
                        break;
                }

                focusedCell?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell =
                    PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is TextBlock ) && firstChild.Focusable)
            {
                firstChild.Focus();
                if(firstChild is TextBox t)
                    t.SelectAll();
            }
        }

        private void MedicineTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var focusIndex = GetCurrentRowIndex(sender) + 1;
                FocusDataGridCell("MedicineID", PrescriptionMedicines, focusIndex);
            }
        }
        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                {
                    var temp = new List<TextBox>();
                    NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref temp);
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
        private void FocusDataGridCell(string controlName,DataGrid focusGrid,int rowIndex)
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

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref textBoxList);
            var index = textBoxList.IndexOf((TextBox) sender);
            PrescriptionMedicines.SelectedItem = (PrescriptionMedicines.Items[index] as Medicine);
        }

        private void InputTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (!(row?.Item is Medicine med)) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string>(this, med.ID, nameof(PrescriptionDeclareView)));
        }

        private void MedicineID_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter) return;
            e.Handled = true;

            if (PrescriptionMedicines.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
            {
                var itemsCount = PrescriptionMedicines.Items.Count;
                (DataContext as PrescriptionDeclareViewModel)?.AddMedicine.Execute(textBox.Text);
                textBox.Text = string.Empty;

                if (PrescriptionMedicines.Items.Count != itemsCount)
                    PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[PrescriptionMedicines.Items.Count - 2], PrescriptionMedicines.Columns[3]);
            }
            else if (PrescriptionMedicines.CurrentCell.Item is Medicine med)
            {
                if (!med.ID.Equals(textBox.Text))
                    ((PrescriptionDeclareViewModel)DataContext).AddMedicine.Execute(textBox.Text);

                var textBoxList = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
                var index = textBoxList.IndexOf((TextBox) sender);
                if (!((Medicine)PrescriptionMedicines.Items[index]).ID.Equals(textBox.Text))
                    textBox.Text = ((Medicine)PrescriptionMedicines.Items[index]).ID;

                PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[index], PrescriptionMedicines.Columns[3]);
            }
            PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            var t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                if (t != null) t.CaretIndex++;
            }
        }
        private void ShowPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            if (!(row.Item is CustomerHistory)) return;
            Messenger.Default.Send(new NotificationMessage(nameof(PrescriptionDeclareView) + "ShowPrescriptionEditWindow"));
        }
    }
}
