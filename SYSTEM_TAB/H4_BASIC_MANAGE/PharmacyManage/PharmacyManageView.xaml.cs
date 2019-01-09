using His_Pos.Class.Pharmacy;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class.Manufactory;

namespace His_Pos.H4_BASIC_MANAGE.PharmacyManage
{
    /// <summary>
    /// PharmacyManageView.xaml 的互動邏輯
    /// </summary>
    public partial class PharmacyManageView : UserControl, INotifyPropertyChanged
    {
        private bool isFirst = true;

        private ManagePharmacy currentPharmacy;
        public ManagePharmacy CurrentPharmacy
        {
            get { return currentPharmacy; }
            set
            {
                currentPharmacy = value;
                NotifyPropertyChanged("CurrentPharmacy");
            }
        }
        public ObservableCollection<ManagePharmacy> ManagePharmacies { get; set; }

        public PharmacyManageView()
        {
            InitializeComponent();
            DataContext = this;
            InitPharmacy();
        }

        private void InitPharmacy()
        {
            ManagePharmacies = PharmacyDb.GetManagePharmacy();
            ManagePharmacyDataGrid.SelectedIndex = 0;
        }

        private void DataChanged()
        {
            if (isFirst) return;

            IsChangedLbl.Content = "已修改";
            IsChangedLbl.Foreground = Brushes.Red;

            ConfirmBtn.IsEnabled = true;
            CancelBtn.IsEnabled = true;
        }

        private void InitDataChanged()
        {
            IsChangedLbl.Content = "未修改";
            IsChangedLbl.Foreground = Brushes.Black;

            ConfirmBtn.IsEnabled = false;
            CancelBtn.IsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void ManagePharmacy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem is null) return;

            CurrentPharmacy = (ManagePharmacy)(ManagePharmacyDataGrid.SelectedItem as ManagePharmacy).Clone();

            Notes.Document.Blocks.Clear();
            Notes.AppendText(CurrentPharmacy.Note);

            UpdateUi();
            InitDataChanged();
        }

        private void Principal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem is null) return;

            bool isChanged = IsChangedLbl.Content.Equals("已修改");

            //if (((sender as DataGrid).SelectedItem as PharmacyPrincipal).PharmacyGetOverviews is null)
            //    ((sender as DataGrid).SelectedItem as PharmacyPrincipal).PharmacyGetOverviews = null

            PrincipalDetail.DataContext = (sender as DataGrid).SelectedItem;

            if (!isChanged) InitDataChanged();
        }

        private void AddManufactory_Click(object sender, MouseButtonEventArgs e)
        {
            ManagePharmacy newManagePharmacy = PharmacyDb.AddNewManagePharmacy();

            ManagePharmacies.Add(newManagePharmacy);

            ManagePharmacyDataGrid.SelectedItem = newManagePharmacy;
            ManagePharmacyDataGrid.ScrollIntoView(newManagePharmacy);
        }

        private void ConfirmChanged_OnClick(object sender, RoutedEventArgs e)
        {
            int index = ManagePharmacyDataGrid.SelectedIndex;

            CurrentPharmacy.Note = new TextRange(Notes.Document.ContentStart, Notes.Document.ContentEnd).Text;

            ManagePharmacies[index] = CurrentPharmacy;

            ManagePharmacyDataGrid.SelectedIndex = index;

            PharmacyDb.UpdateManagePharmacy(CurrentPharmacy);
        }

        private void DeleteManufactory_Click(object sender, MouseButtonEventArgs e)
        {
            ManagePharmacy pharmacy = ManagePharmacyDataGrid.SelectedItem as ManagePharmacy;

            ManufactoryDb.DeleteManageManufactory(pharmacy.Id);

            ManagePharmacies.Remove(pharmacy);

            ManagePharmacyDataGrid.SelectedIndex = 0;
            ManagePharmacyDataGrid.ScrollIntoView(ManagePharmacyDataGrid.SelectedItem);
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (ManagePharmacyDataGrid.SelectedItem is null) return;

            CurrentPharmacy = (ManagePharmacyDataGrid.SelectedItem as ManagePharmacy).Clone() as ManagePharmacy;
            UpdateUi();
            InitDataChanged();
        }

        private void UpdateUi()
        {
            if (CurrentPharmacy.PharmacyPrincipals.Count > 0)
            {
                PrincipalDetail.IsEnabled = true;
                PrincipalDataGrid.SelectedIndex = 0;

                PrincipalDataGrid.Items.Filter = p => (p as PharmacyPrincipal).IsEnable;
            }
            else
            {
                PrincipalDetail.IsEnabled = false;
                PrincipalDetail.DataContext = null;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataChanged();
        }

        private void PharmacyManageView_Loaded(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }

        private void AddPrincipal_Click(object sender, RoutedEventArgs e)
        {
            PharmacyPrincipal newPrincipal = new PharmacyPrincipal();

            CurrentPharmacy.PharmacyPrincipals.Add(newPrincipal);
            PrincipalDataGrid.SelectedItem = newPrincipal;
            PrincipalDataGrid.ScrollIntoView(newPrincipal);
            PrincipalDetail.IsEnabled = true;
            DataChanged();
        }

        private void DelPrincipal_Click(object sender, RoutedEventArgs e)
        {
            (PrincipalDetail.DataContext as PharmacyPrincipal).IsEnable = false;
            UpdateUi();
            DataChanged();
        }
    }
}
