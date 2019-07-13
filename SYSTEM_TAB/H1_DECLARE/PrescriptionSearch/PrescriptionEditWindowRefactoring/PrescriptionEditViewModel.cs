﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
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
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.DiseaseCode;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.PrescriptionRefactoring;
using His_Pos.NewClass.PrescriptionRefactoring.Service;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using IcCard = His_Pos.NewClass.Prescription.IcCard;
using MedicineVirtual = His_Pos.NewClass.MedicineRefactoring.MedicineVirtual;
using PrescriptionSource = His_Pos.NewClass.Prescription.PrescriptionSource;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ValueParameterNotUsed

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindowRefactoring
{
    public class PrescriptionEditViewModel:ViewModelBase
    {
        #region UIProperties

        public double WindowWidth
        {
            get => SystemParameters.WorkArea.Width * 0.85;
            set { }
        }
        public double WindowHeight
        {
            get => SystemParameters.WorkArea.Height * 0.85;
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
        private bool isPrescribe;
        public bool IsPrescribe
        {
            get => isPrescribe;
            set
            {
                Set(() => IsPrescribe, ref isPrescribe, value);
            }
        }
        public bool CanMakeUp
        {
            get => !EditedPrescription.PrescriptionStatus.IsGetCard && EditedPrescription.InsertTime != null;
        }
        private BackgroundWorker worker;
        #endregion
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
        private List<BuckleMedicineStruct> EditMedicines { get; set; }
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
        #region Commands
        public RelayCommand PrintMedBag { get; set; }
        public RelayCommand<string> GetInstitution { get; set; }
        public RelayCommand<object> GetDiseaseCode { get; set; }
        public RelayCommand<object> CheckClearDisease { get; set; }
        public RelayCommand GetCommonInstitution { get; set; }
        public RelayCommand<string> AddMedicine { get; set; }
        public RelayCommand MedicinePriceChanged { get; set; }
        public RelayCommand RedoEdit { get; set; }
        public RelayCommand EditComplete { get; set; }
        public RelayCommand DataChanged { get; set; }
        public RelayCommand MakeUp { get; set; }
        public RelayCommand PrintDepositSheet { get; set; }
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand PrintReceipt { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand MedicineAmountChanged { get; set; }
        public RelayCommand AdjustDateLostFocus { get; set; }
        public RelayCommand<string> ShowMedicineDetail { get; set; }
        #endregion
        public PrescriptionEditViewModel()
        {
        }

        public PrescriptionEditViewModel(Prescription p,string title)
        {
            Title = title;
            IsEdit = false;
            OriginalPrescription = p;
            EditedPrescription = (Prescription)OriginalPrescription.Clone();
            EditedPrescription.ID = p.ID;
            EditedPrescription.SourceId = p.SourceId;
            InitialItemsSources();
            InitialCommandActions();
            InitPrescription();
        }

        private void InitPrescription()
        {
            if (EditedPrescription.Division != null)
                EditedPrescription.Division = VM.GetDivision(EditedPrescription.Division?.ID);
            EditedPrescription.Pharmacist =
                VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IDNumber.Equals(EditedPrescription.Pharmacist.IDNumber));
            EditedPrescription.AdjustCase = VM.GetAdjustCase(EditedPrescription.AdjustCase.ID);
            EditedPrescription.Copayment = VM.GetCopayment(EditedPrescription.Copayment?.Id);
            if (EditedPrescription.PrescriptionCase != null)
                EditedPrescription.PrescriptionCase = VM.GetPrescriptionCases(EditedPrescription.PrescriptionCase?.ID);
            if (EditedPrescription.SpecialTreat != null)
                EditedPrescription.SpecialTreat = VM.GetSpecialTreat(EditedPrescription.SpecialTreat?.ID);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.GetMedicines();
            EditedPrescription.UpdateMedicines();
            MainWindow.ServerConnection.CloseConnection();
            IsPrescribe = EditedPrescription.IsPrescribe;
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
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            ShowMedicineDetail = new RelayCommand<string>(ShowMedicineDetailAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            MedicineAmountChanged = new RelayCommand(SetBuckleAmount);
            Delete = new RelayCommand(DeleteAction);
            RedoEdit = new RelayCommand(RedoEditAction);
            EditComplete = new RelayCommand(EditCompleteAction);
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
            if(!ConfirmPrintReceipt()) return;
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
            var printConfirmResult = NewFunction.CheckPrint(EditedPrescription);
            var printMedBag = printConfirmResult[0];
            var printSingle = printConfirmResult[1];
            var printReceipt = printConfirmResult[2];
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
                            EditedPrescription.PrintMedBagMultiMode();
                            break;
                        case true:
                            EditedPrescription.PrintMedBagSingleMode();
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
            #endregion
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
            #endregion
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
                var addMedicineWindow = wareHouse is null ? new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionDeclare, "0") : new AddMedicineWindow(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse.ID);
                if (productCount > 1)
                    addMedicineWindow.ShowDialog();
            }
        }

        private void DeleteMedicineAction()
        {
            EditedPrescription.DeleteMedicine();
            DataChangedAction();
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
            EditedPrescription.ID = OriginalPrescription.ID;
            EditedPrescription.SourceId = OriginalPrescription.SourceId;
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
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription.Update();
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("編輯成功", MessageType.SUCCESS);
            Messenger.Default.Send(new NotificationMessage("PrescriptionEdited"));
            Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
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
            if(!CheckWareHouseNotChanged(receiveSelectedInstitution))return;
            EditedPrescription.Institution = new Institution();
            EditedPrescription.Institution = receiveSelectedInstitution;
            EditedPrescription.UpdateMedicines();
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
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, wareHouse is null ? "0" : wareHouse.ID);
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
            if (EditedPrescription.InsertTime is null ||
                EditedPrescription.Type.Equals(PrescriptionSource.ChronicReserve)) return true;
            EditMedicines = new List<BuckleMedicineStruct>();
            var inventoryIDList = new List<int>();
            foreach (var originMed in OriginalPrescription.Medicines)
            {
                if (originMed is MedicineVirtual) continue;
                if (!inventoryIDList.Contains(originMed.InventoryID) && originMed.InventoryID != 0)
                    inventoryIDList.Add(originMed.InventoryID);
            }
            foreach (var inv in inventoryIDList)
            {
                var editMed = EditedPrescription.Medicines.Where(m => !(m is MedicineVirtual) && m.InventoryID.Equals(inv));
                var originMed = OriginalPrescription.Medicines.Where(m => !(m is MedicineVirtual) && m.InventoryID.Equals(inv));
                var buckleDiff = editMed.Sum(m => m.BuckleAmount) - originMed.Sum(m => m.BuckleAmount);
                if (buckleDiff > 0)
                    EditMedicines.Add(new BuckleMedicineStruct(inv, buckleDiff));
            }
            var editInvIDList = new List<int>();
            foreach (var edit in EditMedicines)
            {
                editInvIDList.Add(edit.ID);
            }
            MainWindow.ServerConnection.OpenConnection();
            var invTable = MedicineDb.GetInventoryByInvIDs(editInvIDList);
            MainWindow.ServerConnection.CloseConnection();
            var inventoryList = new List<MedicineInventoryStruct>();
            foreach (DataRow r in invTable.Rows)
            {
                inventoryList.Add(new MedicineInventoryStruct(r.Field<int>("Inv_ID"), r.Field<double>("Inv_Inventory")));
            }
            var negativeStock = string.Empty;
            foreach (var inv in inventoryList)
            {
                if (inv.Inventory - EditMedicines.Single(m => m.ID.Equals(inv.ID)).BuckleAmount >= 0) continue;
                foreach (var med in EditedPrescription.Medicines)
                {
                    if (med is MedicineVirtual) continue;
                    if (med.InventoryID.Equals(inv.ID))
                        negativeStock += "藥品" + med.ID + "\n";
                }
            }
            if (string.IsNullOrEmpty(negativeStock)) return true;
            negativeStock += "扣庫量變化造成負庫，請修改扣庫量。";
            MessageWindow.ShowMessage(negativeStock, MessageType.WARNING);
            return false;
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
                Application.Current.Dispatcher.Invoke(() => MessageWindow.ShowMessage("讀卡作業異常，請重試，如持續異常請先異常代碼上傳並連絡資訊人員", MessageType.WARNING));
            }
        }

        private bool CheckReadCardResult(Prescription pre)
        {
            if (currentCard.IsRead)
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
                IsBusy = false;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(delegate {
                    MessageWindow.ShowMessage("補卡失敗，如卡片異常請選擇異常代碼。", MessageType.ERROR);
                });
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

        #endregion
    }
}
