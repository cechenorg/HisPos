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
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.WareHouseControl
{
    /// <summary>
    /// AddWareHouseWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddWareHouseWindow : Window
    {
        #region ----- Define Variables -----
        public string NewName { get; set; }
        public bool IsNewNameConfirmed { get; set; } = false;
        #endregion

        public AddWareHouseWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ----- Define Functions -----
        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if(!CheckNewName()) return;

            IsNewNameConfirmed = true;
            Close();
        }

        private bool CheckNewName()
        {
            if (string.IsNullOrEmpty(NewName.Trim()))
            {
                MessageWindow.ShowMessage("名稱不可為空", MessageType.ERROR);
                return false;
            }

            return true;
        }
        #endregion
    }
}
