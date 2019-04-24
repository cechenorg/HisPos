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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    /// <summary>
    /// AdjustPharmacistWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AdjustPharmacistWindow : Window
    {
        private AdjustPharmacistViewModel adjustPharmacistViewModel { get; set; }
        public AdjustPharmacistWindow(DateTime declare)
        {
            adjustPharmacistViewModel = new AdjustPharmacistViewModel(declare);
            this.DataContext = adjustPharmacistViewModel;
            InitializeComponent();
            Show();
        }
    }
}
