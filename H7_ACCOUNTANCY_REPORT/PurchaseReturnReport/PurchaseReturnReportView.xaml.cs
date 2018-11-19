using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class.StoreOrder;

namespace His_Pos.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
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
        }


        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime before = now.AddYears(-1);

            PurchaseReturnRecordCollection = StoreOrderDb.GetPurchaseReturnRecord(before, now);

            UpdateUi();
        }

        private void UpdateUi()
        {
            ManReportControl newManReportControl = new ManReportControl(PurchaseReturnRecordCollection.Where(r => r.Manufactory.Equals("杏德")).ToList(), OutsideScrollViewer);

            FirstStack.Children.Add(newManReportControl);

            ManReportControl newManReportControl1 = new ManReportControl(PurchaseReturnRecordCollection.Where(r => r.Manufactory.Equals("NEW")).ToList(), OutsideScrollViewer);

            SecondStack.Children.Add(newManReportControl1);
        }
    }
}
