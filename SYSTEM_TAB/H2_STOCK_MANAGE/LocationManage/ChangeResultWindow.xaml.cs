using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using His_Pos.Class;
using His_Pos.Class.Location;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// ChangeResultWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangeResultWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private ObservableCollection<ItemChangeWindow.ChangeItem> changeItems = new ObservableCollection<ItemChangeWindow.ChangeItem>();
        private ObservableCollection<ItemChangeTypeWindow.ChangeItem> changeItems1;

        public ObservableCollection<ItemChangeWindow.ChangeItem> ChangeItems {
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
        public ChangeResultWindow(ObservableCollection<ItemChangeWindow.ChangeItem> tempchangeItems)
        {
            InitializeComponent();
            DataContext = this;
            ChangeItems = tempchangeItems;
        }

        public ChangeResultWindow(ObservableCollection<ItemChangeTypeWindow.ChangeItem> changeItems1)
        {
            this.changeItems1 = changeItems1;
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in changeItems) {
                ///LocationDb.UpdateLocationDetail(item.id,item.newvalue);
            }
            MessageWindow.ShowMessage("更新成功!",MessageType.SUCCESS);
            
            Close();
        }
    }
}
