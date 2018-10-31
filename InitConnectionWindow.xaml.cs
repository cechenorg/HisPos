using System.Windows;
using His_Pos.Class;

namespace His_Pos
{
    /// <summary>
    /// InitConnectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InitConnectionWindow : Window
    {
        public InitConnectionWindow()
        {
            InitializeComponent();
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            DatabaseControl.ConfirmConnectionChange_Click(sender, e);

            if(DatabaseControl.LocalConnection.ConnectionPass && DatabaseControl.GlobalConnection.ConnectionPass)
                Close();
            else
            {
                MessageWindow messageWindow = new MessageWindow("連線失敗 請確認資料是否正確", MessageType.ERROR);
                messageWindow.ShowDialog();
            }
        }
    }
}
