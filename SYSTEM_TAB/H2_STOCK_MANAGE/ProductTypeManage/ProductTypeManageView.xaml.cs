using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.ProductType;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// ProductTypeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTypeManageView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        public string[] Months { get; set; }
        public string[] Days { get; set; }

        private ObservableCollection<ProductTypeManageMaster> typeManageMasters = new ObservableCollection<ProductTypeManageMaster>();

        public ObservableCollection<ProductTypeManageMaster> TypeManageMasters
        {
            get { return typeManageMasters; }
            set
            {
                typeManageMasters = value;
                NotifyPropertyChanged("TypeManageMasters");
            }
        }

        private ObservableCollection<ProductTypeManageDetail> typeManageDetails = new ObservableCollection<ProductTypeManageDetail>();

        public ObservableCollection<ProductTypeManageDetail> TypeManageDetails
        {
            get { return typeManageDetails; }
            set
            {
                typeManageDetails = value;
                NotifyPropertyChanged("TypeManageDetails");
            }
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                NotifyPropertyChanged("Products");
            }
        }

        //public SeriesCollection StockValuePieSeries { get; set; } = new SeriesCollection();
        //public SeriesCollection SalesPieSeries { get; set; } = new SeriesCollection();

        //private SeriesCollection salesLineSeries = new SeriesCollection();
        //public SeriesCollection SalesLineSeries
        //{
        //    get { return salesLineSeries; }
        //    set
        //    {
        //        salesLineSeries = value;
        //        NotifyPropertyChanged("SalesLineSeries");
        //    }
        //}

        //public LineSeries YearSalesLineSeries { get; set; }
        //public LineSeries LastYearSalesLineSeries { get; set; }

        //public LineSeries MonthSalesLineSeries { get; set; }
        //public LineSeries LastMonthSalesLineSeries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public ProductTypeManageView()
        {
            InitializeComponent();

            DataContext = this;

            //LoadingWindow loadingWindow = new LoadingWindow();
            //loadingWindow.InitProductType(this);
            //loadingWindow.Show();
            //loadingWindow.Topmost = true;
        }

        public void InitMonthsAndDays()
        {
            DateTime today = DateTime.Today.Date;

            Months = new string[12];
            for (int x = 0; x < 12; x++)
            {
                Months[x] = DateTime.Today.Year.ToString() + "/" + (x + 1).ToString();
            }

            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

            Days = new string[daysInMonth];
            for (int x = 0; x < daysInMonth; x++)
            {
                Days[x] = DateTime.Today.Month.ToString() + "/" + (x + 1).ToString();
            }
        }

        public void InitPieCharts()
        {
            foreach (var productType in TypeManageMasters)
            {
                //if (productType.StockValue != 0)
                //{
                //    PieSeries newStockPieSeries = new PieSeries()
                //    {
                //        Title = productType.Name,
                //        Values = new ChartValues<double> { productType.StockValue },
                //        DataLabels = true
                //    };

                //    StockValuePieSeries.Add(newStockPieSeries);
                //}

                //if (productType.Sales != 0)
                //{
                //    PieSeries newSalesPieSeries = new PieSeries()
                //    {
                //        Title = productType.Name,
                //        Values = new ChartValues<double> { productType.Sales },
                //        DataLabels = true
                //    };

                //    SalesPieSeries.Add(newSalesPieSeries);
                //}
            }
        }

        public void InitTypes()
        {
            TypeManageMasters.Clear();
            TypeManageDetails.Clear();

            ///ProductDb.GetProductTypeManage(TypeManageMasters, TypeManageDetails);
        }

        private void TypeMaster_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) is null || (sender as DataGrid).SelectedItem is null) return;

            TypeDetail.Items.Filter = item => ((ProductTypeManageDetail)item).Parent == ((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Id;

            BigType.Text = ((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Name;
            BigTypeEngName.Text = ((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).EngName;

            PieChartPushOut();
            InitLineChart(((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Id);
            UpdateDetailUi();

            InitTypeDataChanged();
        }

        private void UpdateDetailUi()
        {
            if (TypeDetail.Items.Count != 0)
            {
                TypeDetail.SelectedIndex = 0;

                SmallType.IsEnabled = true;
                SmallTypeEngName.IsEnabled = true;
            }
            else
            {
                SmallType.Text = "";
                SmallTypeEngName.Text = "";

                SmallType.IsEnabled = false;
                SmallTypeEngName.IsEnabled = false;
            }
        }

        private void InitLineChart(string typeId)
        {
            //SalesLineSeries.Clear();
            //YearSalesLineSeries = new LineSeries() { Title = "今年", Stroke = Brushes.RoyalBlue };
            //LastYearSalesLineSeries = new LineSeries() { Title = "去年", Stroke = Brushes.Red };
            //MonthSalesLineSeries = new LineSeries() { Title = "本月", Stroke = Brushes.RoyalBlue };
            //LastMonthSalesLineSeries = new LineSeries() { Title = "上個月", Stroke = Brushes.Red };

            ///ProductDb.GetProductTypeLineSeries(YearSalesLineSeries, LastYearSalesLineSeries, MonthSalesLineSeries, LastMonthSalesLineSeries, typeId);

            UpdateLineChartUi();
           
        }

        private void UpdateLineChartUi()
        {
            //SalesLineSeries.Clear();

            //if ((bool) MonthRadioButton.IsChecked)
            //{
            //    LineChartAxis.Labels = Days;

            //    if((bool)ThisYearLine.IsChecked)
            //        SalesLineSeries.Add(MonthSalesLineSeries);

            //    if ((bool)LastYearLine.IsChecked)
            //        SalesLineSeries.Add(LastMonthSalesLineSeries);
            //}
            //else
            //{
            //    LineChartAxis.Labels = Months;

            //    if ((bool)ThisYearLine.IsChecked)
            //        SalesLineSeries.Add(YearSalesLineSeries);

            //    if ((bool)LastYearLine.IsChecked)
            //        SalesLineSeries.Add(LastYearSalesLineSeries);
            //}
        }

        private void PieChartPushOut()
        {
            //foreach (PieSeries pieSeries in StockValuePieSeries)
            //{
            //    if (pieSeries.Title.Equals(((ProductTypeManageMaster) TypeMaster.SelectedItem).Name))
            //        pieSeries.PushOut = 20;
            //    else
            //    {
            //        pieSeries.PushOut = 0;
            //    }
            //}

            //foreach (PieSeries pieSeries in SalesPieSeries)
            //{
            //    if (pieSeries.Title.Equals(((ProductTypeManageMaster)TypeMaster.SelectedItem).Name))
            //        pieSeries.PushOut = 20;
            //    else
            //    {
            //        pieSeries.PushOut = 0;
            //    }
            //}
        }

        private void TypeDetail_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) is null || (sender as DataGrid).SelectedItem is null) return;

            ProductsGrid.Items.Filter = item => ((IProductType)item).TypeId == ((ProductTypeManageDetail)(sender as DataGrid).SelectedItem).Id;

            SmallType.Text = ((ProductTypeManageDetail) (sender as DataGrid).SelectedItem).Name;
            SmallTypeEngName.Text = ((ProductTypeManageDetail)(sender as DataGrid).SelectedItem).EngName;

            InitTypeDataChanged();
        }

        private void LineChartRange_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLineChartUi();
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            Product newProduct = new Product();/// ProductDb.GetProductById(((Product) ProductsGrid.SelectedItem).Id);

            if (ProductDetail.Instance is null)
            {
                ProductDetail newProductDetail = new ProductDetail();
                newProductDetail.Show();
            }

            ProductDetail.Instance.AddNewTab(newProduct);
            ProductDetail.Instance.Focus();
        }

        private void CancelTypeChange(object sender, RoutedEventArgs e)
        {
            if (TypeDetail.SelectedItem != null)
            {
                BigType.Text = ((ProductTypeManageMaster)TypeMaster.SelectedItem).Name;
                BigTypeEngName.Text = ((ProductTypeManageMaster)TypeMaster.SelectedItem).EngName;

                SmallType.Text = ((ProductTypeManageDetail)TypeDetail.SelectedItem).Name;
                SmallTypeEngName.Text = ((ProductTypeManageDetail)TypeDetail.SelectedItem).EngName;
            }

            InitTypeDataChanged();
        }

        private void ConfirmTypeChange(object sender, RoutedEventArgs e)
        {
            if(CheckValidChange())
            {
                ((ProductTypeManageMaster)TypeMaster.SelectedItem).Name = BigType.Text;
                ((ProductTypeManageMaster)TypeMaster.SelectedItem).EngName = BigTypeEngName.Text;
                
                ///ProductDb.UpdateProductType(((ProductTypeManageMaster)TypeMaster.SelectedItem).Id, BigType.Text, BigTypeEngName.Text);

                if (TypeDetail.Items.Count != 0)
                {
                    ((ProductTypeManageDetail)TypeDetail.SelectedItem).Name = SmallType.Text;
                    ((ProductTypeManageDetail)TypeDetail.SelectedItem).EngName = SmallTypeEngName.Text;

                    ///ProductDb.UpdateProductType(((ProductTypeManageDetail)TypeDetail.SelectedItem).Id, SmallType.Text, SmallTypeEngName.Text);
                }
                
                MessageWindow.ShowMessage("修改成功!", MessageType.SUCCESS);
                

                InitTypeDataChanged();
            }
        }

        private bool CheckValidChange()
        {
            string error = "";

            if (BigTypeEngName.Text.Length != 2 || (SmallTypeEngName.Text.Length != 2 && TypeDetail.Items.Count != 0))
                error += "英文簡碼須為2碼!";

            if (BigTypeEngName.Text.Equals("") || (SmallTypeEngName.Text.Equals("") && TypeDetail.Items.Count != 0))
                error += "名稱不可為空!";

            if (BigType.Text.Equals("") || (SmallType.Text.Equals("") && TypeDetail.Items.Count != 0))
                error += "名稱不可為空!";

            if (error.Length == 0)
                return true;
            else
            {
                MessageWindow.ShowMessage(error, MessageType.ERROR);
                

                return false;
            }
        }

        private void DeleteTypeClick(object sender, RoutedEventArgs e)
        {
            if ((ProductTypeManageMaster)TypeMaster.SelectedItem is null) return;

            DeleteTypeWindow deleteTypeWindow = new DeleteTypeWindow((ProductTypeManageMaster)TypeMaster.SelectedItem, (ProductTypeManageDetail)TypeDetail.SelectedItem);
            deleteTypeWindow.ShowDialog();

            if(CheckDeletable(deleteTypeWindow.DeleteType))
            {
                if(deleteTypeWindow.DeleteType is ProductTypeManageMaster)
                {
                    ///ProductDb.DeleteProductType(((ProductTypeManageMaster)deleteTypeWindow.DeleteType).Id);
                    TypeManageMasters.Remove((ProductTypeManageMaster)deleteTypeWindow.DeleteType);
                    
                    for( int x = 0; x < TypeDetail.Items.Count; x++ )
                    {
                        ///ProductDb.DeleteProductType((TypeDetail.Items[x] as ProductTypeManageDetail).Id);
                        TypeManageDetails.Remove(TypeDetail.Items[x] as ProductTypeManageDetail);
                    }

                    TypeMaster.SelectedIndex = 0;
                    TypeMaster.ScrollIntoView(TypeMaster.SelectedItem);
                }
                else
                {
                    ((ProductTypeManageMaster)TypeMaster.SelectedItem).TypeCount--;
                    ///ProductDb.DeleteProductType(((ProductTypeManageDetail)deleteTypeWindow.DeleteType).Id);
                    TypeManageDetails.Remove((ProductTypeManageDetail)deleteTypeWindow.DeleteType);
                    UpdateDetailUi();
                }
            }
            
        }

        private bool CheckDeletable(ProductType deleteType)
        {
            if (deleteType != null)
            {
                if (deleteType is ProductTypeManageMaster)
                {
                    foreach (ProductTypeManageDetail detail in TypeDetail.Items)
                    {
                        if (detail.ItemCount != 0)
                        {
                            MessageWindow.ShowMessage((deleteType as ProductTypeManageMaster).Name + "以下類別之商品數須為0!", MessageType.ERROR);
                            
                            return false;
                        }
                    }
                }
                else
                {
                    if ((deleteType as ProductTypeManageDetail).ItemCount != 0)
                    {
                        MessageWindow.ShowMessage((deleteType as ProductTypeManageDetail).Name + "之商品數須為0!", MessageType.ERROR);
                        
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void AddTypeClick(object sender, RoutedEventArgs e)
        {
            AddTypeWindow addTypeWindow = new AddTypeWindow(TypeManageMasters, (TypeMaster.SelectedItem as ProductTypeManageMaster));

            addTypeWindow.ShowDialog();

            if(addTypeWindow.newProductType != null)
            {
                if(addTypeWindow.newProductType is ProductTypeManageMaster)
                {
                    TypeManageMasters.Add(addTypeWindow.newProductType as ProductTypeManageMaster);
                    TypeMaster.SelectedItem = addTypeWindow.newProductType;
                    TypeMaster.ScrollIntoView(TypeMaster.SelectedItem);
                }
                else
                {
                    (TypeMaster.SelectedItem as ProductTypeManageMaster).TypeCount++;

                    TypeManageDetails.Add(addTypeWindow.newProductType as ProductTypeManageDetail);
                    UpdateDetailUi();
                    TypeDetail.SelectedItem = addTypeWindow.newProductType;
                }
            }
        }

        private void ButtonTypeChange_Click(object sender, RoutedEventArgs e)
        {
            ItemChangeTypeWindow itemChangeTypeWindow = new ItemChangeTypeWindow();
            itemChangeTypeWindow.ShowDialog();
        }

        private void Type_TextChanged(object sender, TextChangedEventArgs e)
        {
            TypeDataChanged();
        }

        private void TypeDataChanged()
        {
            CancelBtn.IsEnabled = true;
            ConfirmBtn.IsEnabled = true;

            ChangedLabel.Foreground = Brushes.Red;
            ChangedLabel.Content = "已修改";
        }

        private void InitTypeDataChanged()
        {
            CancelBtn.IsEnabled = false;
            ConfirmBtn.IsEnabled = false;

            ChangedLabel.Foreground = Brushes.DimGray;
            ChangedLabel.Content = "未修改";
        }
    }
}
