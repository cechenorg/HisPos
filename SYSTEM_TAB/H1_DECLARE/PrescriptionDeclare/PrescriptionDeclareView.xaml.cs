﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Interface;
using His_Pos.NewClass.Product.Medicine;
using DataGrid = System.Windows.Controls.DataGrid;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    /// <summary>
    /// PrescriptionDeclareView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDeclareView : UserControl
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
            var t = sender as MaskedTextBox;
            t.SelectionStart = 0;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var selectedItem =  (sender as DataGridRow)?.Item;
            if (selectedItem is IDeletable deletable)
            {
                if (selectedItem is MedicineNHI || selectedItem is MedicineOTC)
                    deletable.Source = string.Empty;

                PrescriptionMedicines.SelectedItem = deletable;
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;
            if (selectedItem is IDeletable deletable)
            {
                if (selectedItem is MedicineNHI || selectedItem is MedicineOTC)
                    deletable.Source = "/Images/DeleteDot.png";

                PrescriptionMedicines.SelectedItem = deletable;
            }
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PrescriptionMedicines.SelectedItem = (sender as DataGridRow)?.Item;
            if (PrescriptionMedicines.SelectedItem is IDeletable)
            {
                if(!string.IsNullOrEmpty((PrescriptionMedicines.SelectedItem as MedicineOTC).Source) || !string.IsNullOrEmpty((PrescriptionMedicines.SelectedItem as MedicineNHI).Source))
                {
                    Messenger.Default.Send(new NotificationMessage("DeleteMedicine"));
                }
            }
        }
    }
}
