using GalaSoft.MvvmLight;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.MedicineSet;
using System.Data;
using OrthopedicsMedicine = His_Pos.NewClass.Cooperative.CooperativeInstitution.Item;

namespace His_Pos.NewClass.Product
{
    public abstract class Product : ObservableObject
    {
        public Product()
        {
        }

        public Product(DataRow row)
        {
            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            IsCommon = row.Field<bool>("Pro_IsCommon");
        }

        public Product(ProductStruct p)
        {
            ID = p.ID;
            ChineseName = p.ChineseName;
            EnglishName = p.EnglishName;
        }

        public Product(MedicineSetItem m)
        {
            ID = m.ID;
            ChineseName = m.ChineseName;
            EnglishName = m.EnglishName;
        }

        protected Product(OrthopedicsMedicine m)
        {
            ID = m.Id;
            ChineseName = m.Desc;
            EnglishName = m.Desc;
        }

        protected Product(CooperativePrescription.Item m)
        {
            ID = m.Id;
            ChineseName = m.Desc;
            EnglishName = m.Desc;
        }

        #region ----- Define Variables -----

        private string id;

        public string ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private string chineseName;

        public string ChineseName
        {
            get { return chineseName; }
            set
            {
                Set(() => ChineseName, ref chineseName, value);
                RaisePropertyChanged(nameof(FullName));
            }
        }

        private string englishName;

        public string EnglishName
        {
            get { return englishName; }
            set
            {
                Set(() => EnglishName, ref englishName, value);
                RaisePropertyChanged(nameof(FullName));
            }
        }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(EnglishName))
                    return (EnglishName.Contains(" ") ? EnglishName.Substring(0, EnglishName.IndexOf(" ")) : EnglishName) + ChineseName;
                return !string.IsNullOrEmpty(ChineseName) ? ChineseName : string.Empty;
            }
        }

        private bool isCommon;//常備品項

        public bool IsCommon
        {
            get => isCommon;
            set
            {
                if (isCommon != value)
                {
                    Set(() => IsCommon, ref isCommon, value);
                }
            }
        }

        #endregion ----- Define Variables -----
    }
}