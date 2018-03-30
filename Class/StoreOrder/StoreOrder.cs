using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.StoreOrder
{
    public class StoreOrder
    {
        public StoreOrder(User ordEmp)
        {
            Type = OrderType.PURCHASE;
            TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));

            Id = StoreOrderDb.GetNewOrderId(ordEmp.Id);
            OrdEmp = ordEmp.Name;
            TotalPrice = "0";
            RecEmp = "";
            Manufactory = new Manufactory.Manufactory();

            Products = new ObservableCollection<AbstractClass.Product>();
        }

        public StoreOrder(string type, string id, string ordEmp,double total, string recEmp, string ManId)
        {
            switch (type)
            {
                case "P":
                    Type = OrderType.PURCHASE;
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                    break;
                case "G":
                    Type = OrderType.GET;
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                    break;
            }
            Id = id;
            OrdEmp = ordEmp;
            TotalPrice = total.ToString("0.##");
            RecEmp = recEmp;

            if(ManId == "")
                Manufactory = new Manufactory.Manufactory();
            else
            {
                var data = MainWindow.ManufactoryTable.Select("MAN_ID = '" + ManId + "'");

                Manufactory = new Manufactory.Manufactory(data[0]);
            }
        }

        public BitmapImage TypeIcon { get; set; }
        public OrderType Type { get; set; }
        public string Id { get; set; }
        public string OrdEmp { get; set; }
        public string TotalPrice { get; set; }
        public string RecEmp { get; set; }
        public Manufactory.Manufactory Manufactory{ get; set; }
        public ObservableCollection<AbstractClass.Product> Products { get; set; }
    }
}
