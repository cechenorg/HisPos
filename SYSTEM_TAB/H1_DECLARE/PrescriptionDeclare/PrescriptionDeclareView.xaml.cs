﻿using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Interface;
using His_Pos.NewClass.Product.Medicine;
using DataGrid = System.Windows.Controls.DataGrid;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

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
        }

        private void PrescriptionMedicines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var index = dg.SelectedIndex;
            if(index == -1) return;
            ((PrescriptionDeclareViewModel)DataContext).SelectedMedicinesIndex = index;
        }

        private void DateControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var selectedItem =  (sender as DataGridRow)?.Item;
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
            ((PrescriptionDeclareViewModel) DataContext).SelectedMedicine = (Medicine)selectedItem;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(((PrescriptionDeclareViewModel) DataContext).SelectedMedicine is IDeletable)) return;
            switch (PrescriptionMedicines.SelectedItem)
            {
                case MedicineNHI med:
                {
                    if(!string.IsNullOrEmpty(med.Source))
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
    }
}
