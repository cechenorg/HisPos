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
        private bool isFirst = true;

        private ManageManufactory currentManufactory;
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
            
            CurrentManufactory = ((sender as DataGrid).SelectedItem as ManageManufactory).Clone() as ManageManufactory;

            Notes.Document.Blocks.Clear();
            Notes.AppendText(CurrentManufactory.Note);

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
            if ((sender as DataGrid).SelectedItem is null) return;

            bool isChanged = IsChangedLbl.Content.Equals("已修改");

            if (((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryGetOverviews is null)
                ((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryGetOverviews = ManufactoryDb.GetManufactoryGetOverview(((sender as DataGrid).SelectedItem as ManufactoryPrincipal).Id);

            if (((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryPayOverviews is null)
                ((sender as DataGrid).SelectedItem as ManufactoryPrincipal).ManufactoryPayOverviews = ManufactoryDb.GetManufactoryPayOverview(((sender as DataGrid).SelectedItem as ManufactoryPrincipal).Id);

            PrincipalDetail.DataContext = (sender as DataGrid).SelectedItem;

            if (!isChanged) InitDataChanged();
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

            CurrentManufactory.Note = new TextRange(Notes.Document.ContentStart, Notes.Document.ContentEnd).Text;

            ManageManufactories[index] = CurrentManufactory;

            ManageManufactoryDataGrid.SelectedIndex = index;

            ManufactoryDb.UpdateManageManufactory(CurrentManufactory);
        }

        private void DeleteManufactory_Click(object sender, MouseButtonEventArgs e)
        {
            ManageManufactory manufactory = ManageManufactoryDataGrid.SelectedItem as ManageManufactory;

            ManufactoryDb.DeleteManageManufactory(manufactory.Id);

            ManageManufactories.Remove(manufactory);

            ManageManufactoryDataGrid.SelectedIndex = 0;
            ManageManufactoryDataGrid.ScrollIntoView(ManageManufactoryDataGrid.SelectedItem);
        }

        private void ManufactoryManageView_GotFocus(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }
    }
}
