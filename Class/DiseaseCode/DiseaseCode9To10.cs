using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.Class.DiseaseCode
{
    public class DiseaseCode9To10:INotifyPropertyChanged
    {
        private DiseaseCode icd10;
        private DiseaseCode icd9;
        public DiseaseCode ICD10
        {
            get => icd10;
            set
            {
                icd10 = value;
                OnPropertyChanged(nameof(ICD10));
            }
        }

        public DiseaseCode ICD9
        {
            get => icd9;
            set
            {
                icd9 = value;
                OnPropertyChanged(nameof(ICD9));
            }
        }

        private string icd10_Type;

        public string ICD10_Type
        {
            get => icd10_Type;
            set
            {
                icd10_Type = value;
                OnPropertyChanged(nameof(ICD10_Type));
            }
        }

        public DiseaseCode9To10(DataRow row)
        {
            ICD9 = new DiseaseCode();
            ICD10 = new DiseaseCode();
            if (row["ICD_10_ID"].ToString().Equals("查無疾病代碼"))
            {
                ICD10.Id = row["ICD_10_ID"].ToString();
                ICD10.Name = string.Empty;
                ICD9.Id = string.Empty;
                ICD9.Name = string.Empty;
                ICD10_Type = string.Empty;
            }
            else
            {
                ICD9.Id = row["ICD_9_ID"].ToString();
                ICD9.Name = row["ICD_9_CHI"].ToString();
                ICD10.Id = row["ICD_10_ID"].ToString();
                ICD10.Name = row["ICD_10_CHI"].ToString();
                ICD10_Type = row["ICD_10_TYPE"].ToString().Equals("1")?"CM":"PCS";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
