using His_Pos.AbstractClass;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Class.Product;
using His_Pos.Interface;

namespace His_Pos.Class.StoreOrder
{
    public class StoreOrder : INotifyPropertyChanged, ICloneable
    {
        public StoreOrder(User ordEmp, Manufactory.Manufactory manufactory, ObservableCollection<AbstractClass.Product> products = null)
        {
            Type = OrderType.UNPROCESSING;
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\PosDot.png", UriKind.Relative));

            Id = StoreOrderDb.GetNewOrderId(ordEmp.Id);
            OrdEmp = ordEmp.Name;
            TotalPrice = "0";
            RecEmp = "";
            Category = new Category();

            Manufactory = (manufactory is null)? new Manufactory.Manufactory() : manufactory;

            Products = (products is null)? new ObservableCollection<AbstractClass.Product>() : products;
        }

        public StoreOrder(DataRow row)
        {
            Id = row["STOORD_ID"].ToString();
            switch (row["STOORD_FLAG"].ToString())
            {
                case "P":
                    Type = OrderType.UNPROCESSING;
                    break;
                case "G":
                    Type = OrderType.PROCESSING;
                    break;
            }

            Category = new Category(row["STOORD_TYPE"].ToString());
            OrdEmp = row["ORD_EMP"].ToString();
            TotalPrice = Double.Parse(row["TOTAL"].ToString()).ToString("0.##");
            RecEmp = row["REC_EMP"].ToString();
            if (row["MAN_ID"].ToString() == "")
                Manufactory = new Manufactory.Manufactory();
            else
            {
                var data = MainWindow.ManufactoryTable.Select("MAN_ID = '" + row["MAN_ID"].ToString() + "'");

                Manufactory = new Manufactory.Manufactory(data[0]);
            }
        }

        private StoreOrder()
        {
        }

        public BitmapImage typeIcon;

        public BitmapImage TypeIcon
        {
            get { return typeIcon; }
            set
            {
                typeIcon = value;
                NotifyPropertyChanged("TypeIcon");
            }
        }

        public OrderType type;
        public OrderType Type
        {
            get { return type; }
            set
            {
                type = value;
                switch (type)
                {
                    case OrderType.UNPROCESSING:
                        TypeIcon = new BitmapImage(new Uri(@"..\..\Images\PosDot.png", UriKind.Relative));
                        break;
                    case OrderType.PROCESSING:
                        TypeIcon = new BitmapImage(new Uri(@"..\..\Images\DarkerHisDot.png", UriKind.Relative));
                        break;
                }
            }
        }
        public Category Category { get; set; }
        public string Id { get; set; }
        public string OrdEmp { get; set; }
        public string totalPrice;
        public string TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                NotifyPropertyChanged("TotalPrice");
            }
        }
        private string recEmp;

        public string RecEmp
        {
            get { return recEmp; }
            set
            {
                recEmp = value;
                NotifyPropertyChanged("RecEmp");
            }
        }
        private Manufactory.Manufactory manufactory;
        public Manufactory.Manufactory Manufactory
        {
            get { return manufactory; }
            set
            {
                manufactory = value;
                NotifyPropertyChanged("Manufactory");
            }
        }

        public ObservableCollection<AbstractClass.Product> Products { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
      
        public string IsAnyDataEmpty()
        {
            string message = "";
            DateTime datetimevalue;
            if (String.IsNullOrEmpty(Category.CategoryName))
                message += "請填寫處理單類別\n";

            if(Manufactory is null || Manufactory.Id is null)
                message += "請填寫廠商名稱\n";

            if (Products is null || Products.Count == 0)
                message += "請填寫商品\n";

            if (type == OrderType.PROCESSING && String.IsNullOrEmpty(RecEmp))
                message += "請填寫收貨人\n";

            foreach (AbstractClass.Product product in Products)
            {
                if (type == OrderType.PROCESSING)
                {
                    if(product is ProductPurchaseMedicine && String.IsNullOrEmpty(((IProductPurchase)product).BatchNumber) )
                        message += "請填寫商品 " + product.Id + " 批號\n";

                    if (String.IsNullOrEmpty(((IProductPurchase)product).Invoice))
                        message += "請填寫商品 " + product.Id + " 發票號碼\n";

                    if (!DateTime.TryParse(((IProductPurchase)product).ValidDate, out datetimevalue))
                        message += "商品 " + product.Id + " 效期格式不正確\n";
                    
                }

                if ( Math.Abs(((ITrade)product).Amount) <= 0)
                    message += "商品 " + product.Id + " 數量不能為0\n";

                if( (Category.CategoryName.Equals("退貨") || Category.CategoryName.Equals("調貨")) && Math.Abs(((ITrade)product).Amount) > ((IProductPurchase)product).Stock.Inventory)
                    message += "商品 " + product.Id + " 數量不能超過庫存量\n";
               
                
            }

            return message;
        }

        public object Clone()
        {
            StoreOrder storeOrder = new StoreOrder();

            storeOrder.TypeIcon = TypeIcon;
            storeOrder.Type = Type;
            storeOrder.Category = Category;
            storeOrder.Id = Id;
            storeOrder.OrdEmp = OrdEmp;
            storeOrder.TotalPrice = TotalPrice;
            storeOrder.RecEmp = RecEmp;
            storeOrder.Manufactory = Manufactory;
            storeOrder.Products = new ObservableCollection<AbstractClass.Product>();

            foreach (AbstractClass.Product product in Products)
            {
                storeOrder.Products.Add(((ICloneable)product).Clone() as AbstractClass.Product);
            }

            return storeOrder;
        }
    }
}
