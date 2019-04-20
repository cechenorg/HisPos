using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.Medicine.MedicineSet
{
    public class MedicineSet:ObservableObject
    {
        public MedicineSet() { }

        public MedicineSet(DataRow r)
        {
            ID = r.Field<int>("");
            Name = r.Field<string>("");
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
        private ObservableCollection<MedicineSetItem> medicines;
        public ObservableCollection<MedicineSetItem> Medicines
        {
            get => medicines;
            set
            {
                Set(() => Medicines, ref medicines, value);
            }
        }
        private MedicineSetItem selectedMedicine;
        public MedicineSetItem SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = false;

                Set(() => SelectedMedicine, ref selectedMedicine, value);

                if (selectedMedicine != null)
                    ((IDeletableProduct)selectedMedicine).IsSelected = true;
            }
        }
        public void GetSetItems()
        {
            var table = MedicineSetDb.GetMedicineSetDetail(ID);
            foreach (DataRow r in table.Rows)
            {
                Medicines.Add(new MedicineSetItem(r));
            }
        }
    }
}
