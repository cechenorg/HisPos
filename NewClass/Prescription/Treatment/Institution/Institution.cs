using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class Institution : ObservableObject
    {
        public Institution(){}

        public Institution(DataRow r)
        {
            Id = r["Ins_ID"].ToString();
            Name = r["Ins_Name"].ToString();
            FullName = r["Ins_FullName"].ToString();
            Common = r["Ins_IsCommon"].ToString() == "0" ? false : true;
            IsCooperative = r["Ins_IsCooperate"].ToString() == "0" ? false : true; 
        }
        public string Id { get; }//院所代碼
        public string Name { get; set; }//院所名稱
        public string FullName { get; set; }
        public bool Common { get; set; }
        public bool IsCooperative { get; set; }
    }
}
