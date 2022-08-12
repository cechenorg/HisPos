using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.EditRecords;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using IcCard = His_Pos.NewClass.Prescription.ICCard.IcCard;
using MedicineVirtual = His_Pos.NewClass.Medicine.Base.MedicineVirtual;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ValueParameterNotUsed

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    public class PrescriptionEditViewModel : ViewModelBase
    {
        #region UIProperties

        public double WindowWidth
        {
            get => SystemParameters.WorkArea.Width * 0.85;
            set { }
        }

        public double WindowHeight
        {
            get => SystemParameters.WorkArea.Height * 0.95;
            set { }
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

        private string title;

        public string Title
        {
            get => title;
            private set
            {
                Set(() => Title, ref title, value);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private AddMedicineWindow MedicineWindow { get; set; }

        private bool isEdit;

        public bool IsEdit
        {
            get => isEdit;
            set
            {
                Set(() => IsEdit, ref isEdit, value);
            }
        }

        private bool customerEdited;

        public bool CustomerEdited
        {
            get => customerEdited;
            private set
            {
                Set(() => CustomerEdited, ref customerEdited, value);
            }
        }

        private bool isPrescribe;

        public bool IsPrescribe
        {
            get => isPrescribe;
            set
            {
                Set(() => IsPrescribe, ref isPrescribe, value);
            }
        }

        public bool CanMakeUp => !EditedPrescription.PrescriptionStatus.IsGetCard && EditedPrescription.InsertTime != null;
        private BackgroundWorker worker;
        private string selectedDetail;

        public string SelectedDetail
        {
            get => selectedDetail;
            set
            {
                if (value is null) return;
                if (!string.IsNullOrEmpty(value) && value.Equals("Option1"))
                {
                    if (EditedPrescription.Patient.Name.Equals("匿名"))
                    {
                        MessageWindow.ShowMessage("匿名資料不可編輯", MessageType.WARNING);
                        value = "Option2";
                    }
                }
                else if (!string.IsNullOrEmpty(value) && value.Equals("Option1"))
                {
                    CheckCustomerEdited();
                }
                Set(() => SelectedDetail, ref selectedDetail, value);
            }
        }

        public bool CanEdit => !EditedPrescription.PrescriptionStatus.IsAdjust || 
            EditedPrescription.InsertTime != null && 
            EditedPrescription.InsertTime >= DateTime.Today || VM.CurrentUser.ID == 1 || 
            VM.CurrentUser.IsPharmist();
        public bool PriceReadOnly => !CanEdit;

        #endregion UIProperties

        private IcCard currentCard;
        private PrescriptionService currentService;
        private ErrorUploadWindowViewModel.IcErrorCode errorCode;
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

        public Prescription PrintEditedPrescription { get; set; }

        private bool chronicTimesCanEdit;

        public bool ChronicTimesCanEdit
        {
            get => chronicTimesCanEdit;
            set
            {
                Set(() => ChronicTimesCanEdit, ref chronicTimesCanEdit, value);
            }
        }

        #region ItemsSources

        public Institutions Institutions { get; set; }
        public Divisions Divisions { get; set; }
        public Employees MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
        public PrescriptionEditRecords EditRecords { get; set; }
        private PrescriptionEditRecord selectedRecord;

        public PrescriptionEditRecord SelectedRecord
        {
            get => selectedRecord;
            set
            {
                Set(() => SelectedRecord, ref selectedRecord, value);
            }
        }

        #endregion ItemsSources

        #region Commands

        public RelayCommand PrintMedBag { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> CheckClearDisease { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand CopaymentSelectionChanged { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand MedicinePriceChanged { get; set; }
        public RelayCommand RedoEdit { get; set; }
        public RelayCommand EditComplete { get; set; }
        public RelayCommand DataChanged { get; set; }
        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrintDepositSheet { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand ChangeMedicineIDToMostPriced { get; set; }
        public RelayCommand PrintReceipt { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand AdjustNoBuckle { get; set; }
        public RelayCommand ResetBuckleAmount { get; set; }
        public RelayCommand CustomerDetailEdited { get; set; }
        public RelayCommand CustomerRedoEdited { get; set; }
        public RelayCommand SavePatientData { get; set; }
        public RelayCommand AdjustDateLostFocus { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }

        #endregion Commands

        public PrescriptionEditViewModel()
        {
        }

        public PrescriptionEditViewModel(Prescription p, string title)
        {
            Title = $"{title} {p.OrderContent}";
            IsEdit = false;
            OriginalPrescription = p;
            ChronicTimesCanEdit = !OriginalPrescription.AdjustCase.IsChronic();
            EditedPrescription = (Prescription)OriginalPrescription.Clone();
            if (EditedPrescription.Institution != null && EditedPrescription.Institution.ID == "3532082753")
            {
                PrintEditedPrescription = (Prescription)OriginalPrescription.PrintClone();
            }

            EditedPrescription.ID = p.ID;
            EditedPrescription.SourceId = p.SourceId;
            InitialItemsSources();
            InitialCommandActions();
            InitPrescription();
            SelectedDetail = EditedPrescription.Patient.Name.Equals("匿名") ? "Option2" : "Option1";
            Messenger.Default.Register<NotificationMessage>("UpdateUsableAmountMessage", UpdateInventories);
        }

        private void UpdateInventories(NotificationMessage msg)
        {
            if (msg.Notification == "UpdateUsableAmountMessage" && EditedPrescription != null)
            {
                if (EditedPrescription.Medicines != null && EditedPrescription.Medicines.Count > 0)
                    EditedPrescription.UpdateMedicines();
            }
        }

        private void InitPrescription()
        {
            if (EditedPrescription.Division != null)
                EditedPrescription.Division = VM.GetDivision(OriginalPrescription.Division?.ID);
            if (MedicalPersonnels.Count(m => m.IDNumber.Equals(EditedPrescription.Pharmacist.IDNumber)) == 0)
                MedicalPersonnels.Add(EditedPrescription.Pharmacist);
            EditedPrescription.Pharmacist =
                MedicalPersonnels.SingleOrDefault(p => p.IDNumber.Equals(OriginalPrescription.Pharmacist.IDNumber));
            EditedPrescription.AdjustCase = VM.GetAdjustCase(OriginalPrescription.AdjustCase.ID);
            EditedPrescription.Copayment = VM.GetCopayment(OriginalPrescription.Copayment?.Id);
            if (EditedPrescription.Institution != null &&  EditedPrescription.Institution.ID == "3532082753")
            {
                PrintEditedPrescription.Copayment = VM.GetCopayment(OriginalPrescription.Copayment?.Id);
            }
            if (OriginalPrescription.PrescriptionCase != null)
                EditedPrescription.PrescriptionCase = VM.GetPrescriptionCases(OriginalPrescription.PrescriptionCase?.ID);
            if (OriginalPrescription.PaymentCategory != null)
                EditedPrescription.PaymentCategory = VM.GetPaymentCategory(OriginalPrescription.PaymentCategory?.ID);
            if (OriginalPrescription.SpecialTreat != null)
                EditedPrescription.SpecialTreat = VM.GetSpecialTreat(OriginalPrescription.SpecialTreat?.ID);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.GetMedicines();
            EditedPrescription.UpdateMedicines();
            MainWindow.ServerConnection.CloseConnection();
            IsPrescribe = EditedPrescription.IsPrescribe;
            foreach (var m in OriginalPrescription.Medicines)
            {
                foreach (var editMed in EditedPrescription.Medicines)
                {
                    if (!editMed.ID.Equals(m.ID)) continue;
                    if (editMed.Dosage != m.Dosage) continue;
                    if (editMed.UsageName != m.UsageName) continue;
                    if (editMed.Days != m.Days) continue;
                    if (m.PositionID != editMed.PositionID) continue;
                    if (editMed.Amount != m.Amount) continue;
                    editMed.BuckleAmount = m.BuckleAmount;
                }
            }
            EditedPrescription.OrderContent = OriginalPrescription.OrderContent;
            RaisePropertyChanged("CanMakeUp");
        }

        private void InitialItemsSources()
        {
            Institutions = VM.Institutions;
            Divisions = VM.Divisions;
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(EditedPrescription.AdjustDate ?? DateTime.Today);
            AdjustCases = VM.AdjustCases;
            PaymentCategories = VM.PaymentCategories;
            PrescriptionCases = VM.PrescriptionCases;
            Copayments = VM.Copayments;
            SpecialTreats = VM.SpecialTreats;
            EditRecords = new PrescriptionEditRecords();
            EditRecords.GetData(EditedPrescription.ID.ToString());
        }

        private void InitialCommandActions()
        {
            DataChanged = new RelayCommand(DataChangedAction);
            MakeUp = new RelayCommand(MakeUpAction);
            PrintDepositSheet = new RelayCommand(PrintDepositSheetAction);
            PrintReceipt = new RelayCommand(PrintReceiptAction);
            PrintMedBag = new RelayCommand(PrintMedBagAction);
            GetInstitution = new RelayCommand<string>(GetInstitutionAction);
            GetCommonInstitution = new RelayCommand(GetCommonInstitutionAction);
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            CheckClearDisease = new RelayCommand<object>(CheckClearDiseaseAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ChangeMedicineIDToMostPriced = new RelayCommand(ChangeMedicineIDToMostPricedAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            MedicineAmountChanged = new RelayCommand(SetBuckleAmount);
            AdjustNoBuckle = new RelayCommand(AdjustNoBuckleAction);
            ResetBuckleAmount = new RelayCommand(ResetBuckleAmountAction);
            CustomerDetailEdited = new RelayCommand(CustomerDetailEditedAction);
            CustomerRedoEdited = new RelayCommand(CustomerRedoEditedAction);
            SavePatientData = new RelayCommand(SavePatientDataAction);
            Delete = new RelayCommand(DeleteAction, () => CanEdit);
            RedoEdit = new RelayCommand(RedoEditAction);
            EditComplete = new RelayCommand(EditCompleteAction);
            AdjustDateLostFocus = new RelayCommand(AdjustDateLostFocusAction);
        }

        private void DataChangedAction()
        {
            IsEdit = true;
        }

        private void MakeUpAction()
        {
            MessageWindow.ShowMessage("補卡作業進行時請勿拔起卡片，以免補卡異常", MessageType.WARNING);
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                CheckIsReadCard();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void PrintDepositSheetAction()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = Resources.押金單據列印;
                EditedPrescription.PrescriptionPoint.CountDeposit();
                EditedPrescription.PrintDepositSheet();
            };
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void PrintReceiptAction()
        {
            if (!ConfirmPrintReceipt()) return;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                EditedPrescription.PrescriptionPoint.CountAmountsPay();
                BusyContent = Resources.收據列印;
                EditedPrescription.PrintReceipt();
            };
            IsBusy = true;
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            worker.RunWorkerAsync();
        }

        private void PrintMedBagAction()
        {
            if (EditedPrescription.Institution.ID == "3532082753")
            {
                PrintEditedPrescription.Division.Name = "";
            }
            var printConfirmResult = NewFunction.CheckPrint(EditedPrescription);
            var printMedBag = printConfirmResult[0];
            var printSingle = printConfirmResult[1];
            var printReceipt = printConfirmResult[2];
            var reportFormat = Properties.Settings.Default.ReportFormat;
            if (printMedBag is null || printReceipt is null)
                return;
            if ((bool)printMedBag && printSingle is null)
                return;
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                if ((bool)printMedBag)
                {
                    BusyContent = Resources.藥袋列印;
                    switch (printSingle != null && (bool)printSingle)
                    {
                        case false:
                            if (reportFormat == MainWindow.GetEnumDescription((PrintFormat)0))
                            {
                                EditedPrescription.PrintMedBagSingleModeByCE();
                            }
                            else
                            {
                                EditedPrescription.PrintMedBagMultiMode();
                            }
                            break;
                        case true:
                            if (reportFormat == MainWindow.GetEnumDescription((PrintFormat)0))
                            {
                                EditedPrescription.PrintMedBagSingleModeByCE();
                            }
                            else
                            {
                                EditedPrescription.PrintMedBagSingleMode();
                            }
                            break;
                    }
                }
                if ((bool)printReceipt)
                {
                    BusyContent = Resources.收據列印;
                    EditedPrescription.PrintReceipt();
                }
            };
            worker.RunWorkerCompleted += (o, ea) => { IsBusy = false; };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void GetInstitutionAction(string insID)
        {
            #region ReSharperDisable

            // ReSharper disable RedundantAssignment
            // ReSharper disable once UnusedVariable

            #endregion ReSharperDisable

            if (CheckFocusDivision(insID))
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusDivision"));
                return;
            }
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            var institutionSelectionWindow = new InstitutionSelectionWindow(insID);
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void GetCommonInstitutionAction()
        {
            #region ReSharperDisable

            // ReSharper disable RedundantAssignment
            // ReSharper disable once UnusedVariable

            #endregion ReSharperDisable

            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            var commonHospitalsWindow = new CommonHospitalsWindow();
        }

        private void GetDiseaseCodeAction(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID) || EditedPrescription.CheckDiseaseEquals(parameters))
            {
                DiseaseFocusNext(elementName);
                return;
            }
            //診斷碼查詢
            switch (elementName)
            {
                case "MainDiagnosis":
                    EditedPrescription.MainDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;

                case "SecondDiagnosis":
                    if (!string.IsNullOrEmpty(diseaseID))
                        EditedPrescription.SubDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
            }
            DataChangedAction();
        }

        private void CheckClearDiseaseAction(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID))
            {
                switch (elementName)
                {
                    case "MainDiagnosis":
                        EditedPrescription.MainDisease = new DiseaseCode();
                        break;

                    case "SecondDiagnosis":
                        EditedPrescription.SubDisease = new DiseaseCode();
                        break;
                }
            }
        }

        private void CopaymentSelectionChangedAction()
        {
            if (EditedPrescription.Copayment is null || EditedPrescription.PrescriptionPoint is null) return;
            switch (EditedPrescription.Copayment.Id)
            {
                case "I21" when EditedPrescription.PrescriptionPoint.MedicinePoint > 100:
                    EditedPrescription.Copayment = VM.GetCopayment("I20");
                    if (EditedPrescription.Institution.ID == "3532082753")
                    {
                        PrintEditedPrescription.Copayment = VM.GetCopayment("I20");
                    }
                    break;

                case "I20" when EditedPrescription.PrescriptionPoint.MedicinePoint <= 100:
                    EditedPrescription.Copayment = VM.GetCopayment("I21");
                    if (EditedPrescription.Institution.ID == "3532082753")
                    {
                        PrintEditedPrescription.Copayment = VM.GetCopayment("I21");
                    }
                    break;
            }
            DataChangedAction();
        }

        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (!CheckMedicineIDLength(medicineID)) return;
            var productCount = GetProductCount(medicineID);
            if (productCount == 0)
                MessageWindow.ShowMessage(Resources.查無藥品, MessageType.WARNING);
            else
            {
                var wareHouse = EditedPrescription.WareHouse;
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedMedicine);
                var addMedicineWindow = wareHouse is null ? new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionEdit, "0") : new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionEdit, wareHouse.ID);
                if (productCount > 1)
                    addMedicineWindow.ShowDialog();
            }
        }

        private void DeleteMedicineAction()
        {
            EditedPrescription.DeleteMedicine();
            EditedPrescription.CountPrescriptionPoint();
            EditedPrescription.CountSelfPay();
            EditedPrescription.PrescriptionPoint.CountAmountsPay();
            DataChangedAction();
        }

        private void ChangeMedicineIDToMostPricedAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.AddMedicine(((MedicineNHI)EditedPrescription.SelectedMedicine).MostPricedID);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            var wareID = EditedPrescription.WareHouse is null ? "0" : EditedPrescription.GetWareHouseID();
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, wareID }, "ShowProductDetail"));
        }

        private void SetBuckleAmount()
        {
            EditedPrescription.IsBuckle = EditedPrescription.WareHouse != null;
            DataChangedAction();
        }

        private void AdjustNoBuckleAction()
        {
            switch (EditedPrescription.SelectedMedicine.AdjustNoBuckle)
            {
                case true:
                    EditedPrescription.SelectedMedicine.AdjustNoBuckle = false;
                    EditedPrescription.SelectedMedicine.BuckleAmount = EditedPrescription.SelectedMedicine.Amount;
                    break;

                case false:
                    EditedPrescription.SelectedMedicine.AdjustNoBuckle = true;
                    EditedPrescription.SelectedMedicine.BuckleAmount = 0;
                    break;
            }
            DataChangedAction();
        }

        private void ResetBuckleAmountAction()
        {
            EditedPrescription.SelectedMedicine?.ResetBuckleAmount();
            IsEdit = true;
        }

        private void CustomerDetailEditedAction()
        {
            CustomerEdited = true;
        }

        private void CustomerRedoEditedAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.Patient = Customer.GetCustomerByCusId(EditedPrescription.Patient.ID);
            MainWindow.ServerConnection.CloseConnection();
            CustomerEdited = false;
        }

        private void SavePatientDataAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.Patient.Save();
            MainWindow.ServerConnection.CloseConnection();
            CustomerEdited = false;
        }

        private void CountMedicinePoint()
        {
            EditedPrescription.CheckPrescriptionVariable();
            EditedPrescription.CountPrescriptionPoint();
            EditedPrescription.CountSelfPay();
            EditedPrescription.PrescriptionPoint.CountAmountsPay();
            RaisePropertyChanged("TotalMedPoint");
            DataChangedAction();
        }

        private void DeleteAction()
        {
            ConfirmWindow deleteConfirm = new ConfirmWindow("確定刪除此處方?", "刪除確認");
            var delete = deleteConfirm.DialogResult;
            if ((bool)delete)
            {
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.Delete();
                MainWindow.ServerConnection.CloseConnection();
                Messenger.Default.Send(EditedPrescription.Type.Equals(PrescriptionType.ChronicReserve)
                    ? new NotificationMessage("ReservePrescriptionEdited")
                    : new NotificationMessage("PrescriptionEdited"));
                Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
            }
        }

        private void RedoEditAction()
        {
            EditedPrescription = (Prescription)OriginalPrescription.Clone();
            if (EditedPrescription.Institution.ID == "3532082753")
            {
                PrintEditedPrescription = (Prescription)OriginalPrescription.PrintClone();
            }
            EditedPrescription.ID = OriginalPrescription.ID;
            EditedPrescription.SourceId = OriginalPrescription.SourceId;
            PrintEditedPrescription.ID = OriginalPrescription.ID;
            PrintEditedPrescription.SourceId = OriginalPrescription.SourceId;
            InitPrescription();
            IsEdit = false;
        }

        private void EditCompleteAction()
        {
            if (!CheckSameOrIDEmptyMedicine()) return;
            if (!CheckMedicinesNegativeStock()) return;
            currentService = PrescriptionService.CreateService(EditedPrescription);
            if (!currentService.CheckEditPrescription(EditedPrescription.PrescriptionStatus.IsGetCard)) return;
            EditedPrescription.SetDetail();
            MainWindow.SingdeConnection.OpenConnection();
            var result = EditedPrescription.Update();
            if (result && EditedPrescription.Type.Equals(PrescriptionType.ChronicRegister) && !EditedPrescription.PrescriptionStatus.OrderStatus.Equals("備藥狀態:已收貨"))
            {
                MedicinesSendSingdeViewModel vm = null;
                var medicinesSendSingdeWindow = new MedicinesSendSingdeWindow(EditedPrescription);
                vm = (MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext;
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                    return;
                currentService.SendOrder(vm);
            }
            else if (result && EditedPrescription.Type.Equals(PrescriptionType.Normal))
            {
                var medicalServiceDiff = EditedPrescription.PrescriptionPoint.MedicalServicePoint -
                                OriginalPrescription.PrescriptionPoint.MedicalServicePoint;
                var medicineDiff = EditedPrescription.PrescriptionPoint.MedicinePoint -
                                  OriginalPrescription.PrescriptionPoint.MedicinePoint;
                var originPaySelf = OriginalPrescription.PrescriptionPoint.AmountSelfPay ?? 0;
                var editedPaySelf = EditedPrescription.PrescriptionPoint.AmountSelfPay ?? 0;
                var paySelfDiff = editedPaySelf - originPaySelf;
                PrescriptionDb.InsertPrescriptionPointEditRecord(EditedPrescription.ID, medicalServiceDiff, medicineDiff, paySelfDiff);
            }
            MainWindow.SingdeConnection.CloseConnection();
            if (result)
                MessageWindow.ShowMessage("編輯成功", MessageType.SUCCESS);
            Messenger.Default.Send(new NotificationMessage("PrescriptionEdited"));
            Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
        }

        private void AdjustDateLostFocusAction()
        {
            if (EditedPrescription.AdjustDate != null)
                EditedPrescription.UpdateMedicines();
        }

        private void DiseaseFocusNext(string elementName)
        {
            if (elementName == "MainDiagnosis")
            {
                Messenger.Default.Send(new NotificationMessage(this, "FocusSubDisease"));
                return;
            }
            Messenger.Default.Send(new NotificationMessage(this, "FocusChronicTotal"));
        }

        private bool ConfirmPrintReceipt()
        {
            var receiptResult = new ConfirmWindow(Resources.收據列印確認, Resources.列印確認, true);
            var printReceipt = receiptResult.DialogResult;
            return printReceipt != null && (bool)printReceipt;
        }

        private bool CheckFocusDivision(string insID)
        {
            return EditedPrescription.Institution != null &&
                   !string.IsNullOrEmpty(EditedPrescription.Institution.FullName) &&
                   insID.Equals(EditedPrescription.Institution.FullName);
        }

        [SuppressMessage("ReSharper", "TooManyChainedReferences")]
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            if (!CheckWareHouseNotChanged(receiveSelectedInstitution)) return;
            EditedPrescription.Institution = new Institution();
            EditedPrescription.Institution = receiveSelectedInstitution;
            var notification = string.IsNullOrEmpty(EditedPrescription.Division?.ID) ? "FocusDivision" : "FocusMedicalNumber";
            Messenger.Default.Send(new NotificationMessage(this, notification));
        }

        private bool CheckWareHouseNotChanged(Institution receiveSelectedInstitution)
        {
            var wareHouse = VM.CooperativeClinicSettings.GetWareHouseByPrescription(receiveSelectedInstitution,
                EditedPrescription.AdjustCase.ID);
            var tempIns = EditedPrescription.Institution.ID;
            if (wareHouse is null && EditedPrescription.WareHouse != null)
            {
                MessageWindow.ShowMessage("編輯失敗，選擇院所會影響庫存", MessageType.WARNING);
                GetInstitutionAction(tempIns);
                return false;
            }
            if (wareHouse != null && EditedPrescription.WareHouse is null)
            {
                MessageWindow.ShowMessage("編輯失敗，選擇院所會影響庫存", MessageType.WARNING);
                GetInstitutionAction(tempIns);
                return false;
            }
            if (!wareHouse.ID.Equals(EditedPrescription.WareHouse.ID))
            {
                MessageWindow.ShowMessage("編輯失敗，選擇院所對應不同庫", MessageType.WARNING);
                GetInstitutionAction(tempIns);
                return false;
            }
            return true;
        }

        private int GetProductCount(string medicineID)
        {
            var wareHouse = EditedPrescription.WareHouse;
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionEdit, wareHouse is null ? "0" : wareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            return productCount;
        }

        private bool CheckMedicineIDLength(string medicineID)
        {
            if (medicineID.Length >= 5) return true;
            switch (medicineID)
            {
                case "R001":
                case "R002":
                case "R003":
                case "R004":
                case "R005":
                    EditedPrescription.Medicines.Add(new MedicineVirtual(medicineID));
                    break;

                default:
                    MessageWindow.ShowMessage(Resources.搜尋字串長度不足 + "5", MessageType.WARNING);
                    break;
            }
            return false;
        }

        private void GetSelectedMedicine(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionEditViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.AddMedicine(msg.Content.ID);
            MainWindow.ServerConnection.CloseConnection();
            DataChangedAction();
        }

        private bool CheckSameOrIDEmptyMedicine()
        {
            var medicinesSame = EditedPrescription.CheckMedicinesIdEmpty();
            if (string.IsNullOrEmpty(medicinesSame)) return true;
            MessageWindow.ShowMessage(medicinesSame, MessageType.WARNING);
            return false;
        }

        private bool CheckMedicinesNegativeStock()
        {
            if (EditedPrescription.InsertTime is null) return true;
            if (EditedPrescription.WareHouse is null) return true;
            var negativeStock = GetNegativeStockMessage(GetMedicinesInventories());
            if (string.IsNullOrEmpty(negativeStock)) return true;
            negativeStock += "扣庫量變化造成負庫，請修改扣庫量。";
            MessageWindow.ShowMessage(negativeStock, MessageType.WARNING);
            return false;
        }

        private Inventorys GetMedicinesInventories()
        {
            MainWindow.ServerConnection.OpenConnection();
            var inventories = Inventorys.GetAllInventoryByProIDs(GetMedicinesIDsConcatenated(), EditedPrescription.WareHouse?.ID);
            MainWindow.ServerConnection.CloseConnection();
            foreach (var med in OriginalPrescription.Medicines)
            {
                if (med is MedicineVirtual) continue;
                inventories.Single(i => i.InvID.Equals(med.InventoryID)).OnTheFrame += med.BuckleAmount;
            }
            return inventories;
        }

        private string GetNegativeStockMessage(Inventorys inventoryList)
        {
            var negativeStock = string.Empty;
            foreach (var inv in inventoryList)
            {
                var editMedicines = EditedPrescription.Medicines.Where(m => m.InventoryID.Equals(inv.InvID));
                if (inv.OnTheFrame - editMedicines.Sum(m => m.BuckleAmount) >= 0) continue;
                negativeStock = EditedPrescription.Medicines.Where(med => !(med is MedicineVirtual))
                    .Where(med => med.InventoryID.Equals(inv.InvID))
                    .Aggregate(negativeStock, (current, med) => current + ("藥品" + med.ID + "\n"));
            }
            return negativeStock;
        }

        private List<string> GetMedicinesIDsConcatenated()
        {
            var originMedIDs = OriginalPrescription.Medicines.Where(m => !(m is MedicineVirtual)).Select(m => m.ID).ToList();
            var editMedIDs = EditedPrescription.Medicines.Where(m => !(m is MedicineVirtual)).Select(m => m.ID).ToList();
            return originMedIDs.Concat(editMedIDs).Distinct().ToList();
        }

        private void CheckCustomerEdited()
        {
            if (CustomerEdited)
            {
                var savePatientData = new ConfirmWindow("顧客資料已被編輯，是否儲存變更?", "顧客編輯確認");
                if ((bool)savePatientData.DialogResult)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    EditedPrescription.Patient.Save();
                    MainWindow.ServerConnection.CloseConnection();
                    CustomerEdited = false;
                }
                else
                {
                    CustomerRedoEditedAction();
                    CustomerEdited = false;
                }
            }
        }

        #region 補卡

        private void CheckIsReadCard()
        {
            currentCard = new IcCard();
            worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                ReadCard();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                StartMakeUp();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ReadCard()
        {
            BusyContent = Resources.讀取健保卡;
            try
            {
                currentCard.Read();
            }
            catch (Exception e)
            {
                NewFunction.ExceptionLog(e.Message);
                NewFunction.ShowMessageFromDispatcher("讀卡作業異常，請重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING);
            }
        }

        private bool CheckReadCardResult(Prescription pre)
        {
            if (currentCard.IsRead)
            {
                if (EditedPrescription.Patient.IsAnonymous())
                {
                    if (!GetPatientFromIcCard())
                        return false;
                }
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
            currentCard.GetRegisterBasic();
            if (currentCard.CheckNeedUpdate())
            {
                BusyContent = Resources.更新卡片;
                currentCard.UpdateCard();
            }
            BusyContent = Resources.取得就醫序號;
            currentCard.GetMedicalNumber(2);
            if (currentCard.IsGetMedicalNumber)
                pre.TempMedicalNumber = currentCard.GetLastMedicalNumber();
        }

        private void StartMakeUp()
        {
            if (CheckReadCardResult(EditedPrescription))
            {
                EditedPrescription.CountPrescriptionPoint();
                EditedPrescription.CountSelfPay();
                EditedPrescription.PrescriptionPoint.CountAmountsPay();
                EditedPrescription.SetDetail();
                currentService = PrescriptionService.CreateService(EditedPrescription);
                WriteCard();
                currentService.MakeUpComplete();
                RaisePropertyChanged("CanMakeUp");
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
            currentService.SetCard(currentCard);
            currentService.CreateDailyUploadData(errorCode);
            if (errorCode != null)
                currentService.SetMedicalNumberByErrorCode(errorCode);
            else
            {
                currentService.SetMedicalNumber();
            }
            currentService.CheckDailyUploadMakeUp(errorCode);
        }

        private bool AskErrorUpload()
        {
            var e = new ErrorUploadWindow(currentCard.IsGetMedicalNumber);
            e.ShowDialog();
            if (((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode is null)
            {
                MessageWindow.ShowMessage(Resources.尚未選擇異常代碼, MessageType.WARNING);
            }
            errorCode = ((ErrorUploadWindowViewModel)e.DataContext).SelectedIcErrorCode;
            return errorCode != null;
        }

        private bool GetPatientFromIcCard()
        {
            var patientFromCard = new Customer(currentCard);
            return CheckCustomerByCard(patientFromCard);
        }

        private bool CheckCustomerByCard(Customer patientFromCard)
        {
            Customer checkedPatient;
            MainWindow.ServerConnection.OpenConnection();
            var table = CustomerDb.CheckCustomerByCard(currentCard.IDNumber);
            if (table.Rows.Count > 0)
            {
                var patientFromDB = new Customer(table.Rows[0]);
                patientFromDB.CheckPatientWithCard(patientFromCard);
                checkedPatient = patientFromDB;
                checkedPatient.Save();
            }
            else
            {
                var insertResult = patientFromCard.InsertData();
                if (!insertResult)
                {
                    MessageWindow.ShowMessage("顧客新增失敗。", MessageType.WARNING);
                    return false;
                }
                checkedPatient = patientFromCard;
            }
            EditedPrescription.Patient = checkedPatient;
            return true;
        }

        #endregion 補卡
    }
}