using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using StringRes = His_Pos.Properties.Resources;
using MedSelectWindow = His_Pos.FunctionWindow.AddProductWindow.AddMedicineWindow;
using HisAPI = His_Pos.HisApi.HisApiFunction;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    public class PrescriptionEditViewModel:ViewModelBase
    {
        private Prescription OriginalPrescription { get; set; }
        private Prescription editedPrescription;
        public Prescription EditedPrescription
        {
            get => editedPrescription;
            set
            {
                Set(() => EditedPrescription, ref editedPrescription, value);
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
        private bool isGetCard;
        public bool IsGetCard
        {
            get => isGetCard;
            private set
            {
                Set(() => IsGetCard, ref isGetCard, value);
            }
        }
        private bool CheckEdit()
        {
            var preEdited = !EditedPrescription.PublicInstancePropertiesEqual(OriginalPrescription);
            var pharmacyEdited = !EditedPrescription.Treatment.Pharmacist.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Pharmacist);
            var adjustDateEdited = DateTime.Compare((DateTime)EditedPrescription.Treatment.AdjustDate, (DateTime)OriginalPrescription.Treatment.AdjustDate) != 0;
            var medEdited = false;
            if (OriginalPrescription.Medicines.Count != EditedPrescription.Medicines.Count(m=>m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial))
                medEdited = true;
            else
            {
                for (var i = 0; i < EditedPrescription.Medicines.Count(m => m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial); i++)
                {
                    medEdited = !OriginalPrescription.Medicines[i].PublicInstancePropertiesEqual(EditedPrescription.Medicines[i]);
                    if(medEdited)
                        break;
                }
            }
            var amountSelfPayEdited = OriginalPrescription.PrescriptionPoint.AmountSelfPay != EditedPrescription.PrescriptionPoint.AmountSelfPay;
            if (!NotPrescribe)
            {
                return preEdited || pharmacyEdited || adjustDateEdited || medEdited || amountSelfPayEdited;
            }
            var medicalNumberEdited = !EditedPrescription.Treatment.TempMedicalNumber.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.TempMedicalNumber);
            bool treatDateEdited;
            if(OriginalPrescription.Treatment.TreatDate is null && EditedPrescription.Treatment.TreatDate != null || EditedPrescription.Treatment.TreatDate is null && OriginalPrescription.Treatment.TreatDate != null)
                treatDateEdited = true;
            else
            {
                treatDateEdited = DateTime.Compare((DateTime) EditedPrescription.Treatment.TreatDate,
                                      (DateTime) OriginalPrescription.Treatment.TreatDate) != 0;
            }
            var insEdited = !EditedPrescription.Treatment.Institution.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Institution);
            var divEdited = !EditedPrescription.Treatment.Division.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Division);
            var mainDiseaseEdited = !EditedPrescription.Treatment.MainDisease.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.MainDisease);
            var subDiseaseEdited = !EditedPrescription.Treatment.SubDisease.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.SubDisease);
            var adjCaseEdited = !EditedPrescription.Treatment.AdjustCase.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.AdjustCase);
            var preCaseEdited = !EditedPrescription.Treatment.PrescriptionCase.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.PrescriptionCase);
            var payCatEdited = !EditedPrescription.Treatment.PaymentCategory.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.PaymentCategory);
            var copEdited = !EditedPrescription.Treatment.Copayment.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Copayment);
            var payEdited = !EditedPrescription.Treatment.PaymentCategory.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.PaymentCategory);
            var speTreEdited = !EditedPrescription.Treatment.SpecialTreat.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.SpecialTreat);
            bool chronicSeqEdited;
            if (OriginalPrescription.Treatment.ChronicSeq is null && EditedPrescription.Treatment.ChronicSeq != null || EditedPrescription.Treatment.ChronicSeq is null && OriginalPrescription.Treatment.ChronicSeq != null)
                chronicSeqEdited = true;
            else
            {
                chronicSeqEdited = OriginalPrescription.Treatment.ChronicSeq == EditedPrescription.Treatment.ChronicSeq;
            }
            bool chronicTotalEdited;
            if (OriginalPrescription.Treatment.ChronicTotal is null && EditedPrescription.Treatment.ChronicTotal != null || EditedPrescription.Treatment.ChronicTotal is null && OriginalPrescription.Treatment.ChronicTotal != null)
                chronicTotalEdited = true;
            else
            {
                chronicTotalEdited = OriginalPrescription.Treatment.ChronicTotal == EditedPrescription.Treatment.ChronicTotal;
            }
            return preEdited || insEdited || divEdited || pharmacyEdited || treatDateEdited || adjustDateEdited || mainDiseaseEdited || subDiseaseEdited || adjCaseEdited || preCaseEdited || copEdited
                   || payEdited || speTreEdited || medEdited || medicalNumberEdited || payCatEdited || amountSelfPayEdited || chronicSeqEdited || chronicTotalEdited;
            
        }

        private MedSelectWindow MedicineWindow { get; set; }
        private bool isEdit;
        public bool IsEdit 
        {
            get => isEdit;
            set
            {
                Set(() => IsEdit, ref isEdit, value);
            }
        }
        public int previousSelectedIndex { get; set; }
        private int selectedMedicinesIndex;
        public int SelectedMedicinesIndex
        {
            get => selectedMedicinesIndex;
            set
            {
                if (value != -1)
                {
                    Set(() => SelectedMedicinesIndex, ref selectedMedicinesIndex, value);
                }
            }
        }
        private ViewModelEnum viewModel { get; set; }
        private bool notPrescribe;
        public bool NotPrescribe
        {
            get => notPrescribe;
            set
            {
                Set(() => NotPrescribe, ref notPrescribe, value);
            }
        }
        public double WindowWidth
        {
            get => SystemParameters.WorkArea.Width * 0.85;
            set {}
        }
        public double WindowHeight
        {
            get => SystemParameters.WorkArea.Height * 0.85;
            set {}
        }
        public double StartTop
        {
            get => (SystemParameters.WorkArea.Height - WindowHeight) / 2;
            set { }
        }
        public double StartLeft
        {
            get => (SystemParameters.WorkArea.Width - WindowWidth) / 2;
            set { }
        }
        #region Commands
        public RelayCommand PrintMedBagCmd { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> GetMainDiseaseCodeById { get; set; }
        public RelayCommand<string> GetSubDiseaseCodeById { get; set; }
        public RelayCommand AdjustCaseSelectionChanged { get; set; }
        public RelayCommand CopaymentSelectionChanged { get; set; }
        public RelayCommand ShowCommonInstitutionSelectionWindow { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand MedicinePriceChanged { get; set; }
        public RelayCommand RedoEdit { get; set; }
        public RelayCommand EditComplete { get; set; }
        public RelayCommand CheckDataChanged { get; set; }
        public RelayCommand MakeUpClick { get; set; }
        public RelayCommand PrintDepositSheet { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand PrintReceiptCmd { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        #endregion
        #region ItemsSources
        public Institutions Institutions { get; set; }
        public Divisions Divisions { get; set; }
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
        #endregion
        public PrescriptionEditViewModel(Prescription selected, ViewModelEnum vm)
        {
            NotPrescribe = !selected.Treatment.AdjustCase.ID.Equals("0");
            OriginalPrescription = selected;
            OriginalPrescription.PrescriptionPoint.GetAmountPaySelf(OriginalPrescription.Id);
            Init((Prescription)OriginalPrescription.Clone());
            IsGetCard = !NotPrescribe || EditedPrescription.PrescriptionStatus.IsGetCard;
            viewModel = vm;
        }
        #region InitialFunctions
        /*
         * clone checkEdit
         */
        private void Init(Prescription selected)
        {
            InitPrescription(selected);
            IsEdit = false;
            InitialItemsSources();
            InitialCommandActions();
        }

        private void InitPrescription(Prescription selected)
        {
            MainWindow.ServerConnection.OpenConnection();
            selected.Patient = selected.Patient.GetCustomerByCusId(selected.Patient.ID);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription = selected;
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.AdjustMedicinesType();
            MainWindow.ServerConnection.CloseConnection();
            if (EditedPrescription.Treatment.Division != null)
                EditedPrescription.Treatment.Division = VM.GetDivision(EditedPrescription.Treatment.Division?.ID);
            EditedPrescription.Treatment.Pharmacist =
                VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IdNumber.Equals(EditedPrescription.Treatment.Pharmacist.IdNumber));
            EditedPrescription.Treatment.AdjustCase = VM.GetAdjustCase(EditedPrescription.Treatment.AdjustCase.ID);
            EditedPrescription.Treatment.Copayment = VM.GetCopayment(EditedPrescription.Treatment.Copayment?.Id);
            if (EditedPrescription.Treatment.PrescriptionCase != null)
                EditedPrescription.Treatment.PrescriptionCase = VM.GetPrescriptionCases(EditedPrescription.Treatment.PrescriptionCase?.ID);
            if (EditedPrescription.Treatment.SpecialTreat != null)
                EditedPrescription.Treatment.SpecialTreat = VM.GetSpecialTreat(EditedPrescription.Treatment.SpecialTreat?.ID);
            EditedPrescription.PrescriptionPoint.GetDeposit(EditedPrescription.Id);
            EditedPrescription.CheckIsCooperative();
        }

        private void InitialItemsSources()
        {
            Institutions = VM.Institutions;
            Divisions = VM.Divisions;
            MedicalPersonnels = VM.CurrentPharmacy.MedicalPersonnels;
            AdjustCases = VM.AdjustCases;
            PaymentCategories = VM.PaymentCategories;
            PrescriptionCases = VM.PrescriptionCases;
            Copayments = VM.Copayments;
            SpecialTreats = VM.SpecialTreats;
        }
        private void InitialCommandActions()
        {
            PrintMedBagCmd = new RelayCommand(PrintMedBagAction);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ShowCommonInsSelectionWindowAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            GetMainDiseaseCodeById = new RelayCommand<string>(GetMainDiseaseCodeByIdAction);
            GetSubDiseaseCodeById = new RelayCommand<string>(GetSubDiseaseCodeByIdAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction,CheckIsNotPrescribe);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            RedoEdit = new RelayCommand(RedoEditAction);
            EditComplete = new RelayCommand(EditCompleteAction);
            CheckDataChanged = new RelayCommand(CheckEditStatus);
            MakeUpClick = new RelayCommand(MakeUpClickAction);
            PrintDepositSheet = new RelayCommand(PrintDepositSheetAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            PrintReceiptCmd = new RelayCommand(PrintReceiptAction);
            Delete = new RelayCommand(DeleteAction);
            MedicineAmountChanged = new RelayCommand(SetBuckleAmount);
        }
        #endregion
        #region CommandActions
        private void PrintMedBagAction()
        {
            var printConfirmResult = NewFunction.CheckPrint(EditedPrescription);
            var printMedBag = printConfirmResult[0];
            var printSingle = printConfirmResult[1];
            var printReceipt = printConfirmResult[2];
            if (printMedBag is null || printReceipt is null)
                return;
            if ((bool)printMedBag && printSingle is null)
                return;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if ((bool)printMedBag)
                    PrintMedBag(false, (bool)printMedBag, (bool)printSingle, (bool)printReceipt);
            };
            worker.RunWorkerAsync();
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionEditViewModel) + "InsSelected", GetSelectedInstitution);
            var commonInsSelectionWindow = new CommonHospitalsWindow(ViewModelEnum.PrescriptionEdit);
            commonInsSelectionWindow.ShowDialog();
        }
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(Resources.搜尋字串長度不足 + "4", MessageType.WARNING);
                return;
            }
            if (EditedPrescription.Treatment.Institution != null && !string.IsNullOrEmpty(EditedPrescription.Treatment.Institution.FullName) && search.Equals(EditedPrescription.Treatment.Institution.FullName))
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusDivision"));
                return;
            }
            EditedPrescription.Treatment.Institution = null;
            var result = Institutions.Where(i => i.ID.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    EditedPrescription.Treatment.Institution = result[0];
                    break;
                default:
                    Messenger.Default.Register<Institution>(this, nameof(PrescriptionEditViewModel) + "InsSelected", GetSelectedInstitution);
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search,ViewModelEnum.PrescriptionEdit);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void GetMainDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!string.IsNullOrEmpty(EditedPrescription.Treatment.MainDisease.FullName) && id.Equals(EditedPrescription.Treatment.MainDisease.FullName))
            {
                Messenger.Default.Send(new NotificationMessage(this,"FocusSubDisease"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                EditedPrescription.Treatment.MainDisease = result;
            }
            CheckEditStatus();
        }

        private void GetSubDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id) || (!string.IsNullOrEmpty(EditedPrescription.Treatment.MainDisease.FullName) && id.Equals(EditedPrescription.Treatment.MainDisease.FullName)))
            {
                Messenger.Default.Send(new NotificationMessage(this,"FocusChronicTotal"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                EditedPrescription.Treatment.SubDisease = result;
            }
            CheckEditStatus();
        }
        private void AdjustCaseSelectionChangedAction()
        {
            if (EditedPrescription.Treatment.AdjustCase != null &&
                EditedPrescription.Treatment.AdjustCase.ID.Equals("0"))
            {
                EditedPrescription.Treatment.Clear();
                SetMedicinesPaySelf();
            }
            EditedPrescription.CheckPrescriptionVariable();
            CheckEditStatus();
        }

        private void CopaymentSelectionChangedAction()
        {
            if (!EditedPrescription.CheckFreeCopayment())
            {
                if (EditedPrescription.PrescriptionPoint.MedicinePoint <= 100)
                    EditedPrescription.Treatment.Copayment = VM.GetCopayment("I21");
                else
                {
                    EditedPrescription.Treatment.Copayment = VM.GetCopayment("I20");
                }
            }
            CheckEditStatus();
        }
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionEdit);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit);
                MedicineWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit);
            }
            else
            {
                MessageWindow.ShowMessage(StringRes.查無藥品, MessageType.WARNING);
            }
        }
        private void MakeUpClickAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                ReadCard();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void CountMedicinePoint()
        {
            EditedPrescription.CountPrescriptionPoint(true);
        }
        private void EditCompleteAction()
        {
            if (IsEdit)
            {
                if (!CheckSameMedicine())return;
                if (!EditedPrescription.Treatment.AdjustCase.ID.Equals("0"))
                {
                    var error = EditedPrescription.CheckPrescriptionRule(true);
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageWindow.ShowMessage(error, MessageType.ERROR);
                        return;
                    }
                }
                EditedPrescription.CountPrescriptionPoint(false);
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.Update();
                if (EditedPrescription.Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
                {
                    EditedPrescription.AdjustCooperativeMedicines(OriginalPrescription); 
                }
                else
                    EditedPrescription.AdjustMedicines(OriginalPrescription);
                MainWindow.ServerConnection.CloseConnection();
                MessageWindow.ShowMessage("編輯成功",MessageType.SUCCESS);
                //switch (viewModel)
                //{
                //    case ViewModelEnum.PrescriptionSearch:
                //        Messenger.Default.Send(new NotificationMessage(nameof(PrescriptionSearchViewModel)+ "PrescriptionEdited"));
                //        break;
                //    case ViewModelEnum.DeclareFileManage:
                //        Messenger.Default.Send(new NotificationMessage(nameof(DeclareFileManageViewModel) + "PrescriptionEdited"));
                //        break;
                //}
            }
            Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
        }

        private void RedoEditAction()
        {
            InitPrescription((Prescription)OriginalPrescription.Clone());
            IsEdit = false;
        }

        private void DeleteAction()
        {
            ConfirmWindow deleteConfirm = new ConfirmWindow("確定刪除此處方?","刪除確認");
            var delete = deleteConfirm.DialogResult;
            if((bool)delete)
            {
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.Delete();
                MainWindow.ServerConnection.CloseConnection();
                Messenger.Default.Send(new NotificationMessage(nameof(PrescriptionSearchViewModel) + "PrescriptionEdited"));
                Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
            }
        }

        private void PrintDepositSheetAction()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.押金單據列印;
                EditedPrescription.PrescriptionPoint.CountDeposit();
                //EditedPrescription.PrescriptionPoint.GetDeposit(EditedPrescription.Id);
                EditedPrescription.PrintDepositSheet();
            };
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private void PrintReceiptAction()
        {
            var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm, true);
            var printReceipt = receiptResult.DialogResult;
            if (!(bool)printReceipt)
                return;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                EditedPrescription.PrescriptionPoint.ActualReceive = EditedPrescription.PrescriptionPoint.AmountSelfPay + EditedPrescription.PrescriptionPoint.CopaymentPoint;
                BusyContent = StringRes.收據列印;
                EditedPrescription.PrintReceipt();
            };
            IsBusy = true;
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            worker.RunWorkerAsync();
        }
        #endregion
        #region MessengerReceive
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this, nameof(PrescriptionEditViewModel) + "InsSelected", GetSelectedInstitution);
            EditedPrescription.Treatment.Institution = receiveSelectedInstitution;
            EditedPrescription.CheckIsCooperative();
            CheckEditStatus();
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionEditViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.AddMedicineBySearch(msg.Content.ID);
            MainWindow.ServerConnection.CloseConnection();
            EditedPrescription.CountPrescriptionPoint(true);
            CheckEditStatus();
        }

        private void DeleteMedicineAction()
        {
            EditedPrescription.Medicines.Remove(EditedPrescription.SelectedMedicine);
            CountMedicinePoint();
            CheckEditStatus();
        }
        private void SetBuckleAmount()
        {
            if (EditedPrescription.Treatment.Institution.ID.Equals(VM.CooperativeInstitutionID))
                EditedPrescription.SelectedMedicine.BuckleAmount = 0;
            else
            {
                EditedPrescription.SelectedMedicine.BuckleAmount = EditedPrescription.SelectedMedicine.Amount;
            }
            CheckEditStatus();
        }
        private bool CheckSameMedicine()
        {
            var medicinesSame = EditedPrescription.CheckSameMedicine();
            if (string.IsNullOrEmpty(medicinesSame)) return true;
            MessageWindow.ShowMessage(medicinesSame, MessageType.WARNING);
            return false;
        }
        #endregion
        #region Functions
        private void SetMedicinesPaySelf()
        {
            var medList = EditedPrescription.Medicines.Where(m => m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial).ToList();
            if (medList.Count > 0)
            {
                foreach (var m in medList)
                {
                    if (m.PaySelf) continue;
                    m.PaySelf = true;
                }
            }
        }
        private void CheckEditStatus()
        {
            IsEdit = true;
        }
        private void ReadCard()
        {
            var isGetCard = false;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = StringRes.讀取健保卡;
                isGetCard = EditedPrescription.GetCard();
                if (isGetCard)
                {
                    EditedPrescription.Treatment.GetLastMedicalNumber();
                    EditedPrescription.PrescriptionStatus.IsGetCard = true;
                    BusyContent = StringRes.檢查就醫次數;
                    EditedPrescription.Card.GetRegisterBasic();
                    if (EditedPrescription.Card.AvailableTimes != null)
                    {
                        if (EditedPrescription.Card.AvailableTimes == 0)
                        {
                            BusyContent = StringRes.更新卡片;
                            EditedPrescription.Card.UpdateCard();
                        }
                    }
                    BusyContent = StringRes.取得就醫序號;
                    EditedPrescription.Card.GetMedicalNumber(2);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                ErrorUploadWindowViewModel.IcErrorCode errorCode = null;
                if (!EditedPrescription.Card.IsGetMedicalNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        var e = new ErrorUploadWindow(EditedPrescription.Card.IsGetMedicalNumber); //詢問異常上傳
                        e.ShowDialog();
                        errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
                    }));
                    if (errorCode is null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            MessageWindow.ShowMessage("未選擇異常代碼，請重新過卡或選擇異常代碼", MessageType.WARNING);
                        }));
                        return;
                    }
                }
                CreateDailyUploadData(errorCode);

                if (EditedPrescription.PrescriptionStatus.IsCreateSign is null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MessageWindow.ShowMessage("寫卡異常，請重新讀取卡片或選擇異常代碼。", MessageType.ERROR);
                    }));
                    return;
                }
                EditedPrescription.PrescriptionStatus.SetNormalAdjustStatus();
                if (EditedPrescription.Card.IsGetMedicalNumber)
                {
                    if (EditedPrescription.PrescriptionStatus.IsCreateSign != null && (bool)EditedPrescription.PrescriptionStatus.IsCreateSign)
                    {
                        HisAPI.CreatDailyUploadData(EditedPrescription, false);
                    }
                }
                else if (EditedPrescription.PrescriptionStatus.IsCreateSign != null && !(bool)EditedPrescription.PrescriptionStatus.IsCreateSign)
                {
                    HisAPI.CreatErrorDailyUploadData(EditedPrescription, false, errorCode);
                }
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.PrescriptionPoint.GetDeposit(EditedPrescription.Id);
                PrescriptionDb.ProcessCashFlow("退還押金", "PreMasId", EditedPrescription.Id, EditedPrescription.PrescriptionPoint.Deposit * -1);
                EditedPrescription.PrescriptionStatus.UpdateStatus(EditedPrescription.Id);
                MainWindow.ServerConnection.CloseConnection();
                CheckEditStatus();
                IsBusy = false;
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage("補卡作業成功，退還押金" + EditedPrescription.PrescriptionPoint.Deposit + "元", MessageType.SUCCESS);
                });
            };
            worker.RunWorkerAsync();
        }
        private void CreateDailyUploadData(ErrorUploadWindowViewModel.IcErrorCode error = null)
        {
            if (EditedPrescription.PrescriptionStatus.IsGetCard || error != null)
            {
                if (EditedPrescription.Card.IsGetMedicalNumber)
                {
                    CreatePrescriptionSign();
                }
                else
                {
                    EditedPrescription.PrescriptionStatus.IsCreateSign = false;
                }
            }
        }
        private void CreatePrescriptionSign()
        {
            BusyContent = StringRes.寫卡;
            EditedPrescription.PrescriptionSign = HisAPI.WritePrescriptionData(EditedPrescription);
            BusyContent = StringRes.產生每日上傳資料;
            if (EditedPrescription.WriteCardSuccess != 0)
            {
                Application.Current.Dispatcher.Invoke(delegate {
                    var description = MainWindow.GetEnumDescription((ErrorCode)EditedPrescription.WriteCardSuccess);
                    MessageWindow.ShowMessage("寫卡異常 " + EditedPrescription.WriteCardSuccess + ":" + description, MessageType.WARNING);
                });
                EditedPrescription.PrescriptionStatus.IsCreateSign = null;
            }
            else
            {
                EditedPrescription.PrescriptionStatus.IsCreateSign = true;
            }
        }
        private void PrintMedBag(bool noCard, bool printMedBag, bool printSingle, bool printReceipt)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if (printMedBag)
                {
                    BusyContent = "藥袋列印中...";
                    EditedPrescription.PrintMedBag(printSingle);
                    if (printReceipt)
                    {
                        EditedPrescription.PrescriptionPoint.ActualReceive = EditedPrescription.PrescriptionPoint.AmountSelfPay + EditedPrescription.PrescriptionPoint.CopaymentPoint;
                        BusyContent = StringRes.收據列印;
                        EditedPrescription.PrintReceipt();
                    }
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private bool CheckIsNotPrescribe()
        {
            return NotPrescribe;
        }

        #endregion
    }
}
