using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using Xceed.Wpf.Toolkit;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindowRefactoring
{
    /// <summary>
    /// PrescriptionEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionEditWindow : Window
    {
        public PrescriptionEditWindow()
        {
            InitializeComponent();
        }

        public PrescriptionEditWindow(Prescription p)
        {
            InitializeComponent();
        }

        private void FocusDosage(NotificationMessage<int> msg)
        {
            if (msg.Sender is PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel && msg.Notification.Equals("FocusDosage"))
                FocusDataGridCell("Dosage", PrescriptionMedicines, msg.Content);
        }

        private void FocusChronicTotal(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel && msg.Notification.Equals("FocusChronicTotal"))
            {
                ChronicTotal.Focus();
                ChronicTotal.SelectionStart = 0;
            }
        }

        private void FocusSubDisease(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel && msg.Notification.Equals("FocusSubDisease"))
            {
                SecondDiagnosis.Focus();
                SecondDiagnosis.SelectionStart = 0;
            }
        }

        private void FocusDivision(NotificationMessage msg)
        {
            if (msg.Sender is PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel && msg.Notification.Equals("FocusDivision"))
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
                TreatDate.Focus();
                TreatDate.SelectionStart = 0;
            }
        }

        private void TreatDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainDisease.Focus();
                MainDisease.SelectionStart = 0;
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
            TextBox t = sender as TextBox;
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

                    if ((child is TextBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell =
                    PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
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
            ((PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel)DataContext).SelectedMedicinesIndex = rowIndex;
        }

        private void MedicineID_OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var focusIndex = GetCurrentRowIndex(sender);
            ((PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel)DataContext).previousSelectedIndex = focusIndex;
        }

        private void InputTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref textBoxList);
            var index = textBoxList.IndexOf((TextBox)sender);
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
            if (!(sender is DataGridCell cell) || !(cell.DataContext is Medicine med)) return;
            ((PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }

        private void MedicineID_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter) return;
            e.Handled = true;

            if (PrescriptionMedicines.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
            {
                var itemsCount = PrescriptionMedicines.Items.Count;
                (DataContext as PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel)?.AddMedicine.Execute(textBox.Text);
                textBox.Text = string.Empty;

                if (PrescriptionMedicines.Items.Count != itemsCount)
                    PrescriptionMedicines.CurrentCell = new DataGridCellInfo(PrescriptionMedicines.Items[PrescriptionMedicines.Items.Count - 2], PrescriptionMedicines.Columns[3]);
            }
            else if (PrescriptionMedicines.CurrentCell.Item is Medicine med)
            {
                if (!med.ID.Equals(textBox.Text))
                    ((PrescriptionSearch.PrescriptionEditWindow.PrescriptionEditViewModel)DataContext).AddMedicine.Execute(textBox.Text);

                var textBoxList = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineID", ref textBoxList);
                var index = textBoxList.IndexOf((TextBox)sender);
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
            TextBox t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                t.CaretIndex++;
            }
        }

        private void MedicineID_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                    PrescriptionMedicines.SelectedItem = PrescriptionMedicines.CurrentCell.Item;
                }
            }
        }
    }
}
