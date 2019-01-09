using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using His_Pos.Class.Declare;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase
{
    /// <summary>
    /// DeclareDataDetailOverview.xaml 的互動邏輯
    /// </summary>
    public partial class DeclareDataDetailOverview : Window
    {
        public struct PurchaseDeclareDataOverview
        {
            public PurchaseDeclareDataOverview(DataRow dataRow)
            {
                DeclareId = dataRow["HISDECMAS_ID"].ToString();
                CustomerName = dataRow["CUS_NAME"].ToString();
                TreatmentDate = dataRow["HISDECMAS_TREATDATE"].ToString();
                Division = dataRow["HISDIV_NAME"].ToString();
                InstitutionName = dataRow["INS_NAME"].ToString();
            }

            public string DeclareId { get; }
            public string CustomerName { get; }
            public string TreatmentDate { get; }
            public string Division { get; }
            public string InstitutionName { get; }
        }

        public ObservableCollection<PurchaseDeclareDataOverview> PurchaseDeclareDataOverviews { get; set; }

        public DeclareDataDetailOverview(string storeOrderId)
        {
            InitializeComponent();
            DataContext = this;

            PurchaseDeclareDataOverviews = DeclareDb.GetPurchaseDeclareDataOverviews(storeOrderId);
        }
    }
}
