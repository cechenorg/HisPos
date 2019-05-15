using System.Data;
using System.Linq;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using ZeroFormatter;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    [ZeroFormattable]
    public class Institution : ObservableObject
    {
        public Institution(){
            
        }
         
        public Institution(DataRow r)
        {
            ID = r.Field<string>("Ins_ID");
            Name = r.Field<string>("Ins_Name");
            FullName = r.Field<string>("Ins_FullName");
            Common = r.Field<bool>("Ins_IsCommon");
            IsCooperative = r.Field<bool>("Ins_IsCooperate");
        }
        [Index(0)]
        public virtual string ID { get; set; } = string.Empty;
        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;
        private string fullName = string.Empty;
        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }
        [IgnoreFormat]
        public bool Common { get; set; }
        [IgnoreFormat]
        public bool IsCooperative { get; set; }
        #region Function
        public void UpdateUsedTime() {
            InstitutionDb.UpdateUsedTime(ID);
        }
        #endregion

        public CooperativeClinicSetting IsCooperativeClinic()
        {
            return ViewModelMainWindow.CooperativeClinicSettings.SingleOrDefault(c => c.CooperavieClinic.ID.Equals(ID));
        }

        public bool CheckIsOrthopedics()
        {
            return !string.IsNullOrEmpty(ViewModelMainWindow.CooperativeInstitutionID) && ID.Equals(ViewModelMainWindow.CooperativeInstitutionID);
        }

        public bool CheckCooperative()
        {
            return VM.CooperativeClinicSettings.Count(c => c.CooperavieClinic.ID.Equals(ID)) > 0;
        }
    }
}
