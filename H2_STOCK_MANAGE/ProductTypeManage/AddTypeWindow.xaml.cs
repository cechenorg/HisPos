using His_Pos.Class.ProductType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace His_Pos.H4_BASIC_MANAGE.ProductTypeManage
{
    /// <summary>
    /// AddTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddTypeWindow : Window
    {
        public ObservableCollection<ProductTypeManageMaster> TypeManageMasters { get; set; }

        public ProductType newProductType;

        public AddTypeWindow(ObservableCollection<ProductTypeManageMaster> typeManageMasters, ProductTypeManageMaster selected)
        {
            InitializeComponent();

            DataContext = this;

            TypeManageMasters = typeManageMasters;
            UpdateUi();
            BigTypeCombo.SelectedItem = selected;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }

        private void UpdateUi()
        {
            if (SelectionHint is null || SelectionHint is null) return;

            if( (bool)SmallTypeRadioButton.IsChecked)
            {
                SelectionHint.Content = "選擇所屬大類別";
                BigTypeCombo.Visibility = Visibility.Visible;
            }
            else
            {
                SelectionHint.Content = "輸入大類別資訊";
                BigTypeCombo.Visibility = Visibility.Collapsed;
            }
        }

        private void ConfrimClick(object sender, RoutedEventArgs e)
        {
            if(CheckEmptyData())
            {
                if ((bool)BigTypeRadioButton.IsChecked)
                {
                    ProductTypeManageMaster productTypeManageMaster = new ProductTypeManageMaster(ChiName.Text, EngName.Text);

                    newProductType = productTypeManageMaster;
                }
                else
                {
                    ProductTypeManageDetail productTypeManageDetail = new ProductTypeManageDetail(BigTypeCombo.SelectedValue.ToString(), ChiName.Text, EngName.Text);

                    newProductType = productTypeManageDetail;
                }

                Close();
            }
        }

        private bool CheckEmptyData()
        {
            string error = "";

            if (ChiName.Text.Equals(""))
                error += "未填寫中文名稱!\n";

            if(EngName.Text.Equals(""))
                error += "未填寫英文簡碼!\n";

            if( EngName.Text.Length != 2 )
                error += "英文簡碼須為兩碼!\n";

            if( error.Length != 0 )
            {
                MessageWindow messageWindow = new MessageWindow(error, Class.MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return false;
            }

            return true;
        }
    }
}
