using System.Data;
using System.Windows.Controls;

namespace His_Pos.Class.Product
{
    public class Medicine : AbstractClass.Product
    {
        public Medicine()
        {
        }

        public Medicine(string id, string name, double price, string inventory, double total, bool paySelf, double hcPrice, Medicate medicalCategory)
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
        public double TotalPrice { get; set; }
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate MedicalCategory { get; set;}= new Medicate();

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
