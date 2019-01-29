using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using Cus = His_Pos.NewClass.Person.Customer.Customer;
using IcCard = His_Pos.NewClass.Prescription.IcCard;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow
{
    public class CustomPrescriptionViewModel:ViewModelBase
    {
        public Prescriptions CooperativePrescriptions { get; set; }
        public Prescriptions ReservedPrescriptions { get; set; }
        public Prescriptions UngetCardPrescriptions { get; set; }
        public Prescription SelectedPrescription { get; set; }
        public RelayCommand MakeUpClick { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public Cus Patient { get; set; }
        public IcCard Card { get; set; }
        private bool isSelectCooperative { get; set; }
        private Visibility cooperativeVisible;

        public Visibility CooperativeVisible
        {
            get => cooperativeVisible;
            set
            {
                Set(() => CooperativeVisible, ref cooperativeVisible, value);
            }
        }

        private Visibility reservedVisible;

        public Visibility ReservedVisible
        {
            get => reservedVisible;
            set
            {
                Set(() => ReservedVisible, ref reservedVisible, value);
            }
        }

        private Visibility ungetCardVisible;

        public Visibility UngetCardVisible
        {
            get => ungetCardVisible;
            set
            {
                Set(() => UngetCardVisible, ref ungetCardVisible, value);
            }
        }
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

        public CustomPrescriptionViewModel(Cus cus, IcCard card,bool isGetMakeUpPrescription)
        {
            Patient = cus;
            Card = card;
            InitializePrescription();
            RegisterMessenger();
            InitialCommand();
        }

        private void InitialCommand()
        {
            MakeUpClick = new RelayCommand(MakeUpClickAction);
            PrescriptionSelected = new RelayCommand(CustomPrescriptionSelected);
        }

        private void ConfirmClickAction()
        {
            if (SelectedPrescription is null)
                MessageWindow.ShowMessage("尚未選擇處方", MessageType.ERROR);
            else
                CustomPrescriptionSelected();
                
        }

        private void CancleClickAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
        }

        private void MakeUpClickAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                foreach (var p in UngetCardPrescriptions)
                {
                    p.Patient = Patient;
                    p.Card = Card;
                    if (CreatePrescriptionSign(p))
                        HisApiFunction.CreatDailyUploadData(p,true);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private bool CreatePrescriptionSign(Prescription prescription)
        {
            BusyContent = StringRes.寫卡;
            prescription.PrescriptionSign = HisApiFunction.WritePrescriptionData(prescription);
            BusyContent = StringRes.產生每日上傳資料;
            if (prescription.PrescriptionSign.Count != prescription.Medicines.Count(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf))
            {
                bool? isDone = null;
                ErrorUploadWindowViewModel.IcErrorCode errorCode;
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage(StringRes.寫卡異常, MessageType.ERROR);
                    var e = new ErrorUploadWindow(prescription.Card.IsGetMedicalNumber); //詢問異常上傳
                    e.ShowDialog();
                    while (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
                    {
                        e = new ErrorUploadWindow(prescription.Card.IsGetMedicalNumber);
                        e.ShowDialog();
                    }
                    errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
                    if (isDone is null)
                        HisApiFunction.CreatErrorDailyUploadData(prescription, true,errorCode);
                    isDone = true;
                });
                return false;
            }
            return true;
        }

        private void InitializePrescription()
        {
            SelectedPrescription = null;
            CooperativePrescriptions = new Prescriptions();
            MainWindow.ServerConnection.OpenConnection();
            CooperativePrescriptions.GetCooperaPrescriptionsByCusIDNumber(Patient.IDNumber);
            ReservedPrescriptions = new Prescriptions();
            ReservedPrescriptions.GetReservePrescriptionByCusId(Patient.ID);
            ReservedPrescriptions.GetRegisterPrescriptionByCusId(Patient.ID);
            UngetCardPrescriptions = new Prescriptions();
            UngetCardPrescriptions.GetPrescriptionsNoGetCardByCusId(Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
            if (CooperativePrescriptions.Count == 0)
                CooperativeVisible = Visibility.Collapsed;
            if(ReservedPrescriptions.Count == 0)
                ReservedVisible = Visibility.Collapsed;
            if (UngetCardPrescriptions.Count == 0)
                UngetCardVisible = Visibility.Collapsed;
            if (CooperativePrescriptions.Count == 0 && ReservedPrescriptions.Count == 0 && UngetCardPrescriptions.Count == 0)
                Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
            else
            {
                Messenger.Default.Send(new NotificationMessage("ShowCustomPrescription"));
            }
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<Prescription>(this,"ReservedSelected", GetReservePrescription);
            Messenger.Default.Register<Prescription>(this, "CooperativeSelected", GetCooperativePrescription);
        }

        private void GetReservePrescription(Prescription p)
        {
            isSelectCooperative = false;
            SelectedPrescription = p;
        }

        private void GetCooperativePrescription(Prescription p)
        {
            isSelectCooperative = true;
            SelectedPrescription = p;
        }

        private void CustomPrescriptionSelected()
        {
            SelectedPrescription.GetCompletePrescriptionData(true);
            Messenger.Default.Send(SelectedPrescription,"CustomPrescriptionSelected");
            Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
        }
    }
}
