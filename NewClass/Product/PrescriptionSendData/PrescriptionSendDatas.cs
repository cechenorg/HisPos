using His_Pos.NewClass.Product.Medicine;
using System.Collections.ObjectModel;

namespace His_Pos.NewClass.Product
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas() { }
        public void ConvertMedToSendData(Medicine.Medicines ms) {
            foreach (Medicine.Medicine m in ms) {
                if(!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC) )
                Add(new PrescriptionSendData(m));
            } 
        }

    }
}
