using His_Pos.Class;
using His_Pos.LocationManage;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace His_Pos.H4_BASIC_MANAGE.LocationManage
{
    /// <summary>
    /// NewLocationView.xaml 的互動邏輯
    /// </summary>
    public partial class NewLocationView : Window
    {
        public NewLocationView()
        {
            InitializeComponent();
            txtNewLocation.Focus();
        }

        private void ButtonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            string error = IsCheck();
            if (error == "")
            {
                LocationManageView.Instance.NewLocation(null, txtNewLocation.Text);
                Close();
            }
            else
            {
                txtNewLocation.Text = "";
                MessageBox.Show(error);
            }
        }

        private string IsCheck()
        {
            int number = 0;
            bool canConvert = int.TryParse(txtNewLocation.Text.Substring(0, 1), out number);
            if (canConvert)
            {
                return "第一個字不可以為數字";
            }
            foreach (ContentControl contentControl in LocationManageView.Instance.LocationCanvus.Children)
            {
                LocationControl locationControl = (LocationControl)contentControl.Content;
                if (locationControl.Name == txtNewLocation.Text) return "已有同名櫃位";
            }

            return "";
        }
    }
}