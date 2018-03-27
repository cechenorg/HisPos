using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.StoreOrder
{
    public class StoreOrderOverview
    {
        public StoreOrderOverview(string type, string id, string ordEmp,double total, string recEmp, string ManId)
        {
            switch (type)
            {
                case "P":
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                    break;
                case "G":
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                    break;
            }
            Id = id;
            OrdEmp = ordEmp;
            TotalPrice = total.ToString("#.##");
            RecEmp = recEmp;

            var data = MainWindow.ManufactoryTable.Select("MAN_ID = '" + ManId + "'");

            Manufactory = new Manufactory.Manufactory(data[0]);
        }

        public BitmapImage TypeIcon { get; set; }
        public string Id { get; set; }
        public string OrdEmp { get; set; }
        public string TotalPrice { get; set; }
        public string RecEmp { get; set; }
        public Manufactory.Manufactory Manufactory{ get; set; }
    }
}
