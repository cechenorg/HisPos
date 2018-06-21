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

            InitManufactory();


            DataContext = this;
        }

        private void InitManufactory()
        {
            ManageManufactories = ManufactoryDb.GetManageManufactory();
            ManageManufactoryDataGrid.SelectedIndex = 0;
        }

        private void Manufactory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentManufactory = (sender as DataGrid).SelectedItem as ManageManufactory;

            if (CurrentManufactory.ManufactoryPrincipals.Count > 0)
                PrincipalDataGrid.SelectedIndex = 0;
            else
                PrincipalDetail.DataContext = null;
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
    }
}
