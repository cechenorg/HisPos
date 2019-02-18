using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Person.MedicalPerson;
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
        private Medicine selectedMedicine;
        public Medicine SelectedMedicine
        {
            get => selectedMedicine;
            set
            {
                if (SelectedMedicine is MedicineNHI || SelectedMedicine is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial)
                    ((IDeletableProduct)SelectedMedicine).IsSelected = false;
                Set(() => SelectedMedicine, ref selectedMedicine, value);
                if (SelectedMedicine is MedicineNHI || SelectedMedicine is MedicineOTC || SelectedMedicine is MedicineSpecialMaterial)
                    ((IDeletableProduct)SelectedMedicine).IsSelected = true;
            }
        }

        private bool CheckEdit()
        {
            var preEdited = !EditedPrescription.PublicInstancePropertiesEqual(OriginalPrescription);
            var insEdited = !EditedPrescription.Treatment.Institution.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Institution);
            var divEdited = !EditedPrescription.Treatment.Division.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Division);
            var pharmacyEdited = !EditedPrescription.Treatment.Pharmacist.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Pharmacist);
            var treatDateEdited = DateTime.Compare((DateTime)EditedPrescription.Treatment.TreatDate,(DateTime)OriginalPrescription.Treatment.TreatDate) != 0;
            var adjustDateEdited = DateTime.Compare((DateTime)EditedPrescription.Treatment.AdjustDate, (DateTime)OriginalPrescription.Treatment.AdjustDate) != 0;
            var mainDiseaseEdited = !EditedPrescription.Treatment.MainDisease.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.MainDisease);
            var subDiseaseEdited = !EditedPrescription.Treatment.SubDisease.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.SubDisease);
            var adjCaseEdited = !EditedPrescription.Treatment.AdjustCase.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.AdjustCase);
            var preCaseEdited = !EditedPrescription.Treatment.PrescriptionCase.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.PrescriptionCase);
            var copEdited = !EditedPrescription.Treatment.Copayment.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.Copayment);
            var payEdited = !EditedPrescription.Treatment.PaymentCategory.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.PaymentCategory);
            var speTreEdited = !EditedPrescription.Treatment.SpecialTreat.PublicInstancePropertiesEqual(OriginalPrescription.Treatment.SpecialTreat);
            var medEdited = !EditedPrescription.Medicines.PublicInstancePropertiesEqual(OriginalPrescription.Medicines);
            return preEdited || insEdited || divEdited || pharmacyEdited || treatDateEdited || adjustDateEdited || mainDiseaseEdited  || subDiseaseEdited || adjCaseEdited || preCaseEdited || copEdited
                   || payEdited || speTreEdited || medEdited;
        }

        private MedSelectWindow MedicineWindow { get; set; }
        private Visibility isEdit;
        public Visibility IsEdit 
        {
            get => isEdit;
            set
            {
                Set(() => IsEdit, ref isEdit, value);
            }
        }
        private Visibility isNoCard;
        public Visibility IsNoCard
        {
            get => isNoCard;
            set
            {
                Set(() => IsNoCard, ref isNoCard, value);
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
        #region Commands
        public RelayCommand PrintMedBag { get; set; }
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
        public RelayCommand ComboBoxSelectionChanged { get; set; }
        public RelayCommand TextBoxTextChanged { get; set; }
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
            viewModel = vm;
            MainWindow.ServerConnection.OpenConnection();
            selected.AdjustMedicinesType(true);
            MainWindow.ServerConnection.CloseConnection();
            OriginalPrescription = selected;
            Init((Prescription)selected.Clone());
        }

        #region InitialFunctions
        private void Init(Prescription selected)
        {
            InitPrescription(selected);
            IsEdit = Visibility.Hidden;
            IsNoCard = EditedPrescription.PrescriptionStatus.IsGetCard ? Visibility.Hidden : Visibility.Visible;
            InitialItemsSources();
            InitialCommandActions();
            RegisterMessengers();
        }

        private void InitPrescription(Prescription selected)
        {
            MainWindow.ServerConnection.OpenConnection();
            selected.Patient = selected.Patient.GetCustomerByCusId(selected.Patient.ID);
            MainWindow.ServerConnection.OpenConnection();
            EditedPrescription = selected;
            if (EditedPrescription.Treatment.Division != null)
                EditedPrescription.Treatment.Division = VM.GetDivision(EditedPrescription.Treatment.Division?.ID);
            EditedPrescription.Treatment.Pharmacist =
                VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IdNumber.Equals(EditedPrescription.Treatment.Pharmacist.IdNumber));
            EditedPrescription.Treatment.AdjustCase = VM.GetAdjustCase(EditedPrescription.Treatment.AdjustCase.Id);
            EditedPrescription.Treatment.Copayment = VM.GetCopayment(EditedPrescription.Treatment.Copayment?.Id);
            if (EditedPrescription.Treatment.PrescriptionCase != null)
                EditedPrescription.Treatment.PrescriptionCase = VM.GetPrescriptionCases(EditedPrescription.Treatment.PrescriptionCase?.ID);
            if (EditedPrescription.Treatment.SpecialTreat != null)
                EditedPrescription.Treatment.SpecialTreat = VM.GetSpecialTreat(EditedPrescription.Treatment.SpecialTreat?.ID);
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
            PrintMedBag = new RelayCommand(PrintMedBagAction);
            ShowCommonInstitutionSelectionWindow = new RelayCommand(ShowCommonInsSelectionWindowAction);
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            GetMainDiseaseCodeById = new RelayCommand<string>(GetMainDiseaseCodeByIdAction);
            GetSubDiseaseCodeById = new RelayCommand<string>(GetSubDiseaseCodeByIdAction);
            AdjustCaseSelectionChanged = new RelayCommand(AdjustCaseSelectionChangedAction);
            CopaymentSelectionChanged = new RelayCommand(CopaymentSelectionChangedAction);
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            MedicinePriceChanged = new RelayCommand(CountMedicinePoint);
            RedoEdit = new RelayCommand(RedoEditAction);
            EditComplete = new RelayCommand(EditCompleteAction);
            ComboBoxSelectionChanged = new RelayCommand(SelectionChangedAction);
            TextBoxTextChanged = new RelayCommand(TextBoxTextChangedAction);
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<Institution>(this, nameof(PrescriptionEditViewModel) + "InsSelected", GetSelectedInstitution);
            Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            Messenger.Default.Register<NotificationMessage>("DeleteMedicine", DeleteMedicine);
        }
        #endregion
        #region CommandActions
        private void PrintMedBagAction()
        {
            var medBagPrint = new ConfirmWindow(StringRes.PrintMedBag, StringRes.PrintConfirm);
            if (medBagPrint.DialogResult != null && (bool)medBagPrint.DialogResult)
            {
                var printBySingleMode = new MedBagSelectionWindow();
                var singleMode = false;
                if (printBySingleMode.DialogResult != null)
                    singleMode = printBySingleMode.DialogResult != null && (bool)printBySingleMode.DialogResult;
                var receiptPrint = false;
                if (EditedPrescription.PrescriptionPoint.AmountsPay > 0)
                {
                    var receiptResult = new ConfirmWindow(StringRes.PrintReceipt, StringRes.PrintConfirm);
                    if (receiptResult.DialogResult != null)
                        receiptPrint = (bool)receiptResult.DialogResult;
                }
                EditedPrescription.PrintMedBag(singleMode);
                if(receiptPrint)
                    EditedPrescription.PrintReceipt();
            }
            else
            {
                MessageWindow.ShowMessage(StringRes.InsertPrescriptionSuccess, MessageType.SUCCESS);
            }
        }
        private void ShowCommonInsSelectionWindowAction()
        {
            var commonInsSelectionWindow = new CommonHospitalsWindow(ViewModelEnum.PrescriptionEdit);
            commonInsSelectionWindow.ShowDialog();
        }
        private void ShowInsSelectionWindowAction(string search)
        {
            if (search.Length < 4)
            {
                MessageWindow.ShowMessage(Resources.ShortSearchString + "4", MessageType.WARNING);
                return;
            }
            if (EditedPrescription.Treatment.Institution != null && !string.IsNullOrEmpty(EditedPrescription.Treatment.Institution.FullName) && search.Equals(EditedPrescription.Treatment.Institution.FullName))
            {
                Messenger.Default.Send(new NotificationMessage("FocusDivision"));
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
                Messenger.Default.Send(new NotificationMessage("FocusSubDisease"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                EditedPrescription.Treatment.MainDisease = result;
            }
        }

        private void GetSubDiseaseCodeByIdAction(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!string.IsNullOrEmpty(EditedPrescription.Treatment.MainDisease.FullName) && id.Equals(EditedPrescription.Treatment.MainDisease.FullName))
            {
                Messenger.Default.Send(new NotificationMessage("FocusChronicTotal"));
                return;
            }
            var result = DiseaseCode.GetDiseaseCodeByID(id);
            if (result != null)
            {
                EditedPrescription.Treatment.SubDisease = result;
            }
        }
        private void AdjustCaseSelectionChangedAction()
        {
            if (EditedPrescription.Treatment.AdjustCase != null &&
                EditedPrescription.Treatment.AdjustCase.Id.Equals("0"))
            {
                EditedPrescription.Treatment.Clear();
                SetMedicinesPaySelf();
            }
            EditedPrescription.CheckPrescriptionVariable();
            CheckEditStatus();
        }

        private void CopaymentSelectionChangedAction()
        {
            EditedPrescription.CountPrescriptionPoint();
        }
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.ShortSearchString + "5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionEdit);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit);
                MedicineWindow.ShowDialog();
            }
            else if (productCount == 1)
            {
                MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.PrescriptionEdit);
            }
            else
            {
                MessageWindow.ShowMessage(StringRes.MedicineNotFound, MessageType.WARNING);
            }
        }
        private void CountMedicinePoint()
        {
            EditedPrescription.CountPrescriptionPoint();
        }
        private void EditCompleteAction()
        {
            if (CheckEdit())
            {
                var error = EditedPrescription.CheckPrescriptionRule(false);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageWindow.ShowMessage(error, MessageType.ERROR);
                    return;
                }
                EditedPrescription.Update();
                EditedPrescription.AdjustMedicines(OriginalPrescription.Medicines);
                switch (viewModel)
                {
                    case ViewModelEnum.PrescriptionSearch:
                        Messenger.Default.Send(new NotificationMessage(nameof(PrescriptionSearchViewModel)+ "PrescriptionEdited"));
                        break;
                    case ViewModelEnum.DeclareFileManage:
                        Messenger.Default.Send(new NotificationMessage(nameof(DeclareFileManageViewModel) + "PrescriptionEdited"));
                        break;
                }
            }
            Messenger.Default.Send(new NotificationMessage("ClosePrescriptionEditWindow"));
        }

        private void RedoEditAction()
        {
            InitPrescription((Prescription)OriginalPrescription.Clone());
        }

        private void SelectionChangedAction()
        {
            CheckEditStatus();
        }
        private void TextBoxTextChangedAction()
        {
            if (!EditedPrescription.Treatment.Division.ID.Equals(OriginalPrescription.Treatment.Division.ID))
                IsEdit = Visibility.Visible;
        }
        #endregion
        #region MessangerReceive
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            EditedPrescription.Treatment.Institution = receiveSelectedInstitution;
            CheckEdit();
        }
        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification == nameof(PrescriptionEditViewModel))
            {
                var selected = EditedPrescription.Medicines.IndexOf(SelectedMedicine);
                if (selected < 0 || selected >= EditedPrescription.Medicines.Count) return;
                EditedPrescription.AddMedicineBySearch(msg.Content.ID, selected);
                EditedPrescription.CountPrescriptionPoint();
                if (selected == EditedPrescription.Medicines.Count - 1)
                    EditedPrescription.Medicines.Add(new Medicine());
                Messenger.Default.Send(selected, "FocusDosage");
            }
        }

        private void DeleteMedicine(NotificationMessage obj)
        {
            if (EditedPrescription.Medicines.Count <= SelectedMedicinesIndex) return;
            var m = EditedPrescription.Medicines[SelectedMedicinesIndex];
            if (m is MedicineNHI med && !string.IsNullOrEmpty(med.Source) ||
                m is MedicineOTC otc && !string.IsNullOrEmpty(otc.Source) ||
                m is MedicineSpecialMaterial special && !string.IsNullOrEmpty(special.Source))
            {
                EditedPrescription.Medicines.RemoveAt(SelectedMedicinesIndex);
                EditedPrescription.CountPrescriptionPoint();
                if (EditedPrescription.Medicines.Count == 0)
                {
                    EditedPrescription.Medicines.Add(new Medicine());
                }
            }
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
            IsEdit = CheckEdit() ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion
    }
}
