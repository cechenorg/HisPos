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
using His_Pos.Class.ProductType;

namespace His_Pos.H4_BASIC_MANAGE.ProductTypeManage
{
    /// <summary>
    /// DeleteTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteTypeWindow : Window
    {
        public DeleteTypeWindow(ProductTypeManageMaster master, ProductTypeManageDetail detail = null)
        {
            InitializeComponent();

            BigType.Content = master.Name;

            if (detail is null)
            {

                SmallType.IsEnabled = false;
                BigTypeRadioButton.IsChecked = true;
            }
            else
            {
                SmallType.Content = detail.Name;
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (CheckDeletable())
            {
                
            }
        }

        private bool CheckDeletable()
        {
            throw new NotImplementedException();
        }
    }
}
