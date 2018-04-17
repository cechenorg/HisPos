using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class Medicine : AbstractClass.Product
    {
        public Medicine(DataRow dataRow) : base(dataRow)
        {
            IsControlMed = Boolean.Parse((dataRow["HISMED_CONTROL"].ToString() == "") ? "False" : dataRow["HISMED_CONTROL"].ToString());
            IsFrozMed = Boolean.Parse((dataRow["HISMED_FROZ"].ToString() == "") ? "False" : dataRow["HISMED_FROZ"].ToString());
            PaySelf = false;
            HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            MedicalCategory = new Medicate
            {
                Dosage = dataRow["HISMED_UNIT"].ToString(),
                Form = dataRow["HISMED_FORM"].ToString()
            };
        }
        
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate MedicalCategory { get; set;}
        public string Ingredient { get; set; }
        public bool IsControlMed { get; set; }
        public bool IsFrozMed { get; set; }
    }
}
