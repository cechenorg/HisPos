using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    /// <summary>
    /// NHIMedicineHistoryPriceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NHIMedicineHistoryPriceWindow : Window
    {
        #region ----- Define Variables -----
        public HistoryPrices HistoryPriceCollection { get; set; }
        #endregion

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
        #endregion
    }
}
