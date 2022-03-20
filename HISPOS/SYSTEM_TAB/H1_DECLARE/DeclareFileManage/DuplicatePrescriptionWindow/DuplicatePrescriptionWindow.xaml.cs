using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.DuplicatePrescriptionWindow
{
    /// <summary>
    /// DuplicatePrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DuplicatePrescriptionWindow : Window
    {
        private DuplicatePrescriptionViewModel duplicatePrescriptionViewModel { get; set; }
        public bool ShowDialog { get; set; }

        public DuplicatePrescriptionWindow(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            duplicatePrescriptionViewModel = new DuplicatePrescriptionViewModel(startDate, endDate);
            DataContext = duplicatePrescriptionViewModel;
            ShowDialog = duplicatePrescriptionViewModel.ShowDialog;
        }
    }
}