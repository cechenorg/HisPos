using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryMaster
    {
        public CustomerHistoryMaster(SystemType type, string date, string customerHistoryDetailId, string customerHistoryData)
        {
            Type = type;
            Date = date;
            CustomerHistoryDetailId = customerHistoryDetailId;
            CustomerHistoryData = customerHistoryData;

            switch (Type)
            {
                case SystemType.POS:
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                    break;
                case SystemType.HIS:
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                    break;
            }
        }

        public BitmapImage TypeIcon { get; }
        public SystemType Type { get; }
        public string Date { get; }
        public string CustomerHistoryDetailId { get; }
        public string CustomerHistoryData { get; }

    }
}
