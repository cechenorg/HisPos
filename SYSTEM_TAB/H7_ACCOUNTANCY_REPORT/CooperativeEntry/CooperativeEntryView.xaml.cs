using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeEntry
{
    /// <summary>
    /// CooperativeEntryView.xaml 的互動邏輯
    /// </summary> 
    public partial class CooperativeEntryView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class CopaymentEntry {
            public CopaymentEntry() { }
            public CopaymentEntry(DataRow row) {
                Date = DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(row["CASHFLOW_DATE"]));
                CopaymentValue = row["部分負擔"].ToString();
                ClinicCopaymentValue = row["合作診所部分負擔"].ToString();
                PaySelfValue = row["自費"].ToString();
                ClinicPaySelfValue = row["合作診所自費"].ToString();
                MedServiceValue = row["藥服費"].ToString();
                ClinicMedServiceValue = row["合作診所藥服費"].ToString();
                DepositValue = row["押金"].ToString(); 
            }

            public string Date { get; set; }
            public string CopaymentValue { get; set; }
            public string ClinicCopaymentValue { get; set; }

            public string PaySelfValue { get; set; }

            public string ClinicPaySelfValue { get; set; }
            public string MedServiceValue { get; set; }
            public string ClinicMedServiceValue { get; set; }
            public string DepositValue { get; set; }
        }

        private ObservableCollection<CopaymentEntry> cooperativeClinicEntryCollection;
        public ObservableCollection<CopaymentEntry> CooperativeClinicEntryCollection
        {
            get => cooperativeClinicEntryCollection;
            set
            {
                cooperativeClinicEntryCollection = value;
                NotifyPropertyChanged("CooperativeClinicEntryCollection");
            }
        } 
        private  CopaymentEntry totalCopaymentEntry = new CopaymentEntry();
        public CopaymentEntry TotalCopaymentEntry
        {
            get => totalCopaymentEntry;
            set
            {
                totalCopaymentEntry = value;
                NotifyPropertyChanged("TotalCopaymentEntry");
            }
        }
        private DateTime startDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }
        public CooperativeEntryView()
        {
            InitializeComponent();
            DataContext = this;
            InitData();
        }
        private void InitData(string startDate = null, string endDate = null) {
            if (startDate == null)
                ;/// CooperativeClinicEntryCollection = ProductDb.GetCopayMentValue(TotalCopaymentEntry);
            else
                ;/// CooperativeClinicEntryCollection = ProductDb.GetCopayMentValue(TotalCopaymentEntry, startDate, endDate); 
            LabelstartDate.Content = StartDate.AddYears(-1911).ToString("yyy/MM/dd");
            LabelendDate.Content = EndDate.AddYears(-1911).ToString("yyy/MM/dd");
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            InitData(StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd"));
            NotifyPropertyChanged("TotalCopaymentEntry");
        }
    }
}
