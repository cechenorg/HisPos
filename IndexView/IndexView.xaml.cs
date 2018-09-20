using His_Pos.Class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class.Declare;

namespace His_Pos.IndexView
{
    /// <summary>
    /// IndexView.xaml 的互動邏輯
    /// </summary>
    public partial class IndexView : UserControl
    {
        public IndexView()
        {
            InitializeComponent();
            Date.Content = DateTime.Today.ToString("yyyy/MM/dd");
            ChronicDb.CaculateChironic();
        }

        public void UpdateUI() {

        }
    }
}
