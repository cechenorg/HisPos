using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.Medicine.MedBag;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class MedicineVirtual : Medicine
    {
        public MedicineVirtual(string id)
        {
            switch (id)
            {
                case "R001":
                    ChineseName = "處方箋遺失或毀損，提前回診";
                    break;
                case "R002":
                    ChineseName = "醫師請假，提前回診";
                    break;
                case "R003":
                    ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                    break;
                case "R004":
                    ChineseName = "其他提前回診或慢箋提前領藥";
                    break;
            }
            Dosage = 0.00;
            Days = 0;
            BuckleAmount = 0;
            Amount = 0.00;
            Price = 0.00;
            PaySelf = false;
            IsBuckle = false;
        }
        public MedicineVirtual(DataRow r)
        {
            ID = r.Field<string>("");
            switch (ID)
            {
                case "R001":
                    ChineseName = "處方箋遺失或毀損，提前回診";
                    break;
                case "R002":
                    ChineseName = "醫師請假，提前回診";
                    break;
                case "R003":
                    ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                    break;
                case "R004":
                    ChineseName = "其他提前回診或慢箋提前領藥";
                    break;
            }
            Dosage = 0.00;
            Days = 0;
            BuckleAmount = 0;
            Amount = 0.00;
            Price = 0.00;
            PaySelf = false;
            IsBuckle = false;
        }

        public override MedBagMedicine CreateMedBagMedicine(bool isSingle)
        {
            return null;
        }
    }
}
