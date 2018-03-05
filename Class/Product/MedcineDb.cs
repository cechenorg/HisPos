using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class MedcineDb 
    {
        public static Medicine GetMedicineData(DataRow d)
        {
            var medicine = new Medicine
            {
                Id = d["HISMED_ID"].ToString(),
                Name = d["PRO_NAME"].ToString(),
                MedicalCategory = new Medicate
                {
                    Dosage = d["HISMED_UNIT"].ToString(),
                    Form = d["HISMED_FORM"].ToString()
                },
                Cost = double.Parse(d["HISMED_COST"].ToString()),
                Price = double.Parse(d["HISMED_SELLPRICE"].ToString()),
                PaySelf = false,
                HcPrice = double.Parse(d["HISMED_PRICE"].ToString())
            };
            return medicine;
        }
    }
}
