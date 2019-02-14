using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// ChangeTypeResultWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangeTypeResultWindow : Window,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private ObservableCollection<ItemChangeTypeWindow.ChangeItem> changeItems = new ObservableCollection<ItemChangeTypeWindow.ChangeItem>();

        public ObservableCollection<ItemChangeTypeWindow.ChangeItem> ChangeItems
        {
            get
            {
                return changeItems;
            }
            set
            {
                changeItems = value;
                NotifyPropertyChanged("ChangeItems");
            }
        }
        public ChangeTypeResultWindow(ObservableCollection<ItemChangeTypeWindow.ChangeItem> tempchangeItems)
        {
            InitializeComponent();
            DataContext = this;
            ChangeItems = tempchangeItems;
        }

   
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in changeItems)
            {
            ///ProductDb.ChangeProductType(item.id, item.newvalue);
            }
            MessageWindow.ShowMessage("更新成功!", MessageType.SUCCESS);
            
            Close();
        }
    }
}
