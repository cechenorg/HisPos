using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class MedicineInformation : AbstractClass.Product
    {
        public MedicineInformation() { }
        public MedicineInformation(DataRow dataRow) : base(dataRow)
        {
            SingleComplex = dataRow["HISMED_SC"].ToString();
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            Manufactory = dataRow["HISMED_MANUFACTORY"].ToString();
            Indication = dataRow["HISMED_INDICATION"].ToString();
            SideEffect = dataRow["HISMED_SIDEFFECT"].ToString();
            HcPrice = dataRow["HISMED_PRICE"].ToString();
            SellPrice = dataRow["HISMED_SELLPRICE"].ToString();
            LastPrice = dataRow["HISMED_LASTPRICE"].ToString();
            Form = dataRow["HISMED_FORM"].ToString();
            Note = dataRow["HISMED_NOTE"].ToString();
        }
        public string SingleComplex { get; set; }
        public string Ingredient { get; set; }
        public string Manufactory { get; set; }
        public string Indication { get; set; }
        public string SideEffect { get; set; }
        public string HcPrice { get; set; }
        public string SellPrice { get; set; }
        public string LastPrice { get; set; }
        public string Form { get; set; }
        public string Note { get; set; }
    }
}
