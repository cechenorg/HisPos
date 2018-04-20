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
using His_Pos.Interface;

namespace His_Pos.Class.StoreOrder
{
    public class StoreOrder : INotifyPropertyChanged
    {
        public StoreOrder(User ordEmp, Manufactory.Manufactory manufactory, ObservableCollection<AbstractClass.Product> products = null)
        {
            Type = OrderType.UNPROCESSING;
            TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));

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

                Manufactory = new Manufactory.Manufactory(data[0], DataSource.MANUFACTORY);
            }
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
                        TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                        break;
                    case OrderType.PROCESSING:
                        TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
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
        public string RecEmp { get; set; }
        public Manufactory.Manufactory Manufactory{ get; set; }

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

            if (String.IsNullOrEmpty(Category.CategoryName))
                message += "請填寫處理單類別\n";

            if(Manufactory is null || Manufactory.Id is null)
                message += "請填寫廠商名稱\n";

            if (Products is null || Products.Count == 0)
                message += "請填寫商品\n";

            foreach (AbstractClass.Product product in Products)
            {
                if ( Math.Abs(((ITrade)product).Amount) <= 0)
                    message += "商品數量不能為0\n";
            }

            return message;
        }
    }
}
