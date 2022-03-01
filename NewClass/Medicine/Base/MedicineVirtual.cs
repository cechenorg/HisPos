using His_Pos.NewClass.Medicine.MedBag;
using System.Data;

namespace His_Pos.NewClass.Medicine.Base
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

                case "R005":
                    ChineseName = "新特約無法使用雲端藥歷";
                    break;
            }
            ID = id;
            Dosage = 0.00;
            Days = null;
            BuckleAmount = 0;
            Amount = 0.00;
            Price = 0.00;
            PaySelf = false;
            IsBuckle = false;
            CanEdit = false;
        }

        public MedicineVirtual(DataRow r)
        {
            ID = r.Field<string>("Pro_ID");
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

                case "R005":
                    ChineseName = "新特約無法使用雲端藥歷";
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