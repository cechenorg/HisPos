using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.ProductType;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// ItemChangeTypeWindow.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class ItemChangeTypeWindow : Window
    {
       public  ObservableCollection<ProductTypeManageMaster> typeManageSourceMasters = new ObservableCollection<ProductTypeManageMaster>();
        public ObservableCollection<ProductTypeManageMaster> TypeManageSourceMasters
        {
            get
            {
                return typeManageSourceMasters;
            }
            set
            {
                typeManageSourceMasters = value;
                NotifyPropertyChanged("TypeManageSourceMasters");
            }
        }
        public ObservableCollection<ProductTypeManageDetail> typeManageSourceDetails = new ObservableCollection<ProductTypeManageDetail>();
        public ObservableCollection<ProductTypeManageDetail> TypeManageSourceDetails
        {
            get
            {
                return typeManageSourceDetails;
            }
            set
            {
                typeManageSourceDetails = value;
                NotifyPropertyChanged("TypeManageSourceDetails");
            }
        }
        public ObservableCollection<ProductTypeManageMaster> typeManageTargetMasters = new ObservableCollection<ProductTypeManageMaster>();
        public ObservableCollection<ProductTypeManageMaster> TypeManageTargetMasters
        {
            get
            {
                return typeManageTargetMasters;
            }
            set
            {
                typeManageTargetMasters = value;
                NotifyPropertyChanged("TypeManageTargetMasters");
            }
        }
        public ObservableCollection<ProductTypeManageDetail> typeManageTargetDetails = new ObservableCollection<ProductTypeManageDetail>();
        public ObservableCollection<ProductTypeManageDetail> TypeManageTargetDetails
        {
            get
            {
                return typeManageTargetDetails;
            }
            set
            {
                typeManageTargetDetails = value;
                NotifyPropertyChanged("TypeManageTargetDetails");
            }
        }
        public class TypeData
        {
            public TypeData(DataRow row)
            {
                proid = row["PRO_ID"].ToString();
                proname = row["PRO_NAME"].ToString();
                typeid = row["PROTYP_ID"].ToString();
                typeName = row["PROTYP_CHINAME"].ToString();
            }
            public string proid { get; set; }
            public string proname { get; set; }
            public string typeid { get; set; }
            public string typeName { get; set; }
        }
        private ObservableCollection<ChangeItem> changeItems = new ObservableCollection<ChangeItem>();
        private ObservableCollection<TypeData> typeDataSourceDatas = new ObservableCollection<TypeData>();
        public ObservableCollection<TypeData> TypeDataSourceDatas
        {
            get
            {
                return typeDataSourceDatas;
            }
            set
            {
                typeDataSourceDatas = value;
                NotifyPropertyChanged("TypeDataSourceDatas");
            }
        }
        private ObservableCollection<TypeData> typeDataTargetDatas = new ObservableCollection<TypeData>();
        public ObservableCollection<TypeData> TypeDataTargetDatas
        {
            get
            {
                return typeDataTargetDatas;
            }
            set
            {
                typeDataTargetDatas = value;
                NotifyPropertyChanged("TypeDataTargetDatas");
            }
        }
        public ItemChangeTypeWindow()
        {
            InitializeComponent();
            DataContext = this;
            ///ProductDb.GetProductTypeManage(TypeManageSourceMasters, TypeManageSourceDetails);
            ComboBoxSourceBig.ItemsSource = TypeManageSourceMasters;
            ComboBoxSourceSmall.ItemsSource = TypeManageSourceDetails;
            ///ProductDb.GetProductTypeManage(TypeManageTargetMasters, TypeManageTargetDetails);
            ComboBoxTargetBig.ItemsSource = TypeManageTargetMasters;
            ComboBoxTargetSmall.ItemsSource = TypeManageTargetDetails;
            foreach (var row in "")///LocationDb.GetProductLocation().Rows)
            {
                ///TypeDataSourceDatas.Add(new TypeData(row));
                ///TypeDataTargetDatas.Add(new TypeData(row));
            }
            DataGridTarget.Items.Filter = ProductTypeTargetFilter;
            DataGridSource.Items.Filter = ProductTypeSourceFilter;

        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public class ChangeItem
        {
            public ChangeItem(string cid, string cname, string coldvalue, string cnewvalue)
            {
                id = cid;
                name = cname;
                oldvalue = coldvalue;
                newvalue = cnewvalue;
            }
            public string id { get; set; }
            public string name { get; set; }
            public string oldvalue { get; set; }
            public string newvalue { get; set; }
        }
       
       

        private void ComboBoxSourceBig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSourceBig.SelectedValue is null) return;
            ComboBoxSourceSmall.Items.Filter = TypeSourceFilter;
        }

        private void ComboBoxSourceSmall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxSourceBig.SelectedValue is null))
                ComboBoxSourceBig.Text = TypeManageSourceMasters.Single(x => x.Id == ((ProductTypeManageDetail)ComboBoxSourceSmall.SelectedItem).Parent).Name.ToString();
            DataGridSource.Items.Filter = ProductTypeSourceFilter;

        }
        private void ComboBoxTargetBig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTargetBig.SelectedValue is null) return;

            ComboBoxTargetSmall.Items.Filter = TypeTargetFilter;
        }

        private void ComboBoxTargetSmall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxTargetBig.SelectedValue is null))
                ComboBoxTargetBig.Text = TypeManageTargetMasters.Single(x => x.Id == ((ProductTypeManageDetail)ComboBoxTargetSmall.SelectedItem).Parent).Name.ToString();

            DataGridTarget.Items.Filter = ProductTypeTargetFilter;
        }
        private void ButtonPlus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridSource.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇項目", MessageType.ERROR);
                
                return;
            }
            if (ComboBoxSourceSmall.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇來源", MessageType.ERROR);
                
                return;
            }
            if (ComboBoxTargetSmall.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇目的", MessageType.ERROR);
                
                return;
            }

            foreach (TypeData item in DataGridSource.SelectedItems)
            {
                changeItems.Add(new ChangeItem(item.proid, item.proname, item.typeName, ((ProductTypeManageDetail)ComboBoxTargetSmall.SelectedItem).Name));
                item.typeName = ((ProductTypeManageDetail)ComboBoxTargetSmall.SelectedItem).Name;
                TypeDataTargetDatas.Single(product => product.proid == item.proid).typeName = ((ProductTypeManageDetail)ComboBoxTargetSmall.SelectedItem).Name;
            }
            
            DataGridTarget.Items.Filter = ProductTypeTargetFilter;
            DataGridSource.Items.Filter = ProductTypeSourceFilter;
            DataGridTarget.SelectedIndex = 0;
            DataGridSource.SelectedIndex = 0;
        }

        private void ButtonBalance_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridTarget.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇項目", MessageType.ERROR);
                
                return;
            }
            if (ComboBoxTargetSmall.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇來源", MessageType.ERROR);
                
                return;
            }
            if (ComboBoxSourceSmall.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇目的", MessageType.ERROR);
                
                return;
            }

            foreach (TypeData item in DataGridTarget.SelectedItems)
            {
                changeItems.Add(new ChangeItem(item.proid, item.proname, item.typeName, ((ProductTypeManageDetail)ComboBoxSourceSmall.SelectedItem).Name));
                item.typeName = ((ProductTypeManageDetail)ComboBoxSourceSmall.SelectedItem).Name;
                TypeDataSourceDatas.Single(product => product.proid == item.proid).typeName = ((ProductTypeManageDetail)ComboBoxSourceSmall.SelectedItem).Name;
            }
            
            DataGridTarget.Items.Filter = ProductTypeTargetFilter;
            DataGridSource.Items.Filter = ProductTypeSourceFilter;
            DataGridTarget.SelectedIndex = 0;
            DataGridSource.SelectedIndex = 0;
        }
        public bool ProductTypeSourceFilter(object item)
        {
            if (ComboBoxSourceSmall.SelectedItem is null) return false;
            if (((TypeData)item).typeName.Equals(((ProductTypeManageDetail)ComboBoxSourceSmall.SelectedItem).Name))
                return true;
            else 
                return false;
        }
        public bool ProductTypeTargetFilter(object item)
        {
            if (ComboBoxTargetSmall.SelectedItem is null) return false;
            if (((TypeData)item).typeName.Equals(((ProductTypeManageDetail)ComboBoxTargetSmall.SelectedItem).Name))
                return true;
            else
                return false;
        }
        public bool TypeSourceFilter(object item)
        {
            if ( ((ProductTypeManageDetail)item).Parent.Equals(((ProductTypeManageMaster)ComboBoxSourceBig.SelectedItem).Id))
                return true;
            else
                return false;
        }
        public bool TypeTargetFilter(object item)
        {
            if (((ProductTypeManageDetail)item).Parent.Equals(((ProductTypeManageMaster)ComboBoxTargetBig.SelectedItem).Id))
                return true;
            else
                return false;
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeTypeResultWindow changeResultWindow = new ChangeTypeResultWindow(changeItems);
            changeResultWindow.ShowDialog();
        }
    }
}
