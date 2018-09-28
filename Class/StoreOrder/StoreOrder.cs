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
using His_Pos.Struct.Manufactory;

namespace His_Pos.Class.StoreOrder
{
    public class StoreOrder : INotifyPropertyChanged, ICloneable
    {
        public StoreOrder(StoreOrderCategory category, User ordEmp, WareHouse wareHouse, Manufactory.Manufactory manufactory, ObservableCollection<AbstractClass.Product> products = null, string note = "")
        {
            Type = OrderType.UNPROCESSING;
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\OrangeDot.png", UriKind.Relative));

            Category = new Category(category);

            Id = StoreOrderDb.GetNewOrderId(ordEmp.Id, wareHouse.Id, manufactory.Id, Category.CategoryName.Substring(0,1));
            OrdEmp = ordEmp.Name;
            TotalPrice = "0";
            RecEmp = "";
            Warehouse = wareHouse;
            Principal = new PurchasePrincipal("");
            DeclareDataCount = 0;

            Manufactory = (manufactory is null)? new Manufactory.Manufactory() : manufactory;

            Products = (products is null)? new ObservableCollection<AbstractClass.Product>() : products;

            Note = note;
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
                case "W":
                    Type = OrderType.WAITING;
                    break;
            }

            Category = new Category(row["STOORD_TYPE"].ToString().Equals("進")? StoreOrderCategory.PURCHASE : StoreOrderCategory.RETURN);
            OrdEmp = row["ORD_EMP"].ToString();
            initProductCount = Int32.Parse(row["TOTAL"].ToString());
            RecEmp = row["REC_EMP"].ToString();
            Manufactory = new Manufactory.Manufactory(row);
            Principal = new PurchasePrincipal(row);
            TotalPrice = "0";
            Warehouse = new WareHouse(row);

            DeclareDataCount = Int32.Parse(row["DECLARECOUNT"].ToString());

            Note = row["STOORD_NOTE"].ToString();
        }

        private StoreOrder()
        {
        }

        private int initProductCount;
        public int ProductCount
        {
            get
            {
                if (Products is null)
                    return initProductCount;
                else
                {
                    return Products.Count;
                }
            }
        }

        public int DeclareDataCount { get; set; }

        public bool IsDataChanged { get; set; } = false;

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
                        TypeIcon = new BitmapImage(new Uri(@"..\..\Images\OrangeDot.png", UriKind.Relative));
                        break;
                    case OrderType.PROCESSING:
                        TypeIcon = new BitmapImage(new Uri(@"..\..\Images\BlueDot.png", UriKind.Relative));
                        break;
                    case OrderType.WAITING:
                        TypeIcon = new BitmapImage(new Uri(@"..\..\Images\BlueRingDot.png", UriKind.Relative));
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

        private PurchasePrincipal principal;
        public PurchasePrincipal Principal
        {
            get { return principal; }
            set
            {
                principal = value;
                NotifyPropertyChanged("Principal");
            }
        }

        public WareHouse Warehouse { get; set; }
        public string Note { get; set; }
        private ObservableCollection<AbstractClass.Product> products;

        public ObservableCollection<AbstractClass.Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                products.CollectionChanged += (s, a) => NotifyPropertyChanged("ProductCount");
            }
        }

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

            if (Products is null || Products.Count == 0)
                message += "請填寫商品\n";

            foreach (AbstractClass.Product product in Products)
            {
                if (type == OrderType.PROCESSING)
                {
                    //if(product is ProductPurchaseMedicine && String.IsNullOrEmpty(((IProductPurchase)product).BatchNumber) )
                    //    message += "請填寫商品 " + product.Id + " 批號\n";

                    //if (String.IsNullOrEmpty(((IProductPurchase)product).Invoice))
                    //    message += "請填寫商品 " + product.Id + " 發票號碼\n";

                    //if (!DateTime.TryParse(((IProductPurchase)product).ValidDate, out datetimevalue))
                    //    message += "商品 " + product.Id + " 效期格式不正確\n";
                }
                else if (type == OrderType.UNPROCESSING && Category.CategoryName.Equals("進貨"))
                {
                    if (((IProductPurchase)product).OrderAmount == 0)
                        message += "商品 " + product.Id + " 預定量不能為0\n";
                }
                else if (type == OrderType.UNPROCESSING && Category.CategoryName.Equals("退貨"))
                {
                    if (((ITrade)product).Amount == 0)
                        message += "商品 " + product.Id + " 退貨量不能為0\n";
                    else if (((ITrade)product).Amount > ((IProductReturn)product).BatchLimit)
                        message += "商品 " + product.Id + " 退貨量不能高於庫存量\n";
                }
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
            storeOrder.Principal = Principal;
            storeOrder.Warehouse = Warehouse;
            storeOrder.Note = Note;

            foreach (AbstractClass.Product product in Products)
            {
                storeOrder.Products.Add(((ICloneable)product).Clone() as AbstractClass.Product);
            }

            return storeOrder;
        }

        internal void CalculateTotalPrice()
        {
            double count = 0;
            foreach (var product in Products)
            {
                count += ((ITrade)product).TotalPrice;
            }

            TotalPrice = Math.Round(count, MidpointRounding.AwayFromZero).ToString();
        }

        internal bool CheckIfOrderNotComplete()
        {
            foreach(var product in Products)
            {
                if (((IProductPurchase)product).OrderAmount > ((ITrade)product).Amount)
                    return true;
            }

            return false;
        }
    }
}
