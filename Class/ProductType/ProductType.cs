using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.ProductType
{
    public class ProductType : INotifyPropertyChanged
    {
        public ProductType() {
            Id = "";
            Rank = "";
            Name = "無";
        }
        public ProductType(DataRow row) {
            Id = row["PROTYP_ID"].ToString();
            Rank = row["PROTYP_PARENT"].ToString();
            Name = row["PROTYP_CHINAME"].ToString();
            EngName = row["PROTYP_ENGNAME"].ToString();
            
        }

        protected ProductType(string id, string rank, string name)
        {
            Id = id;
            Rank = rank;
            Name = name;
        }

        public string Id { get; set; }
        public string Rank { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string engName;
        public string EngName
        {
            get { return engName; }
            set
            {
                engName = value;
                NotifyPropertyChanged("EngName");
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
    }
}
