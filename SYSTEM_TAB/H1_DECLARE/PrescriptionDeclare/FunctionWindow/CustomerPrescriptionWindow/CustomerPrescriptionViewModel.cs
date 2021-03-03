using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.CustomerPrescriptions;
using His_Pos.NewClass.Prescription.ICCard;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.ComponentModel;
using Application = System.Windows.Application;
using Prescription = His_Pos.NewClass.Prescription.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerPrescriptionWindow
{
    public enum CustomerPrescriptionType
    {
        Cooperative = 1,
        Register = 2,
        Reserve = 3
    }

    public class CustomerPrescriptionViewModel : ViewModelBase
    {
        #region Variable

        private string windowTitle;

        public string WindowTitle
        {
            get => windowTitle;
            set
            {
                Set(() => WindowTitle, ref windowTitle, value);
            }
        }

        private string selectedRadioButton;

        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set
            {
                Set(() => SelectedRadioButton, ref selectedRadioButton, value);
                if (string.IsNullOrEmpty(selectedRadioButton)) return;
                switch (selectedRadioButton)
                {
                    case "Option1":
                        SelectedType = CustomerPrescriptionType.Cooperative;
                        break;

                    case "Option2":
                        SelectedType = CustomerPrescriptionType.Register;
                        break;

                    case "Option3":
                        SelectedType = CustomerPrescriptionType.Reserve;
                        break;
                }
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

        public IcCard Card { get; set; }
        private Customer patient;

        public Customer Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
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

        private CustomerPrescriptionType selectedType;

        public CustomerPrescriptionType SelectedType
        {
            get => selectedType;
            set
            {
                Set(() => SelectedType, ref selectedType, value);
            }
        }

        public bool NoCardGridVisible => NoCardPres != null && NoCardPres.Count > 0;
        public bool CooperativeRadioBtnEnable => CooperativePres != null && CooperativePres.Count > 0;
        public bool RegisterRadioBtnEnable => ChronicRegisterPres != null && ChronicRegisterPres.Count > 0;
        public bool ReserveRadioBtnEnable => ChronicReservePres != null && ChronicReservePres.Count > 0;
        private CusPrePreviewBases cooperativePres;

        public CusPrePreviewBases CooperativePres
        {
            get => cooperativePres;
            set
            {
                Set(() => CooperativePres, ref cooperativePres, value);
            }
        }

        private CusPrePreviewBases chronicRegisterPres;

        public CusPrePreviewBases ChronicRegisterPres
        {
            get => chronicRegisterPres;
            set
            {
                Set(() => ChronicRegisterPres, ref chronicRegisterPres, value);
            }
        }

        private CusPrePreviewBases chronicReservePres;

        public CusPrePreviewBases ChronicReservePres
        {
            get => chronicReservePres;
            set
            {
                Set(() => ChronicReservePres, ref chronicReservePres, value);
            }
        }

        private CusPrePreviewBases noCardPres;

        public CusPrePreviewBases NoCardPres
        {
            get => noCardPres;
            set
            {
                Set(() => NoCardPres, ref noCardPres, value);
            }
        }

        private CusPrePreviewBase selectedPrescription;

        public CusPrePreviewBase SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                if (value != null)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    value.GetMedicines();
                    MainWindow.ServerConnection.CloseConnection();
                }
                Set(() => SelectedPrescription, ref selectedPrescription, value);
            }
        }

        private CusPrePreviewBase makeUpPrescription;

        public CusPrePreviewBase MakeUpPrescription
        {
            get => makeUpPrescription;
            set
            {
                Set(() => MakeUpPrescription, ref makeUpPrescription, value);
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

        private ErrorUploadWindowViewModel.IcErrorCode ErrorCode;
        private BackgroundWorker worker;
        private PrescriptionService currentService;

        #endregion Variable

        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        public RelayCommand DeleteRegisterPrescription { get; set; }

        public CustomerPrescriptionViewModel(Customer customer, IcCard card)
        {
            Patient = customer;
            Card = card.DeepCloneViaJson();
            InitCommands();
            MainWindow.ServerConnection.OpenConnection();
            InitVariable();
            MainWindow.ServerConnection.CloseConnection();
            CheckShowDialog();
        }

        private void InitCommands()
        {
            PrescriptionSelected = new RelayCommand(PrescriptionSelectedAction);
            MakeUp = new RelayCommand(MakeUpAction);
            DeleteRegisterPrescription = new RelayCommand(DeleteRegisterPrescriptionAction);
        }

        private void DeleteRegisterPrescriptionAction()
        {
            switch (SelectedPrescription)
            {
                case null:
                    MessageWindow.ShowMessage("請選擇欲刪除之處方。", MessageType.WARNING);
                    return;

                case ChronicPreview pre:
                    {
                        if (pre.Type.Equals(PrescriptionType.ChronicRegister))
                        {
                            var deleteConfirm = new ConfirmWindow("確定刪除此處方?", "刪除確認");
                            var delete = deleteConfirm.DialogResult;
                            if ((bool)delete)
                            {
                                worker = new BackgroundWorker();
                                worker.DoWork += (o, ea) =>
                                {
                                    BusyContent = "刪除處方...";
                                    MainWindow.ServerConnection.OpenConnection();
                                    MainWindow.SingdeConnection.OpenConnection();
                                    SelectedPrescription.CreatePrescription().Delete();
                                    BusyContent = "刪除處方相關訂單...";
                                    ((ChronicPreview)SelectedPrescription).DeleteOrder();
                                    BusyContent = "取得顧客處方...";
                                    Application.Current.Dispatcher.Invoke(InitVariable);
                                    MainWindow.ServerConnection.CloseConnection();
                                    MainWindow.SingdeConnection.CloseConnection();
                                };
                                worker.RunWorkerCompleted += (o, ea) =>
                                {
                                    IsBusy = false;
                                    SelectedRadioButton = "Option2";
                                    RaisePropertyChanged("CooperativeRadioBtnEnable");
                                    RaisePropertyChanged("RegisterRadioBtnEnable");
                                    RaisePropertyChanged("ReserveRadioBtnEnable");
                                    RaisePropertyChanged("SelectedRadioButton");
                                };
                                IsBusy = true;
                                worker.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            MessageWindow.ShowMessage("非登錄處方不能刪除。", MessageType.WARNING);
                        }
                        break;
                    }
                default:
                    MessageWindow.ShowMessage("非登錄處方不能刪除。", MessageType.WARNING);
                    return;
            }
        }

        private void InitVariable()
        {
            WindowTitle = Patient.Name + " 可調劑處方";
            GetPrescriptions();
            CooperativeContent = "合作 " + CooperativePres.Count + " 張";
            RegisterContent = "登錄 " + ChronicRegisterPres.Count + " 張";
            ReserveContent = "預約 " + ChronicReservePres.Count + " 張";
            SetCondition();
        }

        private void CheckShowDialog()
        {
            if (CooperativePres.Count > 0 || ChronicRegisterPres.Count > 0 || ChronicReservePres.Count > 0 || (NoCardPres.Count > 0 && !string.IsNullOrEmpty(Card.CardNumber)))
                ShowDialog = true;
            else
                ShowDialog = false;
        }

        private void GetPrescriptions()
        {
            CooperativePres = new CusPrePreviewBases();
            ChronicRegisterPres = new CusPrePreviewBases();
            ChronicReservePres = new CusPrePreviewBases();
            NoCardPres = new CusPrePreviewBases();
            CooperativePres.GetCooperativeByCusIDNumber(Patient.IDNumber);
            ChronicRegisterPres.GetRegisterByCusId(Patient.ID);
            ChronicReservePres.GetReserveByCusId(Patient.ID);
            if (!Patient.IsAnonymous())
                NoCardPres.GetNoCardByCusId(Patient.ID);
        }

        private bool CheckCardNotNull()
        {
            return Card != null && !string.IsNullOrEmpty(Card.CardNumber);
        }

        private void SetCondition()
        {
            if (CooperativePres.Count > 0)
                SelectedRadioButton = "Option1";

            if (ChronicRegisterPres.Count > 0)
                SelectedRadioButton = "Option2";

            if (CheckNoOtherPrescriptions())
                SelectedRadioButton = "Option3";
        }

        private bool CheckNoOtherPrescriptions()
        {
            return ChronicRegisterPres.Count == 0 && CooperativePres.Count == 0;
        }

        private void PrescriptionSelectedAction()
        {
            try
            {
                if (SelectedPrescription is null) return;
                Messenger.Default.Send(new NotificationMessage<Prescription>(this, SelectedPrescription.CreatePrescription(), "CustomerPrescriptionSelected"));
                Messenger.Default.Send(new NotificationMessage("CloseCustomerPrescriptionWindow"));
            }
            catch (Exception e)
            {
                NewFunction.ExceptionLog(e.Message);
                MessageWindow.ShowMessage("代入處方發生問題，為確保處方資料完整請重新取得病患資料並代入處方。", MessageType.WARNING);
            }
        }

        private void MakeUpAction()
        {
            if (MakeUpPrescription is null)
            {
                if (NoCardPres.Count == 1)
                    MakeUpPrescription = NoCardPres[0];
                else
                {
                    MessageWindow.ShowMessage("請選擇欲補卡處方。", MessageType.WARNING);
                    return;
                }
            }
            MessageWindow.ShowMessage("補卡作業進行時請勿拔起卡片，以免補卡異常", MessageType.WARNING);
            int deposit = 0;
            var result = false;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                var pre = MakeUpPrescription.CreatePrescription();
                CheckIsReadCard(pre);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void CheckIsReadCard(Prescription pre)
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (!Card.IsRead)
                    ReadCard();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StartMakeUp(pre);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ReadCard()
        {
            BusyContent = Resources.讀取健保卡;
            try
            {
                Card.Read();
            }
            catch (Exception e)
            {
                NewFunction.ExceptionLog(e.Message);
                NewFunction.ShowMessageFromDispatcher("讀卡作業異常，請重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING);
            }
        }

        private bool CheckReadCardResult(Prescription pre)
        {
            if (Card.IsRead)
            {
                GetMedicalNumber(pre);
                return true;
            }
            var result = false;
            Application.Current.Dispatcher.Invoke(() => result = AskErrorUpload());
            return result;
        }

        private void GetMedicalNumber(Prescription pre)
        {
            pre.PrescriptionStatus.IsGetCard = true;
            BusyContent = Resources.檢查就醫次數;
            Card.GetRegisterBasic();
            if (Card.CheckNeedUpdate())
            {
                BusyContent = Resources.更新卡片;
                Card.UpdateCard();
            }
            BusyContent = Resources.取得就醫序號;
            var res = Card.GetMedicalNumber(2);
            if (res == 5003)
            {
                BusyContent = Resources.更新卡片;
                Card.UpdateCard();
                BusyContent = Resources.取得就醫序號;
                Card.GetMedicalNumber(2);
            }
            if (Card.IsGetMedicalNumber)
                pre.TempMedicalNumber = Card.GetLastMedicalNumber();
        }

        private void StartMakeUp(Prescription pre)
        {
            if (CheckReadCardResult(pre))
            {
                pre.CountPrescriptionPoint();
                pre.CountSelfPay();
                pre.PrescriptionPoint.CountAmountsPay();
                pre.SetDetail();
                currentService = PrescriptionService.CreateService(pre);
                WriteCard();
                currentService.MakeUpComplete();
                Application.Current.Dispatcher.Invoke(delegate
                {
                    NoCardPres.GetNoCardByCusId(Patient.ID);
                });
                IsBusy = false;
            }
            else
            {
                NewFunction.ShowMessageFromDispatcher("補卡失敗，如卡片異常請選擇異常代碼。", MessageType.ERROR);
                IsBusy = false;
            }
        }

        private void WriteCard()
        {
            BusyContent = Resources.寫卡;
            currentService.SetCard(Card);
            currentService.CreateDailyUploadData(ErrorCode);
            if (ErrorCode != null)
                currentService.SetMedicalNumberByErrorCode(ErrorCode);
            else
            {
                currentService.SetMedicalNumber();
            }
            currentService.CheckDailyUploadMakeUp(ErrorCode);
        }

        private bool AskErrorUpload()
        {
            var e = new ErrorUploadWindow(Card.IsGetMedicalNumber);
            e.ShowDialog();
            if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage(Resources.尚未選擇異常代碼, MessageType.WARNING);
            }
            ErrorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            return ErrorCode != null;
        }
    }
}