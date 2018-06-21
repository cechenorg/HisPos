using His_Pos.Class.Employee;
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

namespace His_Pos.H4_BASIC_MANAGE.EmployeeManage
{

    /// <summary>
    /// EmployeeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeManageView : UserControl, INotifyPropertyChanged
    {

        public Employee originEmployee;
        public Employee employee;
        public Employee Employee
        {
            get
            {
                return employee;
            }
            set
            {
                employee = value;
                NotifyPropertyChanged("Employee");
            }
        }

        public ObservableCollection<Employee> employeeCollection = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> EmployeeCollection
        {
            get
            {
                return employeeCollection;
            }
            set
            {
                employeeCollection = value;
                NotifyPropertyChanged("EmployeeCollection");
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
        public EmployeeManageView()
        {
            InitializeComponent();
            GetEmployeeData();
            DataContext = this;
        }
        private void GetEmployeeData() {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetEmployeeData(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }
        private void DataGridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null) return;
            originEmployee = new Employee((Employee)(sender as DataGrid).SelectedItem);
            Employee = new Employee((Employee)(sender as DataGrid).SelectedItem);
            //richtextbox.AppendText(Employee.);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Employee = originEmployee;
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (EmployeeCollection[i].Id == Employee.Id) {
                    EmployeeCollection[i] = Employee;
                    EmployeeDb.SaveEmployeeData(Employee);
                    break;
                }
            }
        }
    }
}
