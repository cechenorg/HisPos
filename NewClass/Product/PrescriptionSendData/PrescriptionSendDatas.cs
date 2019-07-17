using System.Collections.ObjectModel;
using MedicineOTC = His_Pos.NewClass.Medicine.Base.MedicineOTC;
using Medicines = His_Pos.NewClass.Medicine.Base.Medicines;

namespace His_Pos.NewClass.Product
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas() { }
        public void ConvertMedToSendData(Medicines ms) {
            foreach (var m in ms) {
                if(!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC) )
                Add(new PrescriptionSendData(m));
            } 
        }

    }
}
