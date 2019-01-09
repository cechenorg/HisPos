using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    /// <summary>
    /// PurchaseReturnReportView.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseReturnReportView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Inner Struct -----
        public struct PurchaseReturnRecord
        {
            public PurchaseReturnRecord(DataRow row)
            {
                Manufactory = row["MAN_NAME"].ToString();
                StoOrderId = row["STOORD_ID"].ToString();
                ProductId = row["PRO_ID"].ToString();
                ProductName = row["NAME"].ToString();
                Amount = row["STOORDDET_QTY"].ToString();
                FreeAmount = row["STOORDDET_FREEQTY"].ToString();
                Price = row["STOORDDET_PRICE"].ToString();
                SubTotal = row["STOORDDET_SUBTOTAL"].ToString();
                Type = row["STOORD_TYPE"].ToString() + "貨";
                RecDate = row["STOORD_RECDATE"].ToString();
            }

            public string Manufactory { get; set; }
            public string StoOrderId { get; set; }
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string Amount { get; set; }
            public string FreeAmount { get; set; }
            public string Price { get; set; }
            public string SubTotal { get; set; }
            public string Type { get; set; }
            public string RecDate { get; set; }
        }
        #endregion

        #region ----- Define Variables -----

        private Collection<PurchaseReturnRecord> PurchaseReturnRecordCollection { get; set; }

        private bool IsFirstStack { get; set; } = true;

        public DateTime sDateTime = new DateTime();

        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                sDateTime = value;
                NotifyPropertyChanged("SDateTime");
            }
        }
        public DateTime eDateTime = new DateTime();
        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                eDateTime = value;
                NotifyPropertyChanged("EDateTime");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public PurchaseReturnReportView()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ----- Search Report -----
        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button is null) return;

            if (button.Tag.ToString().Equals("ThisMonth"))
            {
                SDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                EDateTime = DateTime.Now;
            }

            if (!CheckDateTimeValid()) return;

            PurchaseReturnRecordCollection = StoreOrderDb.GetPurchaseReturnRecord(SDateTime, EDateTime);

            UpdateUi();
        }

        private bool CheckDateTimeValid()
        {
            if (StartDate.Text.Contains(" ") || EndDate.Text.Contains(" ")) return false;

            return true;
        }

        private void UpdateUi()
        {
            FirstStack.Children.Clear();
            SecondStack.Children.Clear();

            var ManList = PurchaseReturnRecordCollection.Select(r => r.Manufactory).Distinct().ToList();

            foreach (var man in ManList)
            {
                ManReportControl newManReportControl = new ManReportControl(PurchaseReturnRecordCollection.Where(r => r.Manufactory.Equals(man)).ToList(), OutsideScrollViewer);

                if (IsFirstStack)
                    FirstStack.Children.Add(newManReportControl);
                else
                    SecondStack.Children.Add(newManReportControl);

                IsFirstStack = !IsFirstStack;
            }
        }
        #endregion

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

        private void SetControlStatus_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if(button is null) return;

            ManReportControl.ControlStatus status = (ManReportControl.ControlStatus)Int16.Parse(button.Tag.ToString());

            foreach (var child in FirstStack.Children)
            {
                if (child is ManReportControl)
                    (child as ManReportControl).SetControlStatus(status);
            }

            foreach (var child in SecondStack.Children)
            {
                if (child is ManReportControl)
                    (child as ManReportControl).SetControlStatus(status);
            }
        }
    }
}
