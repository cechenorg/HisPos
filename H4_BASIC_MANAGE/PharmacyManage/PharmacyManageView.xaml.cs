using His_Pos.Class.Pharmacy;
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

namespace His_Pos.H4_BASIC_MANAGE.PharmacyManage
{
    /// <summary>
    /// PharmacyManageView.xaml 的互動邏輯
    /// </summary>
    public partial class PharmacyManageView : UserControl
    {
        private bool isFirst = true;

        private ManagePharmacy currentManufactory;
        public ManagePharmacy CurrentManufactory
        {
            get { return currentManufactory; }
            set
            {
                currentManufactory = value;
                NotifyPropertyChanged("CurrentManufactory");
            }
        }
        public ObservableCollection<ManagePharmacy> ManagePharmacies { get; set; }

        public PharmacyManageView()
        {
            InitializeComponent();
            InitPharmacy();

        }

        private void InitPharmacy()
        {
            ManagePharmacies = PharmacyDb.GetManagePharmacy();
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

    }
}
