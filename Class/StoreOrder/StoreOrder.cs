using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
    }
}
