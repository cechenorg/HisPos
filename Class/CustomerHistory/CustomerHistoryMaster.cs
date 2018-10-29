using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryMaster : INotifyPropertyChanged
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
                    TypeIcon = new BitmapImage(new Uri(@"..\..\Images\OrangeDot.png", UriKind.Relative));
                    break;

                case SystemType.HIS:
                    TypeIcon = new BitmapImage(new Uri(@"..\..\Images\HisDot.png", UriKind.Relative));
                    break;
            }
        }

        private BitmapImage typeIcon;

        public BitmapImage TypeIcon
        {
            get { return typeIcon; }
            set
            {
                typeIcon = value;
                OnPropertyChanged("TypeIcon");
            }
        }

        public SystemType Type { get; }
        private string date;

        public string Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public string CustomerHistoryDetailId { get; }
        private string customerHistoryData;

        public string CustomerHistoryData
        {
            get { return customerHistoryData; }
            set
            {
                customerHistoryData = value;
                OnPropertyChanged("CustomerHistoryData");
            }
        }

        public ObservableCollection<CustomerHistoryDetail> HistoryCollection { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<CustomerHistoryMaster> CustomerHistoryMasterCollection { get; }
        private DataTable CustomerHistoryDetails;

        public CustomerHistoryMaster(DataTable customerHistoryMasters, DataTable customerHistoryDetails)
        {
            CustomerHistoryMasterCollection = new ObservableCollection<CustomerHistoryMaster>();

            foreach (DataRow row in customerHistoryMasters.Rows)
            {
                CustomerHistoryMasterCollection.Add(new CustomerHistoryMaster((SystemType)row["TYPE"], row["DATE"].ToString(), row["HISTORY_ID"].ToString(), row["HISTORY_DATA"].ToString()));
            }

            CustomerHistoryDetails = customerHistoryDetails;
        }

        public ObservableCollection<CustomerHistoryDetail> getCustomerHistoryDetails(SystemType type, string CustomerHistoryDetailId)
        {
            var table = CustomerHistoryDetails.Select("SYSTEMTYPE = '" + (int)type + "' AND CUSHISTORYDETAILID = '" + CustomerHistoryDetailId + "'");
            var customerHistoryDetailCollection = new ObservableCollection<CustomerHistoryDetail>();

            foreach (var row in table)
            {
                if (type == SystemType.HIS)
                    customerHistoryDetailCollection.Add(new CustomerHistoryHis(row));
                else
                {
                    customerHistoryDetailCollection.Add(new CustomerHistoryPos(row));
                }
            }

            return customerHistoryDetailCollection;
        }
    }
}