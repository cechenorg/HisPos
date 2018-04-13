using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class Medicine : AbstractClass.Product
    {
        public Medicine()
        {
        }

        public Medicine(DataRow dataRow, DataSource dataSource)
        {
            switch(dataSource)
            {
                case DataSource.PRODUCTBASICORSAFE:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Amount = Int16.Parse(dataRow["PRO_BASICQTY"].ToString()) -
                            Int16.Parse(dataRow["PRO_INVENTORY"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    break;
                case DataSource.STOORDLIST:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    break;
                case DataSource.MEDICINE:
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                    MedicalCategory = new Medicate
                    {
                        Dosage = dataRow["HISMED_UNIT"].ToString(),
                        Form = dataRow["HISMED_FORM"].ToString()
                    };
                    PaySelf = false;
                    HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
                    Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
                    StockValue = dataRow["TOTAL"].ToString();
                    IsControlMed = Boolean.Parse((dataRow["HISMED_CONTROL"].ToString() == "")? "False": dataRow["HISMED_CONTROL"].ToString());
                    IsFrozMed = Boolean.Parse((dataRow["HISMED_FROZ"].ToString() == "") ? "False" : dataRow["HISMED_FROZ"].ToString());
                    Location = dataRow["PRO_LOCATION"].ToString();
                    Vis = Visibility.Hidden;
                    break;
            }
            
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            BasicAmount = dataRow["PRO_BASICQTY"].ToString();
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            Inventory = Double.Parse((dataRow["PRO_INVENTORY"].ToString() == "") ? "0" : dataRow["PRO_INVENTORY"].ToString());
        }

        public Medicine(string id, string name, double price, double inventory, double amount, bool paySelf, double hcPrice, Medicate medicalCategory)
        {
            Id = id;
            Name = name;
            Price = price;
            Amount = amount;
            PaySelf = paySelf;
            HcPrice = hcPrice;
            MedicalCategory = medicalCategory;
        }

      
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate MedicalCategory { get; set;}
        public string Ingredient { get; set; }

        public bool IsControlMed { get; set; }

        public bool IsFrozMed { get; set; }
    }
}
