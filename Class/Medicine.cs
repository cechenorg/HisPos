using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using His_Pos.AbstractClass;
using His_Pos.Interface;

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
            MedicalCategory = medicalCategory;
        }
        public string Total { get; set; }
        public string PaySelf { get; set; }
        public string HcPrice { get; set; }
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

        public void GetData(DataRow d)
        {
            Id = d["HISMED_ID"].ToString();
            Name = d["PRO_NAME"].ToString();
            MedicalCategory = new Medicate
            {
                Dosage = d["HISMED_UNIT"].ToString(),
                Form = d["HISMED_FORM"].ToString()
            };
            Cost = d["HISMED_COST"].ToString();
            Price = d["HISMED_SELLPRICE"].ToString();
            PaySelf = "0";
        }
    }
}
