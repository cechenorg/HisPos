using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas() { }
        public void ConvertMedToSendData(Medicine.Medicines ms) {
            foreach (Medicine.Medicine m in ms) {
                if(!string.IsNullOrEmpty(m.ID))
                Add(new PrescriptionSendData(m));
            } 
        }

    }
}
