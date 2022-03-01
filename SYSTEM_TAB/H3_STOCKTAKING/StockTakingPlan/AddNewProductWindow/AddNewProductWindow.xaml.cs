using His_Pos.NewClass.StockTaking.StockTakingPlanProduct;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewProductWindow
{
    /// <summary>
    /// AddNewProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewProductWindow : Window
    {
        public AddNewProductWindow(string warID, StockTakingPlanProducts targetProducts)
        {
            InitializeComponent();
            AddNewProductWindowViewModel addNewProductWindowViewModel = new AddNewProductWindowViewModel(warID, targetProducts);
            DataContext = addNewProductWindowViewModel;
            ShowDialog();
        }
    }
}