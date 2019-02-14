using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.ChooseBatchWindow
{
    /// <summary>
    /// ChooseBatchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChooseBatchWindow : Window
    {
        public ChooseBatchWindow(string iD)
        {
            InitializeComponent();

            ChooseBatchWindowViewModel chooseBatchWindowViewModel = new ChooseBatchWindowViewModel(iD);
            DataContext = chooseBatchWindowViewModel;

            if (chooseBatchWindowViewModel.ChooseBatchProductCollection.Count == 1)
                Close();
            else
                ShowDialog();
        }
    }
}
