using System;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions;
using His_Pos.NewClass.PrescriptionRefactoring.Service;
using His_Pos.Properties;
using His_Pos.Service;
using Prescription = His_Pos.NewClass.PrescriptionRefactoring.Prescription;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerPrescriptionWindow
{
    public enum CustomerPrescriptionType
    {
        Orthopedics = 0,
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
                        SelectedType = CustomerPrescriptionType.Orthopedics;
                        break;
                    case "Option2":
                        SelectedType = CustomerPrescriptionType.Cooperative;
                        break;
                    case "Option3":
                        SelectedType = CustomerPrescriptionType.Register;
                        break;
                    case "Option4":
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

        private CustomerPrescriptionType selectedType;

        public CustomerPrescriptionType SelectedType
        {
            get => selectedType;
            set
            {
                Set(() => SelectedType, ref selectedType, value);
            }
        }
        public bool UngetCardGridVisible => NoCardPres != null && NoCardPres.Count > 0;
        public bool OrthopedicsRadioBtnEnable => OrthopedicsPres != null && OrthopedicsPres.Count > 0;
        public bool CooperativeRadioBtnEnable => CooperativePres != null && CooperativePres.Count > 0;
        public bool RegisterRadioBtnEnable => ChronicRegisterPres != null && ChronicRegisterPres.Count > 0;
        public bool ReserveRadioBtnEnable => ChronicReservePres != null && ChronicReservePres.Count > 0;
        public CusPrePreviewBases OrthopedicsPres { get; set; }
        public CusPrePreviewBases CooperativePres { get; set; }
        public CusPrePreviewBases ChronicRegisterPres { get; set; }
        public CusPrePreviewBases ChronicReservePres { get; set; }
        public CusPrePreviewBases NoCardPres { get; set; }
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
        #endregion
        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrescriptionSelected { get; set; }
        
        public CustomerPrescriptionViewModel(Customer customer, IcCard card)
        {
            Patient = customer;
            Card = card.DeepCloneViaJson();
            InitCommands();
            InitVariable();
            CheckShowDialog();
        }

        private void InitCommands()
        {
            PrescriptionSelected = new RelayCommand(PrescriptionSelectedAction);
            MakeUp = new RelayCommand(MakeUpAction);
        }

        private void InitVariable()
        {
            WindowTitle = Patient.Name + " 可調劑處方";
            GetPrescriptions();
            OrthopedicsContent = "骨科 " + OrthopedicsPres.Count + " 張";
            CooperativeContent = "合作 " + CooperativePres.Count + " 張";
            RegisterContent = "登錄 " + ChronicRegisterPres.Count + " 張";
            ReserveContent = "預約 " + ChronicReservePres.Count + " 張";
            SetCondition();
        }

        private void CheckShowDialog()
        {
            if (OrthopedicsPres.Count > 0 || CooperativePres.Count > 0 || ChronicRegisterPres.Count > 0 || ChronicReservePres.Count > 0 || (NoCardPres.Count > 0 && !string.IsNullOrEmpty(Card.CardNumber)))
                ShowDialog = true;
            else
                ShowDialog = false;
        }

        private void GetPrescriptions()
        {
            OrthopedicsPres = new CusPrePreviewBases();
            CooperativePres = new CusPrePreviewBases();
            ChronicRegisterPres = new CusPrePreviewBases();
            ChronicReservePres = new CusPrePreviewBases();
            NoCardPres = new CusPrePreviewBases();
            MainWindow.ServerConnection.OpenConnection();
            OrthopedicsPres.GetOrthopedicsByCustomerIDNumber(Patient.IDNumber);
            CooperativePres.GetCooperativeByCusIDNumber(Patient.IDNumber);
            ChronicRegisterPres.GetRegisterByCusId(Patient.ID);
            ChronicReservePres.GetReserveByCusId(Patient.ID);
            if (CheckCardNotNull())
                NoCardPres.GetUngetCardByCusId(Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
        }

        private bool CheckCardNotNull()
        {
            return Card != null && !string.IsNullOrEmpty(Card.CardNumber);
        }

        private void SetCondition()
        {
            if (ChronicReservePres.Count > 0)
                SelectedRadioButton = "Option4";

            if (ChronicRegisterPres.Count > 0)
                SelectedRadioButton = "Option3";

            if (CooperativePres.Count > 0)
                SelectedRadioButton = "Option2";

            if (OrthopedicsPres.Count > 0 || CheckNoOtherPrescriptions())
                SelectedRadioButton = "Option1";
        }

        private bool CheckNoOtherPrescriptions()
        {
            return CooperativePres.Count == 0 && ChronicRegisterPres.Count == 0 && ChronicReservePres.Count == 0;
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
                Application.Current.Dispatcher.Invoke(() => MessageWindow.ShowMessage("讀卡作業異常，請重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING));
            }
        }

        private bool CheckReadCardResult(Prescription pre)
        {
            if (Card.IsRead)
            {
                GetMedicalNumber(pre);
                return true;
            }
            return AskErrorUpload();
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
            Card.GetMedicalNumber(2);
            if(Card.IsGetMedicalNumber)
                pre.TempMedicalNumber = Card.GetLastMedicalNumber();
        }

        private void StartMakeUp(Prescription pre)
        {
            if (CheckReadCardResult(pre))
            {
                pre.CountPrescriptionPoint();
                pre.SetDetail();
                currentService = PrescriptionService.CreateService(pre);
                WriteCard();
                currentService.MakeUpComplete();
                NoCardPres.GetUngetCardByCusId(Patient.ID);
                IsBusy = false;
            }
            else
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageWindow.ShowMessage("補卡失敗，如卡片異常請選擇異常代碼。", MessageType.ERROR);
                });
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

        //private void ReadCard(Prescription p)
        //{
        //    var isGetCard = false;
        //    BusyContent = Resources.讀取健保卡;
        //    isGetCard = p.GetCard();
        //    if (isGetCard)
        //    {
        //        p.GetLastMedicalNumber();
        //        p.PrescriptionStatus.IsGetCard = true;
        //        BusyContent = Resources.檢查就醫次數;
        //        p.Card.GetRegisterBasic();
        //        if (p.Card.AvailableTimes != null)
        //        {
        //            if (p.Card.AvailableTimes == 0)
        //            {
        //                BusyContent = Resources.更新卡片;
        //                p.Card.UpdateCard();
        //            }
        //        }
        //        BusyContent = Resources.取得就醫序號;
        //        p.Card.GetMedicalNumber(2);
        //        p.GetLastMedicalNumber();
        //        if (!string.IsNullOrEmpty(p.TempMedicalNumber))
        //        {
        //            if (p.ChronicSeq is null)
        //                p.MedicalNumber = p.TempMedicalNumber;
        //            else
        //            {
        //                if (p.ChronicSeq > 1)
        //                {
        //                    p.MedicalNumber = "IC0" + p.ChronicSeq;
        //                    p.OriginalMedicalNumber = p.TempMedicalNumber;
        //                }
        //                else
        //                {
        //                    p.MedicalNumber = p.TempMedicalNumber;
        //                    p.OriginalMedicalNumber = null;
        //                }
        //            }
        //        }
        //    }
        //    ErrorUploadWindowViewModel.IcErrorCode errorCode = null;
        //    if (!p.Card.IsGetMedicalNumber)
        //    {
        //        Application.Current.Dispatcher.Invoke((Action)(() =>
        //        {
        //            var e = new ErrorUploadWindow(p.Card.IsGetMedicalNumber); //詢問異常上傳
        //            e.ShowDialog();
        //            errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
        //        }));
        //        if (errorCode is null)
        //        {
        //            Application.Current.Dispatcher.Invoke((Action)(() =>
        //            {
        //                MessageWindow.ShowMessage("未選擇異常代碼，請重新過卡或選擇異常代碼", MessageType.WARNING);
        //            }));
        //            return;
        //        }
        //    }
        //    CreateDailyUploadData(p, errorCode);

        //    if (p.PrescriptionStatus.IsCreateSign is null)
        //    {
        //        Application.Current.Dispatcher.Invoke((Action)(() =>
        //        {
        //            MessageWindow.ShowMessage("寫卡異常，請重新讀取卡片或選擇異常代碼。", MessageType.ERROR);
        //        }));
        //        return;
        //    }
        //    p.PrescriptionStatus.SetNormalAdjustStatus();
        //    if (p.Card.IsGetMedicalNumber)
        //    {
        //        if (p.PrescriptionStatus.IsCreateSign != null && (bool)p.PrescriptionStatus.IsCreateSign)
        //        {
        //            HisAPI.CreatDailyUploadData(p, false);
        //        }
        //    }
        //    else if (p.PrescriptionStatus.IsCreateSign != null && !(bool)p.PrescriptionStatus.IsCreateSign)
        //    {
        //        p.TempMedicalNumber = errorCode.ID;
        //        if (p.ChronicSeq is null)
        //            p.MedicalNumber = p.TempMedicalNumber;
        //        else
        //        {
        //            if (p.ChronicSeq > 1)
        //            {
        //                p.MedicalNumber = "IC0" + p.ChronicSeq;
        //                p.OriginalMedicalNumber = p.TempMedicalNumber;
        //            }
        //            else
        //            {
        //                p.MedicalNumber = p.TempMedicalNumber;
        //                p.OriginalMedicalNumber = null;
        //            }
        //        }
        //        HisAPI.CreatErrorDailyUploadData(p, false, errorCode);
        //    }
        //    MainWindow.ServerConnection.OpenConnection();
        //    p.PrescriptionPoint.GetDeposit(p.ID);
        //    p.PrescriptionPoint.Deposit = 0;
        //    p.PrescriptionStatus.UpdateStatus(p.ID);
        //    p.Update();
        //    MainWindow.ServerConnection.CloseConnection();
        //    IsBusy = false;
        //}
        //private void CreateDailyUploadData(Prescription p, ErrorUploadWindowViewModel.IcErrorCode error = null)
        //{
        //    if (p.PrescriptionStatus.IsGetCard || error != null)
        //    {
        //        if (p.Card.IsGetMedicalNumber)
        //        {
        //            CreatePrescriptionSign(p);
        //        }
        //        else
        //        {
        //            p.PrescriptionStatus.IsCreateSign = false;
        //        }
        //    }
        //}
        //private void CreatePrescriptionSign(Prescription p)
        //{
        //    BusyContent = Resources.寫卡;
        //    p.PrescriptionSign = HisAPI.WritePrescriptionData(p);
        //    BusyContent = Resources.產生每日上傳資料;
        //    if (p.WriteCardSuccess != 0)
        //    {
        //        Application.Current.Dispatcher.Invoke(delegate {
        //            var description = MainWindow.GetEnumDescription((ErrorCode)p.WriteCardSuccess);
        //            MessageWindow.ShowMessage("寫卡異常 " + p.WriteCardSuccess + ":" + description, MessageType.WARNING);
        //        });
        //        p.PrescriptionStatus.IsCreateSign = null;
        //    }
        //    else
        //    {
        //        p.PrescriptionStatus.IsCreateSign = true;
        //    }
        //}
    }
}
