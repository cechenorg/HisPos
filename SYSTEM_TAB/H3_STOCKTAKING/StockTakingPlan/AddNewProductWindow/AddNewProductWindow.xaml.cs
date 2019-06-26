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

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow {
    /// <summary>
    /// AddNewProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewProductWindow : Window {
        public AddNewProductWindow(string warID) {
            InitializeComponent();
            AddNewProductWindowViewModel addNewProductWindowViewModel = new AddNewProductWindowViewModel(warID);
            DataContext = addNewProductWindowViewModel;
            ShowDialog();
        }
    }
}
