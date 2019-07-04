using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.NewClass.MedicineRefactoring;
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
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using StringRes = His_Pos.Properties.Resources;
using MedSelectWindow = His_Pos.FunctionWindow.AddProductWindow.AddMedicineWindow;
using HisAPI = His_Pos.HisApi.HisApiFunction;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Employee;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using MedicineNHI = His_Pos.NewClass.Product.Medicine.MedicineNHI;
using MedicineOTC = His_Pos.NewClass.Product.Medicine.MedicineOTC;
using MedicineSpecialMaterial = His_Pos.NewClass.Product.Medicine.MedicineSpecialMaterial;
using MedicineVirtual = His_Pos.NewClass.Product.Medicine.MedicineVirtual;

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
        private bool canMakeup;
        public bool CanMakeup
        {
            get => canMakeup;
            set
            {
                Set(() => CanMakeup, ref canMakeup, value);
            }
        }
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
        public double TotalMedPoint
        {
            get
            {
                if (EditedPrescription != null)
                {
                    return EditedPrescription.PrescriptionPoint.MedicinePoint +
                           EditedPrescription.PrescriptionPoint.SpecialMaterialPoint;
                }
                return 0;
            }
        }

        private List<BuckleMedicineStruct> editMedicines { get; set; }
        public bool ShowDialog { get; set; }
        #region Commands
        public RelayCommand PrintMedBagCmd { get; set; }
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> CheckClearDisease { get; set; }
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
        public RelayCommand AdjustDateLostFocus { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        public RelayCommand ComboboxSelectionChanged { get; set; }
        #endregion
        #region ItemsSources
        public Institutions Institutions { get; set; }
        public Divisions Divisions { get; set; }
        public Employees MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public Copayments Copayments { get; set; }
        public SpecialTreats SpecialTreats { get; set; }
        #endregion
        public PrescriptionEditViewModel(int preID,PrescriptionSource pSource)
        {
            MainWindow.ServerConnection.OpenConnection();
            Prescription selected;
            DataRow r;
            if (pSource.Equals(PrescriptionSource.Normal))
            {
                r = PrescriptionDb.GetPrescriptionByID(preID).Rows[0];
                if (r.Field<string>("IsEnable").Equals("0"))
                {
                    MessageWindow.ShowMessage("處方已被刪除。", MessageType.ERROR);
                    Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
                    ShowDialog = false;
                    return;
                }
                selected = new Prescription(r, PrescriptionSource.Normal);
                ShowDialog = true;
            }
            else
            {
                r = PrescriptionDb.GetReservePrescriptionByID(preID).Rows[0];
                selected = new Prescription(r, PrescriptionSource.ChronicReserve);
                ShowDialog = true;
            }
            MainWindow.ServerConnection.CloseConnection();
            CanMakeup = !selected.Treatment.AdjustCase.ID.Equals("0") && selected.PrescriptionStatus.IsAdjust;
            NotPrescribe = !selected.Treatment.AdjustCase.ID.Equals("0");
            OriginalPrescription = selected;
            OriginalPrescription.PrescriptionPoint.GetAmountPaySelf(OriginalPrescription.Id);
            Init((Prescription)OriginalPrescription.Clone());
            IsGetCard = !CanMakeup || EditedPrescription.PrescriptionStatus.IsGetCard;
        }
        public PrescriptionEditViewModel(Prescription p)
        {
            MainWindow.ServerConnection.OpenConnection();
            var selected = p;
            MainWindow.ServerConnection.CloseConnection();
            CanMakeup = !selected.Treatment.AdjustCase.ID.Equals("0") && selected.PrescriptionStatus.IsAdjust;
            NotPrescribe = !selected.Treatment.AdjustCase.ID.Equals("0");
            OriginalPrescription = selected;
            if(!p.Source.Equals(PrescriptionSource.ChronicReserve))
            OriginalPrescription.PrescriptionPoint.GetAmountPaySelf(OriginalPrescription.Id);
            Init((Prescription)OriginalPrescription.Clone());
            IsGetCard = !CanMakeup || EditedPrescription.PrescriptionStatus.IsGetCard;
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
            selected.Patient = Customer.GetCustomerByCusId(selected.Patient.ID);
            EditedPrescription = selected;
            if (selected.Source.Equals(PrescriptionSource.Normal))
            {
                var id = selected.Id;
                var warID = selected.WareHouse?.ID;
                var adjustDate = selected.Treatment.AdjustDate;
                OriginalPrescription.Medicines.Clear();
                OriginalPrescription.Medicines.GetDataByPrescriptionId(id, warID, adjustDate);
                EditedPrescription.Medicines.Clear();;
                EditedPrescription.Medicines.GetDataByPrescriptionId(id, warID, adjustDate);
            }
            else
            {
                EditedPrescription.Medicines.GetDataByReserveId(int.Parse(EditedPrescription.SourceId), EditedPrescription.WareHouse?.ID, EditedPrescription.Treatment.AdjustDate);
            }
            MainWindow.ServerConnection.CloseConnection();
            if (EditedPrescription.Treatment.Division != null)
                EditedPrescription.Treatment.Division = VM.GetDivision(EditedPrescription.Treatment.Division?.ID);
            EditedPrescription.Treatment.Pharmacist =
                VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IDNumber.Equals(EditedPrescription.Treatment.Pharmacist.IDNumber));
            EditedPrescription.Treatment.AdjustCase = VM.GetAdjustCase(EditedPrescription.Treatment.AdjustCase.ID);
            EditedPrescription.Treatment.Copayment = VM.GetCopayment(EditedPrescription.Treatment.Copayment?.Id);
            if (EditedPrescription.Treatment.PrescriptionCase != null)
                EditedPrescription.Treatment.PrescriptionCase = VM.GetPrescriptionCases(EditedPrescription.Treatment.PrescriptionCase?.ID);
            if (EditedPrescription.Treatment.SpecialTreat != null)
                EditedPrescription.Treatment.SpecialTreat = VM.GetSpecialTreat(EditedPrescription.Treatment.SpecialTreat?.ID);
            EditedPrescription.PrescriptionPoint.GetDeposit(EditedPrescription.Id);
            EditedPrescription.CheckIsBuckleAndSource();
        }

        private void InitialItemsSources()
        {
            Institutions = VM.Institutions;
            Divisions = VM.Divisions;
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(EditedPrescription.Treatment.AdjustDate ?? DateTime.Today);
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
            GetDiseaseCode = new RelayCommand<object>(GetDiseaseCodeAction);
            CheckClearDisease = new RelayCommand<object>(CheckClearDiseaseAction);
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
            AdjustDateLostFocus = new RelayCommand(AdjustDateLostFocusAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            ComboboxSelectionChanged = new RelayCommand(ComboboxSelectionChangedAction);
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
            Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            var commonInsSelectionWindow = new CommonHospitalsWindow();
            commonInsSelectionWindow.ShowDialog();
        }
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "4", MessageType.WARNING);
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
                    EditedPrescription.CheckIsCooperative();
                    break;
                default:
                    Messenger.Default.Register<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }

        private void GetDiseaseCodeAction(object sender)
        {
            var parameters = sender.ConvertTo<List<string>>();
            var elementName = parameters[0];
            var diseaseID = parameters[1];
            if (string.IsNullOrEmpty(diseaseID) || EditedPrescription.Treatment.CheckDiseaseEquals(parameters))
            {
                DiseaseFocusNext(elementName);
                return;
            }
            //診斷碼查詢
            switch (elementName)
            {
                case "MainDiagnosis":
                    EditedPrescription.Treatment.MainDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
                case "SecondDiagnosis":
                    if (!string.IsNullOrEmpty(diseaseID))
                        EditedPrescription.Treatment.SubDisease = DiseaseCode.GetDiseaseCodeByID(diseaseID);
                    break;
            }
            CheckEditStatus();
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
                        EditedPrescription.Treatment.MainDisease = new DiseaseCode();
                        break;
                    case "SecondDiagnosis":
                        EditedPrescription.Treatment.SubDisease = new DiseaseCode();
                        break;
                }
            }
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
                MedicineVirtual m = new MedicineVirtual();
                switch (medicineID)
                {
                    case "R001":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "處方箋遺失或毀損，提前回診";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        EditedPrescription.Medicines.Add(m);
                        return;
                    case "R002":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "醫師請假，提前回診";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        EditedPrescription.Medicines.Add(m);
                        return;
                    case "R003":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        EditedPrescription.Medicines.Add(m);
                        return;
                    case "R004":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "其他提前回診或慢箋提前領藥";
                        m.Amount = 0;
                        m.TotalPrice = 0;
                        EditedPrescription.Medicines.Add(m);
                        return;
                    default:
                        MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "5", MessageType.WARNING);
                        return;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            var wareHouse = VM.CooperativeClinicSettings.GetWareHouseByPrescription(EditedPrescription.Treatment.Institution, EditedPrescription.Treatment.AdjustCase.ID);
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse is null ? "0" : wareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = wareHouse is null ? new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit,"0") : new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit, wareHouse.ID);
                MedicineWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                MedicineWindow = wareHouse is null ? new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit, "0") : new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit, wareHouse.ID);
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
            CheckEditStatus();
        }
        private void EditCompleteAction()
        {
            if (IsEdit)
            {
                if (!CheckSameOrIDEmptyMedicine())return;
                if(!CheckMedicinesNegativeStock())return;
                if (!EditedPrescription.Treatment.AdjustCase.ID.Equals("0"))
                {
                    var noCard = !EditedPrescription.PrescriptionStatus.IsGetCard;
                    var error = EditedPrescription.CheckPrescriptionRule(noCard);
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageWindow.ShowMessage(error, MessageType.ERROR);
                        return;
                    }
                }
                if (!EditedPrescription.CheckMedicalNumber())
                    return;
                EditedPrescription.CountPrescriptionPoint(false);
                if(!EditedPrescription.IsBuckle)
                    EditedPrescription.Medicines.SetNoBuckle();
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.Update();
                MainWindow.ServerConnection.CloseConnection();
                MessageWindow.ShowMessage("編輯成功",MessageType.SUCCESS);
                Messenger.Default.Send(EditedPrescription.Source.Equals(PrescriptionSource.ChronicReserve)
                    ? new NotificationMessage("ReservePrescriptionEdited")
                    : new NotificationMessage("PrescriptionEdited"));
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
                Messenger.Default.Send(EditedPrescription.Source.Equals(PrescriptionSource.ChronicReserve)
                    ? new NotificationMessage("ReservePrescriptionEdited")
                    : new NotificationMessage("PrescriptionEdited"));
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
            var receiptResult = new ConfirmWindow(StringRes.收據列印確認, StringRes.列印確認, true);
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
            Messenger.Default.Unregister<Institution>(this, "GetSelectedInstitution", GetSelectedInstitution);
            EditedPrescription.Treatment.Institution = receiveSelectedInstitution;
            EditedPrescription.CheckIsCooperative();
            CheckEditStatus();
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(PrescriptionEditViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.AddMedicineBySearch(msg.Content.ID,EditedPrescription.Treatment.AdjustDate);
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
            EditedPrescription.CheckIsBuckle();
            if (EditedPrescription.IsBuckle)
                EditedPrescription.SelectedMedicine.BuckleAmount = EditedPrescription.SelectedMedicine.Amount;
            else
                EditedPrescription.SelectedMedicine.BuckleAmount = 0;
            CheckEditStatus();
        }
        private void AdjustDateLostFocusAction()
        {
            MedicalPersonnels = VM.CurrentPharmacy.GetPharmacists(EditedPrescription.Treatment.AdjustDate ?? DateTime.Today);
            if (EditedPrescription.Treatment.AdjustDate is null) return;
            EditedPrescription.Medicines.UpdateInventory(EditedPrescription.WareHouse?.ID, EditedPrescription.Treatment.AdjustDate);
        }

        private void ShowMedicineDetailAction(string medicineID)
        {
            var wareID = EditedPrescription.WareHouse is null ? "0" : EditedPrescription.WareHouse.ID;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { medicineID, wareID }, "ShowProductDetail"));
        }

        private void ComboboxSelectionChangedAction()
        {
            CheckEditStatus();
        }

        private bool CheckSameOrIDEmptyMedicine()
        {
            var medicinesSame = EditedPrescription.CheckMedicinesIdEmpty();
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
                    EditedPrescription.Treatment.GetLastMedicalNumber();
                    if (!string.IsNullOrEmpty(EditedPrescription.Treatment.TempMedicalNumber))
                    {
                        if (EditedPrescription.Treatment.ChronicSeq is null)
                            EditedPrescription.Treatment.MedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                        else
                        {
                            if (EditedPrescription.Treatment.ChronicSeq > 1)
                            {
                                EditedPrescription.Treatment.MedicalNumber = "IC0" + EditedPrescription.Treatment.ChronicSeq;
                                EditedPrescription.Treatment.OriginalMedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                            }
                            else
                            {
                                EditedPrescription.Treatment.MedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                                EditedPrescription.Treatment.OriginalMedicalNumber = null;
                            }
                        }
                    }
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
                    EditedPrescription.Treatment.TempMedicalNumber = errorCode.ID;
                    if (EditedPrescription.Treatment.ChronicSeq is null)
                        EditedPrescription.Treatment.MedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                    else
                    {
                        if (EditedPrescription.Treatment.ChronicSeq > 1)
                        {   
                            EditedPrescription.Treatment.MedicalNumber = "IC0" + EditedPrescription.Treatment.ChronicSeq;
                            EditedPrescription.Treatment.OriginalMedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                        }
                        else
                        {
                            EditedPrescription.Treatment.MedicalNumber = EditedPrescription.Treatment.TempMedicalNumber;
                            EditedPrescription.Treatment.OriginalMedicalNumber = null;
                        }
                    }
                    HisAPI.CreatErrorDailyUploadData(EditedPrescription, false, errorCode);
                }
                MainWindow.ServerConnection.OpenConnection();
                EditedPrescription.PrescriptionPoint.GetDeposit(EditedPrescription.Id);
                var deposit = EditedPrescription.PrescriptionPoint.Deposit;
                // string depositName = EditedPrescription.Treatment.Institution.ID == ViewModelMainWindow.CooperativeInstitutionID ? "合作退還押金" : "退還押金";
                // PrescriptionDb.ProcessCashFlow(depositName, "PreMasId", EditedPrescription.Id, EditedPrescription.PrescriptionPoint.Deposit * -1);
                EditedPrescription.PrescriptionPoint.Deposit = 0;
                EditedPrescription.PrescriptionStatus.UpdateStatus(EditedPrescription.Id);
                EditedPrescription.Update();
                MainWindow.ServerConnection.CloseConnection();
                CheckEditStatus();
                IsBusy = false;
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage("補卡作業成功，退還押金" + deposit + "元", MessageType.SUCCESS);
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
            return CanMakeup;
        }
        private bool CheckMedicinesNegativeStock()
        {
            if (EditedPrescription.InsertTime is null ||
                EditedPrescription.Source.Equals(PrescriptionSource.ChronicReserve)) return true;
            editMedicines = new List<BuckleMedicineStruct>();
            var inventoryIDList = new List<int>(); 
            foreach (var originMed in OriginalPrescription.Medicines)
            {
                if(originMed is MedicineVirtual) continue;
                if(!inventoryIDList.Contains(originMed.InventoryID) && originMed.InventoryID != 0)
                    inventoryIDList.Add(originMed.InventoryID);
            }
            foreach (var inv in inventoryIDList)
            {
                var editMed = EditedPrescription.Medicines.Where(m => !(m is MedicineVirtual) && m.InventoryID.Equals(inv));
                var originMed = OriginalPrescription.Medicines.Where(m => !(m is MedicineVirtual) && m.InventoryID.Equals(inv));
                var buckleDiff = editMed.Sum(m => m.BuckleAmount) - originMed.Sum(m => m.BuckleAmount);
                if(buckleDiff > 0)
                    editMedicines.Add(new BuckleMedicineStruct(inv,buckleDiff));
            }
            var editInvIDList = new List<int>();
            foreach (var edit in editMedicines)
            {
                editInvIDList.Add(edit.ID);
            }
            MainWindow.ServerConnection.OpenConnection();
            var invTable = MedicineDb.GetInventoryByInvIDs(editInvIDList);
            MainWindow.ServerConnection.CloseConnection();
            var inventoryList = new List<MedicineInventoryStruct>();
            foreach (DataRow r in invTable.Rows)
            {
                inventoryList.Add(new MedicineInventoryStruct(r.Field<int>("Inv_ID"),r.Field<double>("Inv_Inventory")));
            }
            var negativeStock = string.Empty;
            foreach (var inv in inventoryList)
            {
                if (inv.Inventory - editMedicines.Single(m => m.ID.Equals(inv.ID)).BuckleAmount >= 0) continue;
                foreach (var med in EditedPrescription.Medicines)
                {
                    if (med is MedicineVirtual) continue;
                    if (med.InventoryID.Equals(inv.ID))
                        negativeStock += "藥品" + med.ID + "\n";
                }
            }
            if (string.IsNullOrEmpty(negativeStock)) return true;
            negativeStock += "扣庫量變化造成負庫，請修改扣庫量。";
            MessageWindow.ShowMessage(negativeStock,MessageType.WARNING);
            return false;
        }
        #endregion
    }
}
