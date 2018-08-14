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

namespace His_Pos.H1_DECLARE.MedBagManage
{
    /// <summary>
    /// RdlLocationControl.xaml 的互動邏輯
    /// </summary>
    public partial class RdlLocationControl : UserControl
    {
        public int Id { get; set; }
        public string LabelContent { get; set; }
        public string LabelName { get; set; }
        public Brush GridBackground => LabelContent.Equals("") ? Brushes.Transparent : (Brush)FindResource("GridSelected");

        public RdlLocationControl(int locId,string content ,string name)
        {
            InitializeComponent();
            Id = locId;
            LabelContent = content;
            LabelName = name;
            DataContext = this;
        }

        public RdlLocationControl()
        {
            InitializeComponent();
        }
    }
}