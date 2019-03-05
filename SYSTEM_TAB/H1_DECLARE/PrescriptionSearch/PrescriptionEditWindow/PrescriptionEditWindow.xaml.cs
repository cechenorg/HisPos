using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Interface;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    /// <summary>
    /// PrescriptionEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionEditWindow : Window
    {
        public PrescriptionEditWindow(Prescription selected, ViewModelEnum vm)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("ClosePrescriptionEditWindow"))
                    Close();
            });
            DataContext = new PrescriptionEditViewModel(selected,vm);
            Messenger.Default.Register<int>(this, "FocusDosage", FocusDosage);
            Closing += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void FocusDosage(int currentIndex)
        {
            FocusDataGridCell("Dosage", PrescriptionMedicines, currentIndex);
        }

        private void FocusChronicTotal(NotificationMessage msg)
        {
            if (msg.Notification.Equals("FocusChronicTotal"))
            {
                ChronicTotal.Focus();
                ChronicTotal.SelectionStart = 0;
            }
        }

        private void FocusSubDisease(NotificationMessage msg)
        {
            if (msg.Notification.Equals("FocusSubDisease"))
            {
                SecondDiagnosis.Focus();
                SecondDiagnosis.SelectionStart = 0;
            }
        }

        private void FocusDivision(NotificationMessage msg)
        {
            if (msg.Notification.Equals("FocusDivision"))
            {
                DivisionCombo.Focus();
            }
        }

        private void PrescriptionMedicines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DataGrid dg)) return;
            var index = dg.SelectedIndex;
            if (index == -1) return;
            ((PrescriptionEditViewModel)DataContext).SelectedMedicinesIndex = index;
        }

        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(((PrescriptionEditViewModel)DataContext).SelectedMedicine is IDeletable)) return;
            switch (PrescriptionMedicines.SelectedItem)
            {
                case MedicineNHI med:
                    {
                        if (!string.IsNullOrEmpty(med.Source))
                            Messenger.Default.Send(new NotificationMessage("DeleteMedicine"));
                        break;
                    }
                case MedicineOTC otc:
                    {
                        if (!string.IsNullOrEmpty(otc.Source))
                            Messenger.Default.Send(new NotificationMessage("DeleteMedicine"));
                        break;
                    }
                case MedicineSpecialMaterial spe:
                {
                    if (!string.IsNullOrEmpty(spe.Source))
                        Messenger.Default.Send(new NotificationMessage("DeleteMedicine"));
                    break;
                }
            }
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
            if(rowIndex >= dataGridCells.Count) return;
            dataGridCells[rowIndex].Focus();
            dataGridCells[rowIndex].SelectAll();
            focusGrid.SelectedIndex = rowIndex;
            ((PrescriptionEditViewModel)DataContext).SelectedMedicinesIndex = rowIndex;
        }

        private void MedicineID_OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var focusIndex = GetCurrentRowIndex(sender);
            ((PrescriptionEditViewModel)DataContext).previousSelectedIndex = focusIndex;
        }

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;
            textBox.SelectAll();
        }

        private void InputTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;
            
            e.Handled = true;
            textBox.Focus();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            if (!((Medicine)row.Item is MedicineNHI) && !((Medicine)row.Item is MedicineOTC) &&
                !((Medicine)row.Item is MedicineSpecialMaterial)) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<Medicine>(this, (Medicine)row.Item, nameof(PrescriptionEditWindow)));
        }
    }
}
