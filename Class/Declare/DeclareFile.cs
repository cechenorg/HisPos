using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace His_Pos.Class.Declare
{
    public class DeclareFile:INotifyPropertyChanged
    {
        public DeclareFile() { }

        public DeclareFile(DataRow row)
        {
            Prescriptions = new ObservableCollection<Prescription>();
            FileContent = SerializeToTdata(row["[HISDEC_XML]"].ToString());
            Id = row["FILE_ID"].ToString();
            DeclareDate = row["DECLARE_TIME"].ToString();
            if(!string.IsNullOrEmpty(row["CHRONIC_COUNT"].ToString()))
                ChronicCount = int.Parse(row["CHRONIC_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["NORMAL_COUNT"].ToString()))
                NormalCount = int.Parse(row["NORMAL_COUNT"].ToString());
            if (!string.IsNullOrEmpty(row["TOTALPOINT"].ToString()))
                TotalPoint = double.Parse(row["TOTALPOINT"].ToString());
            ErrorPrescriptionList = SerializeToErrorPrescriptions(row["HISDEC_ERROR"].ToString());
            IsDeclared = row["IS_DECLARED"].ToString().Equals("1");
        }

        public string Id { get; set; }

        private string _declareDate;
        public string DeclareDate
        {
            get => _declareDate;
            set
            {
                //0107-08-31
                _declareDate = value;
                DeclareYear = _declareDate.Substring(0, 4);
                DeclareMonth = _declareDate.Substring(5, 2);
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
        
        public ObservableCollection<Prescription> Prescriptions { get; set; }

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
                _errorPrescriptionList = value;
                if (_errorPrescriptionList.ErrorList.Count > 0)
                    HasError = true;
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

        private Pharmacy SerializeToTdata(string xmlStr)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Tdata));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr));
            Pharmacy tData = (Pharmacy)serializer.Deserialize(memStream);
            return tData;
        }
        public ErrorPrescriptions SerializeToErrorPrescriptions(string xmlStr)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ErrorPrescriptions));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr));
            ErrorPrescriptions errorPrescriptions = (ErrorPrescriptions)serializer.Deserialize(memStream);
            return errorPrescriptions;
        }
    }
}
