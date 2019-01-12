using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class Institution : ObservableObject
    {
        public Institution(){}

        public Institution(DataRow r)
        {
            Id = r[""].ToString();
            Name = r[""].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; }//院所代碼
        public string Name { get; set; }//院所名稱
        public string FullName { get; set; }
        public bool Common { get; set; }
        public bool IsCooperative { get; set; }
    }
}
