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
        private bool isFirst = true;
        public ObservableCollection<CustomItem> oc;
        public ObservableCollection<CustomItem> OC
        {
            get
            {
                return oc;
            }
            set
            {
                oc = value;
                NotifyPropertyChanged("OC");
            }
        }
        ObservableCollection<CustomItem> ChildOC { get; set; }
        public void OnCheck()
        {
            ChildOC = new ObservableCollection<CustomItem>() { };
            foreach (CustomItem item in OC)
            {
                if (item.Checked == true)
                {
                    ChildOC.Add(item);
                    foreach (CustomItem subitem in item.Children)
                    {
                        if (subitem.Checked == true)
                        {
                            ChildOC.Add(subitem);
                        }
                    }
                }
            }
            listbox.ItemsSource = ChildOC;
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) { OnCheck(); }
        private void CheckBox_Loaded(object sender, RoutedEventArgs e) { OnCheck(); }
    public class CustomItem
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public ObservableCollection<CustomItem> Children { get; set; }
    }
   
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
            
            OC = new ObservableCollection<CustomItem>()
        {
        new CustomItem(){Name="DI", Checked=true,
          Children = new ObservableCollection<CustomItem>()
          {
            new CustomItem(){Name="SubItem11", Checked=false},
            new CustomItem(){Name="SubItem12", Checked=false},
            new CustomItem(){Name="SubItem13", Checked=false}
          }
        },
        new CustomItem(){Name="Item2", Checked=true,
          Children = new ObservableCollection<CustomItem>()
          {
            new CustomItem(){Name="SubItem21", Checked=true},
            new CustomItem(){Name="SubItem22", Checked=true},
            new CustomItem(){Name="SubItem23", Checked=true}
          }},
        new CustomItem(){Name="Item3", Checked=true,
          Children = new ObservableCollection<CustomItem>()
          {
            new CustomItem(){Name="SubItem31", Checked=false},
            new CustomItem(){Name="SubItem32", Checked=false},
            new CustomItem(){Name="SubItem33", Checked=false}
          }},
        new CustomItem(){Name="Item4", Checked=true,
          Children = new ObservableCollection<CustomItem>()
          {
            new CustomItem(){Name="SubItem41", Checked=false},
            new CustomItem(){Name="SubItem42", Checked=false},
            new CustomItem(){Name="SubItem43", Checked=false}
          }
        }
         };
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
            Employee = (Employee)((Employee)(sender as DataGrid).SelectedItem).Clone();
            richtextbox.Document.Blocks.Clear();
            richtextbox.AppendText(Employee.Description);
            InitDataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Employee newemployee = EmployeeCollection.Where(emp => emp.Id == Employee.Id).ToList()[0];
            Employee = (Employee)newemployee.Clone();
            richtextbox.Document.Blocks.Clear();
            richtextbox.AppendText(Employee.Description);
            InitDataChanged();
        }
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EmployeeCollection.Count; i++) {
                if (EmployeeCollection[i].Id == Employee.Id) {
                    EmployeeCollection[i] = Employee;
                    EmployeeCollection[i].Description = new TextRange(richtextbox.Document.ContentStart, richtextbox.Document.ContentEnd).Text;
                    EmployeeDb.SaveEmployeeData(Employee);
                    InitDataChanged();
                    break;
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int count = EmployeeCollection.Count;
            Employee employee = new Employee();
           var table = EmployeeDb.SaveEmployeeData(employee);
            employee.Id = table.Rows[0][0].ToString();
            EmployeeCollection.Add(employee);
            DataGridEmployee.SelectedIndex = count;
        }

        private void ButtonDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridEmployee.SelectedItem == null) return;
            EmployeeDb.DeleteEmployeeData((Employee)DataGridEmployee.SelectedItem);
            EmployeeCollection.Remove((Employee)DataGridEmployee.SelectedItem);
            DataGridEmployee.SelectedIndex = EmployeeCollection.Count-1;
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }
        private void DataChanged()
        {
            if (isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonCancel.IsEnabled = true;
            ButtonSubmit.IsEnabled = true;
        }

        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonCancel.IsEnabled = false;
            ButtonSubmit.IsEnabled = false;
        }
       
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }

      

     
    }
}
