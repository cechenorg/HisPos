using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product;
using System.Data;

namespace His_Pos.NewClass.Medicine.MedicineSet
{
    public class MedicineSet : ObservableObject
    {
        public MedicineSet()
        {
            MedicineSetItems = new MedicineSetItems();
        }

        public MedicineSet(DataRow r)
        {
            ID = r.Field<int>("MedSet_ID");
            Name = r.Field<string>("MedSet_Name");
        }

        public int ID { get; set; }
        private string name;

        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }

        private MedicineSetItems medicineSetItems;

        public MedicineSetItems MedicineSetItems
        {
            get => medicineSetItems;
            set
            {
                Set(() => MedicineSetItems, ref medicineSetItems, value);
            }
        }

        private MedicineSetItem selectedMedicine;

        public MedicineSetItem SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (selectedMedicine != null)
                    selectedMedicine.IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    selectedMedicine.IsSelected = true;
            }
        }

        public void AddMedicine(ProductStruct p)
        {
            var medicine = new MedicineSetItem();
            medicine.ID = p.ID;
            medicine.ChineseName = p.ChineseName;
            medicine.EnglishName = p.EnglishName;
            medicine.NHIPrice = p.NHIPrice;
            medicine.IsCommon = p.IsCommon;
            medicine.Frozen = p.IsFrozen;
            if (medicine.ID.EndsWith("00") ||
                medicine.ID.EndsWith("G0"))
                medicine.PositionID = "PO";
            var selectedMedicinesIndex = MedicineSetItems.IndexOf(SelectedMedicine);
            if (SelectedMedicine != null)
            {
                if (selectedMedicinesIndex > 0)
                    medicine.CopyPrevious(MedicineSetItems[selectedMedicinesIndex - 1]);
                MedicineSetItems[selectedMedicinesIndex] = medicine;
            }
            else
            {
                if (MedicineSetItems.Count > 0)
                    medicine.CopyPrevious(MedicineSetItems[MedicineSetItems.Count - 1]);
                MedicineSetItems.Add(medicine);
            }
        }
    }
}