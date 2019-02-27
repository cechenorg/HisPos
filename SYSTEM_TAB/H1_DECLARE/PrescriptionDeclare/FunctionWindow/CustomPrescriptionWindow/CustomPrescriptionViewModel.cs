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
using His_Pos.NewClass.Product.Medicine;
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
        public Prescriptions ReservedPrescriptions { get; set; }
        public Prescriptions RegisteredPrescriptions { get; set; }
        public Prescriptions UngetCardPrescriptions { get; set; }
        public Prescription SelectedPrescription { get; set; }
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
        public CustomPrescriptionViewModel(Cus cus, IcCard card)
        {
            Patient = cus;
            Card = card;
            InitializePrescription();
            MakeUpCard = new RelayCommand(MakeUpCardAction);
            RegisterMessenger();
        }

        private void InitializePrescription()
        {
            SelectedPrescription = null;
            CooperativePrescriptions = new Prescriptions();
            MainWindow.ServerConnection.OpenConnection();
            CooperativePrescriptions.GetCooperaPrescriptionsByCusIDNumber(Patient.IDNumber);
            ReservedPrescriptions = new Prescriptions();
            ReservedPrescriptions.GetReservePrescriptionByCusId(Patient.ID);
            RegisteredPrescriptions = new Prescriptions();
            RegisteredPrescriptions.GetRegisterPrescriptionByCusId(Patient.ID);
            UngetCardPrescriptions = new Prescriptions();
            UngetCardPrescriptions.GetPrescriptionsNoGetCardByCusId(Patient.ID);
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
                ShowDialog = false;
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<Prescription>(this, "PrescriptionSelectionChanged", PrescriptionSelectionChanged);
            Messenger.Default.Register<Prescription>(this, "CooperativePrescriptionSelectionChanged", CooperativePrescriptionSelectionChanged);
            Messenger.Default.Register<NotificationMessage>("PrescriptionSelected", CustomPrescriptionSelected);
        }

        private void PrescriptionSelectionChanged(Prescription p)
        {
            isSelectCooperative = false;
            SelectedPrescription = p;
        }
        private void CooperativePrescriptionSelectionChanged(Prescription p)
        {
            isSelectCooperative = true;
            SelectedPrescription = p;
        }

        private void CustomPrescriptionSelected(NotificationMessage msg)
        {
            if (!msg.Notification.Equals("PrescriptionSelected")) return;
            if (SelectedPrescription is null) return;
            Messenger.Default.Unregister(this);
            Prescription selected = new Prescription();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得處方資料...";
                if (CooperativePrescriptions.Contains(SelectedPrescription))
                    isSelectCooperative = true;
                if (isSelectCooperative)
                    selected = (Prescription)SelectedPrescription.Clone();
                else
                {
                    selected = SelectedPrescription;
                }
                if (!string.IsNullOrEmpty(Card.CardNumber))
                    selected.Card = Card;
                selected.GetCompletePrescriptionData(true, isSelectCooperative, false);
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                Messenger.Default.Send(selected, "CustomPrescriptionSelected");
                Messenger.Default.Send(new NotificationMessage("CloseCustomPrescription"));
            };
            IsBusy = true;
            worker.RunWorkerAsync();
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
                    p.GetCompletePrescriptionData(false, false,true);
                    deposit += p.PrescriptionPoint.Deposit;
                    ReadCard(p);
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
                p.Treatment.GetLastMedicalNumber();
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
                if (CreatePrescriptionSign(p, isGetCard))
                    HisApiFunction.CreatDailyUploadData(p, true);
            }
            p.PrescriptionStatus.IsGetCard = true;
            p.PrescriptionStatus.IsDeposit = false;
            p.PrescriptionStatus.IsDeclare = true;
            p.PrescriptionStatus.IsAdjust = true;
            MainWindow.ServerConnection.OpenConnection();
            PrescriptionDb.ProcessCashFlow("退還押金", "PreMasId", p.Id, p.PrescriptionPoint.Deposit * -1);
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
            if (HisApiFunction.OpenCom())
            {
                HisApiBase.csSoftwareReset(3);
                HisApiFunction.CloseCom();
            }
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
