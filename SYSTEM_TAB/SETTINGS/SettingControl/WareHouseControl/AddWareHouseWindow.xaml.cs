using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System.Windows;

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

        #endregion ----- Define Variables -----

        public AddWareHouseWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ----- Define Functions -----

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (!CheckNewName()) return;

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

        #endregion ----- Define Functions -----
    }
}