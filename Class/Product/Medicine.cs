using System.Data;
using System.Windows.Controls;

namespace His_Pos.Class.Product
{
    public class Medicine : AbstractClass.Product
    {
        public Medicine(DataRow dataRow)
        {
            Id = dataRow["HISMED_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            MedicalCategory = new Medicate
            {
                Dosage = dataRow["HISMED_UNIT"].ToString(),
                Form = dataRow["HISMED_FORM"].ToString()
            };
            Cost = double.Parse(dataRow["HISMED_COST"].ToString());
            Price = double.Parse(dataRow["HISMED_SELLPRICE"].ToString());
            PaySelf = false;
            HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
        }

        public Medicine(string id, string name, double price, double inventory, double total, bool paySelf, double hcPrice, Medicate medicalCategory)
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
