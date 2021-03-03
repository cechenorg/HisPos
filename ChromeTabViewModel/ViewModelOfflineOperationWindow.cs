using His_Pos.NewClass.Medicine.Position;
using His_Pos.NewClass.Medicine.Usage;
using His_Pos.NewClass.OfflineDataSet;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ZeroFormatter;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelOfflineOperationWindow : OfflineOperationWindowViewModel, IChromeTabViewModel
    {
        private bool _canMoveTabs;
        private bool _showAddButton;

        public double WindowWidth => SystemParameters.WorkArea.Width * 0.8;
        public double WindowHeight => SystemParameters.WorkArea.Height * 0.8;
        public double StartTop => (SystemParameters.WorkArea.Height - WindowHeight) / 2;
        public double StartLeft => (SystemParameters.WorkArea.Width - WindowWidth) / 2;
        public static Institutions Institutions { get; set; }
        public static Divisions Divisions { get; set; }
        public static AdjustCases AdjustCases { get; set; }
        public static PaymentCategories PaymentCategories { get; set; }
        public static PrescriptionCases PrescriptionCases { get; set; }
        public static Copayments Copayments { get; set; }
        public static SpecialTreats SpecialTreats { get; set; }
        public static Usages Usages { get; set; }
        public static Positions Positions { get; set; }
        public static OfflineDataSet OfflineDataSet { get; set; }

        public bool CanMoveTabs
        {
            get { return _canMoveTabs; }
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }

        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }

        public ViewModelOfflineOperationWindow()
        {
            AddTabCommandAction("調劑");
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);

            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            CanMoveTabs = true;
            ShowAddButton = false;
            OfflineDataSet = ZeroFormatterSerializer.Deserialize<OfflineDataSet>(File.ReadAllBytes("C:\\Program Files\\HISPOS\\OfflineDataSet.singde"));
            Institutions = new Institutions(OfflineDataSet.Institutions);
            Divisions = new Divisions(OfflineDataSet.Divisions);
            AdjustCases = new AdjustCases(OfflineDataSet.AdjustCases);
            PaymentCategories = new PaymentCategories(OfflineDataSet.PaymentCategories);
            PrescriptionCases = new PrescriptionCases(OfflineDataSet.PrescriptionCases);
            Copayments = new Copayments(OfflineDataSet.Copayments);
            SpecialTreats = new SpecialTreats(OfflineDataSet.SpecialTreats);
            Usages = new Usages(OfflineDataSet.Usages);
            Positions = new Positions(OfflineDataSet.Positions);
        }
    }
}