using GalaSoft.MvvmLight;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using System.Data;
using System.Linq;
using ZeroFormatter;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    [ZeroFormattable]
    public class Institution : ObservableObject
    {
        public Institution()
        {
        }

        public Institution(DataRow r)
        {
            ID = r.Field<string>("Ins_ID");
            Name = r.Field<string>("Ins_Name");
            FullName = r.Field<string>("Ins_FullName");
            LevelType = r.Field<string>("Ins_Type");
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

        [IgnoreFormat]
        public string LevelType { get; set; } //院所等級
        #region Function

        public void UpdateUsedTime()
        {
            InstitutionDb.UpdateUsedTime(ID);
        }

        #endregion Function

        public CooperativeClinicSetting IsCooperativeClinic()
        {
            return VM.CooperativeClinicSettings.SingleOrDefault(c => c.CooperavieClinic.ID.Equals(ID));
        }

        public bool CheckIsOrthopedics()
        {
            return !string.IsNullOrEmpty(VM.CooperativeInstitutionID) && ID.Equals(VM.CooperativeInstitutionID);
        }

        public bool CheckCooperative()
        {
            return VM.CooperativeClinicSettings.Count(c => c.CooperavieClinic.ID.Equals(ID)) > 0;
        }

        public bool CheckIDEqualsCurrentPharmacy()
        {
            return !string.IsNullOrEmpty(ID) && ID.Equals(VM.CurrentPharmacy.ID);
        }
    }
}