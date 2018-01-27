using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Medicine : Product
    {
        public Medicine()
        {
        }

        public Medicine(string id, string name, string price, string inventory, string total, string paySelf, string hcPrice, Medicate medicalCategory)
        {
            Id = id;
            Name = name;
            Price = price;
            Inventory = inventory;
            Total = total;
            PaySelf = paySelf;
            HcPrice = hcPrice;

        }
        public string Total { get; set; }
        public string PaySelf { get; set; }
        public string HcPrice { get; set; }
        public string Dosage { get; set; }
        public string Usage { get; set; }
        public string Days { get; set; }
        public string Position { get; set; }
        public string Form { get; set; }

        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Medicine).Id.Contains(searchText)
                    || (obj as Medicine).Name.Contains(searchText);
            }
        }
    }
}
