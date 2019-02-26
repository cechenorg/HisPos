using System.Data;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.Medicine
{
    public class MedicineOTC:Medicine, IDeletable
    {
        public MedicineOTC() : base() { }
        public MedicineOTC(DataRow r) : base(r) { }

        private string source;
        public string Source
        {
            get => source;
            set
            {
                Set(() => Source, ref source, value);
            }
        }

    }
}
