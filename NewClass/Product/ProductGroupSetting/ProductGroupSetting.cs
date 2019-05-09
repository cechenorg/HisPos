using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSetting : ObservableObject
    {
        public ProductGroupSetting() { }

        public ProductGroupSetting(DataRow r) {
            IsEditable = true;
            ProID = r.Field<string>("Pro_ID");
            Name = r.Field<string>("Pro_ChineseName");
        }
        private bool isEditable = false;
        public bool IsEditable
        {
            get { return isEditable; }
            set { Set(() => IsEditable, ref isEditable, value); }
        }
        private string proID;
        public string ProID
        {
            get { return proID; }
            set { Set(() => ProID, ref proID, value); }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value); }
        }
    }
}
