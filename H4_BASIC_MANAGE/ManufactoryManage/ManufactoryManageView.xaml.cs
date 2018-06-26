using His_Pos.Class.Manufactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.ManufactoryManage
{
    /// <summary>
    /// ManufactoryManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ManufactoryManageView : UserControl, INotifyPropertyChanged
    {
        public ManageManufactory currentManufactory;
        public ManageManufactory CurrentManufactory
        {
            get { return currentManufactory; }
            set
            {
                currentManufactory = value;
                NotifyPropertyChanged("CurrentManufactory");
            }
        }
        public ObservableCollection<ManageManufactory> ManageManufactories { get; set; }

        public ManufactoryManageView()
        {
            InitializeComponent();
            DataContext = this;
            InitManufactory();
            
        }

        private void InitManufactory()
        {
            ManageManufactories = ManufactoryDb.GetManageManufactory();
            ManageManufactoryDataGrid.SelectedIndex = 0;
        }

        private void Manufactory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as DataGrid).SelectedItem is null) return;

            if (((sender as DataGrid).SelectedItem as ManageManufactory).ManufactoryStoreOrderOverviews is null)
                ((sender as DataGrid).SelectedItem as ManageManufactory).ManufactoryStoreOrderOverviews = ManufactoryDb.GetManufactoryStoreOrderOverview(((sender as DataGrid).SelectedItem as ManageManufactory).Id);
            
            CurrentManufactory = ((sender as DataGrid).SelectedItem as ManageManufactory).Clone() as ManageManufactory;
            UpdateUi();
            InitDataChanged();

        }

        private void UpdateUi()
        {
            if (CurrentManufactory.ManufactoryPrincipals.Count > 0)
            {
                PrincipalDetail.IsEnabled = true;
                PrincipalDataGrid.SelectedIndex = 0;
            }
            else
            {
                PrincipalDetail.IsEnabled = false;
                PrincipalDetail.DataContext = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void Principals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrincipalDetail.DataContext = (sender as DataGrid).SelectedItem;
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            if(ManageManufactoryDataGrid.SelectedItem is null) return;

            CurrentManufactory = (ManageManufactoryDataGrid.SelectedItem as ManageManufactory).Clone() as ManageManufactory;
            UpdateUi();
            InitDataChanged();
        }

        private void AddPrincipal_OnClick(object sender, RoutedEventArgs e)
        {
            ManufactoryPrincipal newPrincipal = new ManufactoryPrincipal();

            CurrentManufactory.ManufactoryPrincipals.Add(newPrincipal);
            PrincipalDataGrid.SelectedItem = newPrincipal;
            PrincipalDataGrid.ScrollIntoView(newPrincipal);
            PrincipalDetail.IsEnabled = true;
            DataChanged();
        }

        private void DeletePrincipal_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentManufactory.ManufactoryPrincipals.Remove(PrincipalDetail.DataContext as ManufactoryPrincipal);
            UpdateUi();
            DataChanged();
        }

        private void DataChanged()
        {
            IsChangedLbl.Content = "已修改";
            IsChangedLbl.Foreground = Brushes.Red;
        }

        private void InitDataChanged()
        {
            IsChangedLbl.Content = "未修改";
            IsChangedLbl.Foreground = Brushes.Black;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DataChanged();
        }

        private void AddManufactory_Click(object sender, MouseButtonEventArgs e)
        {
            ManageManufactory newManageManufactory = ManufactoryDb.AddNewManageManufactory();
            
            ManageManufactories.Add(newManageManufactory);

            ManageManufactoryDataGrid.SelectedItem = newManageManufactory;
            ManageManufactoryDataGrid.ScrollIntoView(newManageManufactory);
        }

        private void ConfirmChanged_OnClick(object sender, RoutedEventArgs e)
        {
            int index = ManageManufactoryDataGrid.SelectedIndex;

            ManageManufactories[index] = CurrentManufactory;

            ManageManufactoryDataGrid.SelectedIndex = index;
        }
    }
}
