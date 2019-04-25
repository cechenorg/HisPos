using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.CustomerPrescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using Cus = His_Pos.NewClass.Person.Customer.Customer;
using IcCard = His_Pos.NewClass.Prescription.IcCard;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow
{
    public class CustomPrescriptionViewModel:ViewModelBase
    {
        #region Variables
        public Prescriptions CooperativePrescriptions { get; set; }
        public RegisterAndReservePrescriptions ReservedPrescriptions { get; set; }
        public RegisterAndReservePrescriptions RegisteredPrescriptions { get; set; }
        public CustomerPrescriptions UngetCardPrescriptions { get; set; }
        public CustomPrescriptionStruct SelectedPrescription { get; set; }
        public IcCard Card { get; set; }
        public int PatientID { get; }
        public string PatientIDNumber { get; }
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

        private Visibility registeredVisible;

        public Visibility RegisteredVisible
        {
            get => registeredVisible;
            set
            {
                Set(() => RegisteredVisible, ref registeredVisible, value);
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
        private bool showDialog;
        public bool ShowDialog
        {
            get => showDialog;
            set
            {
                Set(() => ShowDialog, ref showDialog, value);
            }
        }
        #endregion
        public RelayCommand MakeUpCard { get; set; }
        public CustomPrescriptionViewModel(int cusID, string cusIDNumber, IcCard card)
        {
            Card = new IcCard();
            Card = card.DeepCloneViaJson();
            PatientID = cusID;
            PatientIDNumber = cusIDNumber;
            RegisterMessenger();
            InitializePrescription();
            MakeUpCard = new RelayCommand(MakeUpCardAction);
        }

        private void InitializePrescription()
        {
            CooperativePrescriptions = new Prescriptions();
            MainWindow.ServerConnection.OpenConnection();
            CooperativePrescriptions.GetCooperativePrescriptionsByCusIDNumber(PatientIDNumber);
            CooperativePrescriptions.GetXmlOfPrescriptionsByCusIDNumber(PatientIDNumber);
            ReservedPrescriptions = new RegisterAndReservePrescriptions();
            ReservedPrescriptions.GetReservePrescriptionByCusId(PatientID);
            RegisteredPrescriptions = new RegisterAndReservePrescriptions();
            RegisteredPrescriptions.GetRegisterPrescriptionByCusId(PatientID);
            UngetCardPrescriptions = new CustomerPrescriptions();
            UngetCardPrescriptions.GetPrescriptionsNoGetCardByCusId(PatientID);
            MainWindow.ServerConnection.CloseConnection();
            if (CooperativePrescriptions.Count == 0)
                CooperativeVisible = Visibility.Collapsed;
            if(ReservedPrescriptions.Count == 0)
                ReservedVisible = Visibility.Collapsed;
            if (RegisteredPrescriptions.Count == 0)
                RegisteredVisible = Visibility.Collapsed;
            if (UngetCardPrescriptions.Count == 0 || string.IsNullOrEmpty(Card.CardNumber))
                UngetCardVisible = Visibility.Collapsed;
            if (CooperativePrescriptions.Count > 0 || ReservedPrescriptions.Count > 0 || RegisteredPrescriptions.Count > 0 || (UngetCardPrescriptions.Count > 0 && !string.IsNullOrEmpty(Card.CardNumber)))
                ShowDialog = true;
            else
            {
                ShowDialog = false;
                Messenger.Default.Unregister(this);
            }
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<NotificationMessage<int>> (this, PrescriptionSelectionChanged);
            Messenger.Default.Register<CustomPrescriptionStruct>(this, "CooperativePrescriptionSelectionChanged", CooperativePrescriptionSelectionChanged);
            Messenger.Default.Register<NotificationMessage>("CustomPrescriptionSelected", CustomPrescriptionSelected);
        }

        private void PrescriptionSelectionChanged(NotificationMessage<int> msg)
        {
            isSelectCooperative = false;
            SelectedPrescription = msg.Notification.Equals("ReserveSelectionChanged") ? 
                new CustomPrescriptionStruct(msg.Content, PrescriptionSource.ChronicReserve,string.Empty) : new CustomPrescriptionStruct(msg.Content, PrescriptionSource.Normal, string.Empty);
        }
        private void CooperativePrescriptionSelectionChanged(CustomPrescriptionStruct cps)
        {
            isSelectCooperative = true;
            SelectedPrescription = cps;
        }

        private void CustomPrescriptionSelected(NotificationMessage msg)
        {
            try
            {
                if (!msg.Notification.Equals("CustomPrescriptionSelected")) return;
                if (SelectedPrescription.ID is null) return;
                Messenger.Default.Unregister<NotificationMessage>("CustomPrescriptionSelected", CustomPrescriptionSelected);
                switch(SelectedPrescription.Source)
                {
                    case PrescriptionSource.Cooperative:
                        Messenger.Default.Send(new NotificationMessage<Prescription>(this, CooperativePrescriptions.Single(p => p.Remark.Equals(SelectedPrescription.Remark)), "CooperativePrescriptionSelected"));
                        break;
                    case PrescriptionSource.XmlOfPrescription:
                        Messenger.Default.Send(new NotificationMessage<Prescription>(this, CooperativePrescriptions.Single(p => p.SourceId == SelectedPrescription.ID.ToString() ), "CooperativePrescriptionSelected"));
                        break;
                    default:
                        Messenger.Default.Send(SelectedPrescription, "PrescriptionSelected");
                        break;
                }
                Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
                Messenger.Default.Unregister(this);
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage("代入處方發生問題，為確保處方資料完整請重新取得病患資料並代入處方。", MessageType.WARNING);
            }
        }

        private void MakeUpCardAction()
        {
            MessageWindow.ShowMessage("補卡作業進行時請勿拔起卡片，以免補卡異常",MessageType.WARNING);
            int deposit = 0;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                foreach (var p in UngetCardPrescriptions)
                {
                    var pre = new Prescription(PrescriptionDb.GetPrescriptionByID(p.ID).Rows[0], PrescriptionSource.Normal);
                    pre.GetCompletePrescriptionData( false,true);
                    deposit += pre.PrescriptionPoint.Deposit;
                    ReadCard(pre);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                MessageWindow.ShowMessage("補卡作業成功，共" + UngetCardPrescriptions.Count +"張，退還押金" + deposit + "元", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ReadCard(Prescription p)
        {
            var isGetCard = false;
            BusyContent = StringRes.讀取健保卡;
            isGetCard = p.GetCard();
            if (isGetCard)
            {
                p.PrescriptionStatus.IsGetCard = true;
                BusyContent = StringRes.檢查就醫次數;
                p.Card.GetRegisterBasic();
                if (p.Card.AvailableTimes != null)
                {
                    if (p.Card.AvailableTimes == 0)
                    {
                        BusyContent = StringRes.更新卡片;
                        p.Card.UpdateCard();
                    }
                }
                BusyContent = StringRes.取得就醫序號;
                p.Card.GetMedicalNumber(2);
                p.Treatment.GetLastMedicalNumber();
                if (!string.IsNullOrEmpty(p.Treatment.TempMedicalNumber))
                {
                    if (p.Treatment.ChronicSeq is null)
                        p.Treatment.MedicalNumber = p.Treatment.TempMedicalNumber;
                    else
                    {
                        if (p.Treatment.ChronicSeq > 1)
                        {
                            p.Treatment.MedicalNumber = "IC0" + p.Treatment.ChronicSeq;
                            p.Treatment.OriginalMedicalNumber = p.Treatment.TempMedicalNumber;
                        }
                        else
                        {
                            p.Treatment.MedicalNumber = p.Treatment.TempMedicalNumber;
                            p.Treatment.OriginalMedicalNumber = null;
                        }
                    }
                }
                if (CreatePrescriptionSign(p, isGetCard))
                    HisApiFunction.CreatDailyUploadData(p, true);
            }
            p.PrescriptionStatus.IsGetCard = true;
            p.PrescriptionStatus.IsDeposit = false;
            p.PrescriptionStatus.IsDeclare = true;
            p.PrescriptionStatus.IsAdjust = true;
            MainWindow.ServerConnection.OpenConnection();
            //var deposit = p.Treatment.Institution.ID == ViewModelMainWindow.CooperativeInstitutionID ? "合作退還押金" : "退還押金";
            //PrescriptionDb.ProcessCashFlow(deposit, "PreMasId", p.Id, p.PrescriptionPoint.Deposit * -1);
            p.PrescriptionPoint.Deposit = 0;
            p.PrescriptionStatus.UpdateStatus(p.Id);
            p.Treatment.CheckMedicalNumber(false);
            p.Update();
            MainWindow.ServerConnection.CloseConnection();
        }
        private bool CreatePrescriptionSign(Prescription p,bool isGetCard)
        {
            BusyContent = StringRes.寫卡;
            if (isGetCard)
                p.PrescriptionSign = HisApiFunction.WritePrescriptionData(p);
            else
            {
                p.PrescriptionSign = new List<string>();
            }
            //if (HisApiFunction.OpenCom())
            //{
            //    HisApiBase.csSoftwareReset(3);
            //    HisApiFunction.CloseCom();
            //}
            BusyContent = StringRes.產生每日上傳資料;
            if (p.PrescriptionSign.Count != p.Medicines.Count(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf))
            {
                bool? isDone = null;
                ErrorUploadWindowViewModel.IcErrorCode errorCode;
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage(StringRes.寫卡異常, MessageType.ERROR);
                    var e = new ErrorUploadWindow(p.Card.IsGetMedicalNumber); //詢問異常上傳
                    e.ShowDialog();
                    while (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
                    {
                        e = new ErrorUploadWindow(p.Card.IsGetMedicalNumber);
                        e.ShowDialog();
                    }
                    errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
                    if (isDone is null)
                        HisApiFunction.CreatErrorDailyUploadData(p, true, errorCode);
                    isDone = true;
                });
                return false;
            }
            return true;
        }
    }
}
