using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass
{
    public class DiseaseCodeContrast:INotifyPropertyChanged
    {
        private Class.DiseaseCode.DiseaseCode icd10;
        public Class.DiseaseCode.DiseaseCode Icd10
        {
            get => icd10;
            set
            {
                icd10 = value;
                OnPropertyChanged(nameof(Icd10));
            }
        }
        private Class.DiseaseCode.DiseaseCode icd9;
        public Class.DiseaseCode.DiseaseCode Icd9
        {
            get => icd9;
            set
            {
                icd9 = value;
                OnPropertyChanged(nameof(Icd9));
            }
        }

        private string _icd10Type;

        public string Icd10Type
        {
            get => _icd10Type;
            set
            {
                _icd10Type = value;
                OnPropertyChanged(nameof(Icd10Type));
            }
        }

        public DiseaseCodeContrast(DataRow row)
        {
            Icd9 = new Class.DiseaseCode.DiseaseCode();
            Icd10 = new Class.DiseaseCode.DiseaseCode();
            if (row["ICD_10_ID"].ToString().Equals("查無疾病代碼"))
            {
                Icd10.Id = row["ICD_10_ID"].ToString();
                Icd10.Name = string.Empty;
                Icd9.Id = string.Empty;
                Icd9.Name = string.Empty;
                Icd10Type = string.Empty;
            }
            else
            {
                Icd9.Id = row["ICD_9_ID"].ToString();
                Icd9.Name = row["ICD_9_CHI"].ToString();
                Icd10.Id = row["ICD_10_ID"].ToString();
                Icd10.Name = row["ICD_10_CHI"].ToString();
                Icd10Type = row["ICD_10_TYPE"].ToString().Equals("1") ? "CM" : "PCS";
            }
        }
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
