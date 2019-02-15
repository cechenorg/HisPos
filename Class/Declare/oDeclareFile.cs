using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using His_Pos.Service;
using JetBrains.Annotations;

namespace His_Pos.Class.Declare
{
    public class oDeclareFile:INotifyPropertyChanged
    {
        public oDeclareFile()
        {
            ErrorPrescriptionList = new ErrorPrescriptions();
            PrescriptionDdatas = new ObservableCollection<DeclareFileDdata>();
        }

        public oDeclareFile(DataRow row)
        {
            PrescriptionDdatas = new ObservableCollection<DeclareFileDdata>();
            if (!string.IsNullOrEmpty(row["HISDEC_XML"].ToString()))
                FileContent = XmlService.Deserialize<Pharmacy>(row["HISDEC_XML"].ToString());
            if (!string.IsNullOrEmpty(row["HISDEC_ID"].ToString()))
                Id = row["HISDEC_ID"].ToString();
            if (!string.IsNullOrEmpty(row["HISDEC_SENTDATE"].ToString()))
                DeclareDate = row["HISDEC_SENTDATE"].ToString();
            if(!string.IsNullOrEmpty(row["HISDEC_CHRONIC_COUNT"].ToString()))
                ChronicCount = int.Parse(row["HISDEC_CHRONIC_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["HISDEC_NORMAL_COUNT"].ToString()))
                NormalCount = int.Parse(row["HISDEC_NORMAL_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["HISDEC_TOTALPOINT"].ToString()))
                TotalPoint = double.Parse(row["HISDEC_TOTALPOINT"].ToString());
            if (!string.IsNullOrEmpty(row["HISDEC_ERROR"].ToString()))
                ErrorPrescriptionList = XmlService.Deserialize<ErrorPrescriptions>(row["HISDEC_ERROR"].ToString());
            if (!string.IsNullOrEmpty(row["HISDEC_PHARMACY_ID"].ToString()))
                PhmarcyId = row["HISDEC_PHARMACY_ID"].ToString();
            if (!string.IsNullOrEmpty(row["HISDEC_IS_DECLARED"].ToString()))
                IsDeclared = row["HISDEC_IS_DECLARED"].ToString().Equals("1");
            if (!string.IsNullOrEmpty(row["HISDEC_HAS_ERROR"].ToString()))
                IsDeclared = row["HISDEC_HAS_ERROR"].ToString().Equals("1");
        }

        public string Id { get; set; }

        private string _pharmacyId;

        public string PhmarcyId
        {
            get => _pharmacyId;
            set
            {
                _pharmacyId = value;
                OnPropertyChanged(nameof(PhmarcyId));
            }
        }

        private string _declareDate;
        public string DeclareDate
        {
            get => _declareDate;
            set
            {
                //0107-08-31
                _declareDate = value;
                DeclareYear = _declareDate.Substring(0, 4);
                DeclareMonth = _declareDate.Substring(5, 2).Contains("/") ? _declareDate.Substring(5, 1).PadLeft(2,'0') : _declareDate.Substring(5, 2);
            }
        }

        private string _declareYear;
        public string DeclareYear
        {
            get => _declareYear;
            set
            {
                _declareYear = value.StartsWith("0") ? value.Substring(1,3) : value;
                OnPropertyChanged(nameof(DeclareYear));
            }
        }

        private string _declareMonth;

        public string DeclareMonth
        {
            get=>_declareMonth;
            set
            {
                _declareMonth = value;
                OnPropertyChanged(nameof(DeclareMonth));
            }
        }

        private int _chronicCount;

        public int ChronicCount
        {
            get => _chronicCount;
            set
            {
                _chronicCount = value;
                OnPropertyChanged(nameof(ChronicCount));
            }
        }
        private int _normalCount;
        public int NormalCount
        {
            get => _normalCount;
            set
            {
                _normalCount = value;
                OnPropertyChanged(nameof(NormalCount));
            }
        }

        private double _totalPoint;

        public double TotalPoint
        {
            get => _totalPoint;
            set
            {
                _totalPoint = value;
                OnPropertyChanged(nameof(TotalPoint));
            }
        }
        
        public ObservableCollection<DeclareFileDdata> PrescriptionDdatas { get; set; }

        private Pharmacy _fileContent;

        public Pharmacy FileContent
        {
            get => _fileContent;
            set
            {
                _fileContent = value;
                
                OnPropertyChanged(nameof(FileContent));
            }
        }

        private ErrorPrescriptions _errorPrescriptionList;

        public ErrorPrescriptions ErrorPrescriptionList
        {
            get => _errorPrescriptionList;
            set
            {
                if(value == null)
                    return;
                _errorPrescriptionList = value;
                if (_errorPrescriptionList.ErrorList != null)
                {
                    foreach (var p in _errorPrescriptionList.ErrorList)
                    {
                        if(p.Error == null)
                            continue;
                        else if (p.Error.Count > 0)
                            HasError = true;
                    }
                }
                OnPropertyChanged(nameof(ErrorPrescriptionList));
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }

        private bool _isDeclared;

        public bool IsDeclared
        {
            get => _isDeclared;
            set
            {
                _isDeclared = value;
                OnPropertyChanged(nameof(IsDeclared));
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
