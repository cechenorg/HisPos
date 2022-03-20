using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
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
                //LocationManageView.Instance.NewLocation(null, txtNewLocation.Text);
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
            /* int number = 0;
             bool canConvert = int.TryParse(txtNewLocation.Text.Substring(0, 1), out number);
             if (canConvert)
             {
                 return "第一個字不可以為數字";
             }
             foreach (ContentControl contentControl in LocationManageView.Instance.LocationCanvus.Children)
             {
                 LocationControl locationControl = (LocationControl)contentControl.Content;
                 if (locationControl.Name == txtNewLocation.Text) return "已有同名櫃位";
             }*/

            return "";
        }
    }
}