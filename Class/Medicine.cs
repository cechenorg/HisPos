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

        public Medicine(string id, string name, double price, double inventory, double total, string paySelf, double hcPrice, Medicate medicalCategory)
        {
            Id = id;
            Name = name;
            Price = price;
            Inventory = inventory;
            Total = total;
            PaySelf = paySelf;
            HcPrice = hcPrice;
            MedicalCategory = medicalCategory;
        }
        public double Total { get; set; }
        public string PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate MedicalCategory { get; set;}

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
