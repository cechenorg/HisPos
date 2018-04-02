using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.Class.Manufactory
{
    public class Manufactory : INotifyPropertyChanged
    {
        public Manufactory()
        {
            vis = Visibility.Hidden;
        }

        public Manufactory(DataRow row, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.MANUFACTORY:
                    Address = row["MAN_ADDR"].ToString();
                    Telphone = row["MAN_TEL"].ToString();
                    Fax = row["MAN_FAX"].ToString();
                    break;
                case DataSource.PROMAN:
                    OrderId = row["ORDER_ID"].ToString();
                    break;
            }
            
            Id = row["MAN_ID"].ToString();
            Name = row["MAN_NAME"].ToString();
            vis = Visibility.Hidden;
        }
        
        public string Id { get; set; }
        public string Name{ get; set; }
        public string Address{ get; set; }
        public string Telphone{ get; set; }
        public string Fax { get; set; }
        public string OrderId { get; set; }

        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Manufactory).Id.Contains(searchText)
                    || (obj as Manufactory).Name.Contains(searchText);
            }
        }

        public Visibility vis
        {
            get { return Vis; }
            set
            {
                Vis = value;
                NotifyPropertyChanged("Vis");
            }
        }

        private Visibility Vis = Visibility.Hidden;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
