using His_Pos.NewClass.Product.ProductManagement;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    /// <summary>
    /// NHIMedicineHistoryPriceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NHIMedicineHistoryPriceWindow : Window
    {
        #region ----- Define Variables -----

        public HistoryPrices HistoryPriceCollection { get; set; }

        #endregion ----- Define Variables -----

        public NHIMedicineHistoryPriceWindow(string medicineID)
        {
            InitializeComponent();
            InitData(medicineID);

            DataContext = this;
        }

        #region ----- Define Functions -----

        private void InitData(string medicineID)
        {
            HistoryPriceCollection = HistoryPrices.GetHistoryPrices(medicineID);
        }

        #endregion ----- Define Functions -----
    }
}