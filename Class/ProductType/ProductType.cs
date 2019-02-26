using System.ComponentModel;
using System.Data;

namespace His_Pos.Class.ProductType
{
    public class ProductType : INotifyPropertyChanged
    {
        public ProductType() {
            Id = "";
            Parent = "";
            Name = "無";
        }
        public ProductType(DataRow row) {
            Id = row["PROTYP_ID"].ToString();
            Parent = row["PROTYP_PARENT"].ToString();
            Name = row["PROTYP_CHINAME"].ToString();
            EngName = row["PROTYP_ENGNAME"].ToString();
            
        }

        protected ProductType(string parent, string name, string engName)
        {
            Id = "";/// ProductDb.AddNewType(parent, name, engName);
            Parent = parent;
            Name = name;
            EngName = engName;
        }

        public string Id { get; set; }
        public string Parent { get; set; }
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
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
