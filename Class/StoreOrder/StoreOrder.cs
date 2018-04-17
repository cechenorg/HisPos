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
    public class StoreOrder
    {
        public StoreOrder(User ordEmp, Manufactory.Manufactory manufactory, ObservableCollection<AbstractClass.Product> products = null)
        {
            
            Type = OrderType.UNPROCESSING;
            TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));

            Id = StoreOrderDb.GetNewOrderId(ordEmp.Id);
            OrdEmp = ordEmp.Name;
            TotalPrice = "0";
            RecEmp = "";
            Category = "";

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
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                    break;
                case "G":
                    Type = OrderType.PROCESSING;
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                    break;
            }
            switch (row["STOORD_TYPE"].ToString())
            {
                case "進":
                    CategoryColor = "Green";
                    Category = "進貨";
                    break;
                case "退":
                    CategoryColor = "Red";
                    Category = "退貨";
                    break;
                case "調":
                    CategoryColor = "Blue";
                    Category =  "調貨";
                    break;
                default:
                    Category = "";
                    break;
            }
           
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
        public BitmapImage TypeIcon { get; set; }
        public OrderType Type { get; set; }
        public string Category { get; set; }
        public string CategoryColor { get; set; }
        public string Id { get; set; }
        public string OrdEmp { get; set; }
        public string TotalPrice { get; set; }
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

            if (String.IsNullOrEmpty(Category))
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
