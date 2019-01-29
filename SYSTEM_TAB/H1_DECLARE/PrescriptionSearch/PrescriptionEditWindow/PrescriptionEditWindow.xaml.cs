﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Interface;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    /// <summary>
    /// PrescriptionEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionEditWindow : Window
    {
        public PrescriptionEditWindow(Prescription selected)
        {
            InitializeComponent();
            DataContext = new PrescriptionEditViewModel(selected);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("ClosePrescriptionEditWindow"))
                    Close();
            });
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
        }
        private void PrescriptionMedicines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DataGrid dg)) return;
            var index = dg.SelectedIndex;
            if (index == -1) return;
            ((PrescriptionEditViewModel)DataContext).SelectedMedicinesIndex = index;
        }

        private void DateControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;
            if (!(selectedItem is IDeletable deletable)) return;
            if (selectedItem is MedicineNHI || selectedItem is MedicineOTC)
                deletable.Source = string.Empty;
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;
            if (!(selectedItem is IDeletable deletable)) return;
            if (selectedItem is MedicineNHI || selectedItem is MedicineOTC)
                deletable.Source = "/Images/DeleteDot.png";
            ((PrescriptionEditViewModel)DataContext).SelectedMedicine = (Medicine)selectedItem;
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
            }
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
                firstChild.Focus();
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
            dataGridCells[rowIndex].Focus();
            dataGridCells[rowIndex].SelectionStart = 0;
        }
    }
}