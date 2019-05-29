using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow
{
    public class CustomerPrescriptionViewModel : ViewModelBase
    {
        #region Variable
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }
        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            private set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        private bool showDialog;
        public bool ShowDialog
        {
            get => showDialog;
            set
            {
                Set(() => ShowDialog, ref showDialog, value);
            }
        }
        public IcCard Card { get; set; }
        public int PatientID { get; }
        public string PatientIDNumber { get; }
        private string orthopedicsContent;
        public string OrthopedicsContent
        {
            get => orthopedicsContent;
            set
            {
                Set(() => OrthopedicsContent, ref orthopedicsContent, value);
            }
        }
        private string cooperativeContent;
        public string CooperativeContent
        {
            get => cooperativeContent;
            set
            {
                Set(() => CooperativeContent, ref cooperativeContent, value);
            }
        }
        private string registerContent;
        public string RegisterContent
        {
            get => registerContent;
            set
            {
                Set(() => RegisterContent, ref registerContent, value);
            }
        }
        private string reserveContent;
        public string ReserveContent
        {
            get => reserveContent;
            set
            {
                Set(() => ReserveContent, ref reserveContent, value);
            }
        }
        public CusPrePreviewBases OrthopedicsPres { get; set; }
        public CusPrePreviewBases CooperativePres { get; set; }
        public CusPrePreviewBases ChronicRegisterPres { get; set; }
        public CusPrePreviewBases ChronicReservePres { get; set; }
        public CusPrePreviewBases UngetCardPres { get; set; }
        #endregion
        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public CustomerPrescriptionViewModel(int cusID, string cusIDNumber, IcCard card)
        {
            PatientID = cusID;
            PatientIDNumber = cusIDNumber;
            Card = card.DeepCloneViaJson();
            InitializePrescription();
        }
        private void InitializePrescription()
        {
            OrthopedicsPres = new CusPrePreviewBases();
            CooperativePres = new CusPrePreviewBases();
            ChronicRegisterPres = new CusPrePreviewBases();
            ChronicReservePres = new CusPrePreviewBases();
            MainWindow.ServerConnection.OpenConnection();
            OrthopedicsPres.GetOrthopedicsByCustomerIDNumber(PatientIDNumber);
            CooperativePres.GetCooperativeByCusIDNumber(PatientIDNumber);
            ChronicRegisterPres.GetRegisterByCusId(PatientID);
            ChronicReservePres.GetReserveByCusId(PatientID);
            UngetCardPres.GetUngetCardByCusId(PatientID);
            MainWindow.ServerConnection.CloseConnection();
            OrthopedicsContent = "骨科 " + OrthopedicsPres.Count + " 張";
            CooperativeContent = "合作 " + CooperativePres.Count + " 張";
            RegisterContent = "登錄 " + ChronicRegisterPres.Count + " 張";
            ReserveContent = "預約 " + ChronicReservePres.Count + " 張";
        }
    }
}
