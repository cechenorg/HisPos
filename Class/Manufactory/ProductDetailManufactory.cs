using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Interface;

namespace His_Pos.Class.Manufactory
{
    public class ProductDetailManufactory : Manufactory, IDeletable
    {
        public ProductDetailManufactory()
        {
        }
        public ProductDetailManufactory(Manufactory manufactory)
        {
            Id = manufactory.Id;
            Name = manufactory.Name;
            Address = manufactory.Address;
            Telphone = manufactory.Telphone;
            Fax = manufactory.Fax;
        }
        public ProductDetailManufactory(DataRow row, DataSource dataSource)
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
        }

        private string source;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string OrderId { get; set; }

        public string Source {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }

        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Manufactory).Id is null)? true:(obj as Manufactory).Id.Contains(searchText)
                    || (obj as Manufactory).Name.Contains(searchText);
            }
        }
        
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


    }
}
