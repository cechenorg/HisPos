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
using His_Pos.Class.Product;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// OtcDetail.xaml 的互動邏輯
    /// </summary>
    public partial class OtcDetail : Window
    {
        private Otc otc;
        public OtcDetail(Otc o)
        {
            InitializeComponent();

            otc = o;
            UpdateUi();
        }

        private void UpdateUi()
        {
            if (otc is null) return;

            ProductName.Content = otc.Name;
            ProductId.Content = otc.Id;
        }
    }
}
