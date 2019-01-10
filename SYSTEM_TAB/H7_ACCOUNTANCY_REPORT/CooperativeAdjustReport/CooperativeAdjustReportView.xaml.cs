using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class.Product;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeAdjustReport
{
    /// <summary>
    /// CooperativeAdjustReportView.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeAdjustReportView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class CooperativeAdjustMed{

            public CooperativeAdjustMed(DataRow dataRow) {
                MedId = dataRow["HISMED_ID"].ToString();
                MedName = dataRow["PRO_NAME"].ToString();
                MedUseAmount = dataRow["HISDECDET_AMOUNT"].ToString();
            }
            public string MedId { get; set; }
            public string MedName { get; set; }
            public string MedUseAmount { get; set; }
        }
        private ObservableCollection<CooperativeAdjustMed> cooperativeAdjustMedCollection;
        public ObservableCollection<CooperativeAdjustMed> CooperativeAdjustMedCollection
        {
            get => cooperativeAdjustMedCollection;
            set
            {
                cooperativeAdjustMedCollection = value;
                NotifyPropertyChanged("CooperativeAdjustMedCollection");
            }
        }

        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime SDateTime
        {
            get => sDateTime;
            set
            {
                sDateTime = value;
                NotifyPropertyChanged("SDateTime");
            }
        }
        private DateTime eDateTime = DateTime.Now;
        public DateTime EDateTime
        {
            get => eDateTime;
            set
            {
                eDateTime = value;
                NotifyPropertyChanged("EDateTime");
            }
        }
        public CooperativeAdjustReportView()
        {
            InitializeComponent();
            DataContext = this;
            InitData();
        }

        public void InitData() {
           /// CooperativeAdjustMedCollection = ProductDb.GetCooperativeAdjustMed(SDateTime,EDateTime);
        }

        #region ----- Date Control -----

        private void Date_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;

                if (textBox is null) return;

                switch (textBox.Name)
                {
                    case "StartDate":
                        EndDate.Focus();
                        EndDate.SelectAll();
                        break;
                    case "EndDate":
                        SearchButton.Focus();
                        break;
                }
            }
        }
        #endregion

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           /// CooperativeAdjustMedCollection = ProductDb.GetCooperativeAdjustMed(SDateTime, EDateTime);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           /// CooperativeAdjustMedCollection = ProductDb.GetCooperativeAdjustMed(DateTime.Now.AddDays(-DateTime.Now.Day + 1), DateTime.Now);
        }
    }
}
