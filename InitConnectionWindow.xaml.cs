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
