using System;
using System.Data;
using System.Linq;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class Institution : ObservableObject
    {
        public Institution(){}
         
        public Institution(DataRow r)
        {
            Id = r.Field<string>("Ins_ID");
            Name = r.Field<string>("Ins_Name");
            FullName = r.Field<string>("Ins_FullName");
            Common = r.Field<bool>("Ins_IsCommon");
            IsCooperative = r.Field<bool>("Ins_IsCooperate");
        }
        public string Id { get; }//院所代碼
        public string Name { get; set; }//院所名稱
        private string fullName;
        public string FullName
        {
            get => fullName;
            set { Set(() => FullName, ref fullName, value); }
        }

        public bool Common { get; set; }
        public bool IsCooperative { get; set; }
    }
}
