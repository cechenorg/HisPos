using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelMainWindow : MainViewModel, IViewModelMainWindow
    {
        //this property is to show you can lock the tabs with a binding
        private bool _canMoveTabs;
        public bool CanMoveTabs
        {
            get => _canMoveTabs;
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }
        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;
        public bool ShowAddButton
        {
            get => _showAddButton;
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }

        private string _cardReaderStatus;
        public string CardReaderStatus
        {
            get => _cardReaderStatus;
            set
            {
                Set(() => CardReaderStatus, ref _cardReaderStatus, value);
            }
        }
        private string _samDcStatus;
        public string SamDcStatus
        {
            get => _samDcStatus;
            set
            {
                Set(() => SamDcStatus, ref _samDcStatus, value);
            }
        }

        private string _hpcCardStatus;
        public string HpcCardStatus
        {
            get => _hpcCardStatus;
            set
            {
                Set(() => HpcCardStatus, ref _hpcCardStatus, value);
            }
        }

        private bool _isConnectionOpened;

        public bool IsConnectionOpened
        {
            get => _isConnectionOpened;
            set
            {
                _isConnectionOpened = value;
            }
        }

        private bool _hisApiException;

        public bool HisApiException
        {
            get => _hisApiException;
            set
            {
                _hisApiException = value;
            }
        }
        private bool _isIcCardValid;
        public bool IsIcCardValid
        {
            get => _isIcCardValid;
            set
            {
                _isIcCardValid = value;
            }
        }
        private bool _isHpcValid;

        public bool IsHpcValid
        {
            get => _isHpcValid;
            set
            {
                Set(() => IsHpcValid, ref _isHpcValid, value);
            }
        }

        private bool _isVerifySamDc;

        public bool IsVerifySamDc
        {
            get => _isVerifySamDc;
            set
            {
                Set(() => IsVerifySamDc, ref _isVerifySamDc, value);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(() => IsBusy, ref _isBusy, value);
            }
        }
        private string _busyContent;

        public string BusyContent
        {
            get => _busyContent;
            set
            {
                Set(() => BusyContent, ref _busyContent, value);
            }
        }

        public static Institutions Institutions { get; set; }
        public static Divisions Divisions { get; set; }
        public static AdjustCases AdjustCases { get; set; }
        public static PaymentCategories PaymentCategories { get; set; }
        public static PrescriptionCases PrescriptionCases { get; set; }
        public static Copayments Copayments { get; set; }
        public static SpecialTreats SpecialTreats { get; set; }
        public static Usages Usages { get; set; }
        public static Positions Positions { get; set; }
        public static Pharmacy CurrentPharmacy { get; set; }
        public static Employee CurrentUser { get; set; }
        public ViewModelMainWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            CurrentPharmacy = Pharmacy.GetCurrentPharmacy();
            CurrentPharmacy.MedicalPersonnels = new MedicalPersonnels();
            CanMoveTabs = true;
            ShowAddButton = false;
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
        }

        private RelayCommand initialData;
        public RelayCommand InitialData
        {
            get =>
                initialData ??
                (initialData = new RelayCommand(ExecuteInitData));
            set => initialData = value;
        }

        private void ExecuteInitData()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得醫療院所...";
                Institutions = new Institutions(true);
                BusyContent = "取得科別...";
                Divisions = new Divisions();
                BusyContent = "取得調劑案件...";
                AdjustCases = new AdjustCases();
                BusyContent = "取得給付類別...";
                PaymentCategories = new PaymentCategories();
                BusyContent = "取得處方案件...";
                PrescriptionCases = new PrescriptionCases();
                BusyContent = "取得部分負擔...";
                Copayments = new Copayments();
                BusyContent = "取得部分負擔...";
                SpecialTreats = new SpecialTreats();
                BusyContent = "取得藥品用法...";
                Usages = new Usages();
                BusyContent = "取得藥品途徑...";
                Positions = new Positions();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
    }
}
