using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class Medicine : AbstractClass.Product
    {
        public Medicine()
        {
        }

        public Medicine(DataRow dataRow)
        {
            TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            MedicalCategory = new Medicate
            {
                Dosage = dataRow["HISMED_UNIT"].ToString(),
                Form = dataRow["HISMED_FORM"].ToString()
            };
            //Cost = double.Parse(dataRow["HISMED_COST"].ToString());
            //Price = double.Parse(dataRow["HISMED_SELLPRICE"].ToString());
            PaySelf = false;
            HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            StockValue = dataRow["TOTAL"].ToString();
            BasicAmount = dataRow["PRO_BASICQTY"].ToString();
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
        }

        public Medicine(string id, string name, double price, double inventory, double total, bool paySelf, double hcPrice, Medicate medicalCategory)
        {
            Id = id;
            Name = name;
            Price = price;
            Total = total;
            PaySelf = paySelf;
            HcPrice = hcPrice;
            MedicalCategory = medicalCategory;
        }

        public double Total { get; set; }
        public double TotalPrice { get; set; }
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate MedicalCategory { get; set;}

        public string Ingredient { get; set; }
    }
}
