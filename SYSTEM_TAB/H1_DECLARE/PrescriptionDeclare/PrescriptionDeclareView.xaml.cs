using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Messaging;
using DataGrid = System.Windows.Controls.DataGrid;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;
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
    }
}
