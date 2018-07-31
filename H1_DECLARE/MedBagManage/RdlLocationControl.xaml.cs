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

namespace His_Pos.H4_BASIC_MANAGE.MedBagManage
{
    /// <summary>
    /// RdlLocationControl.xaml 的互動邏輯
    /// </summary>
    public partial class RdlLocationControl : UserControl
    {
        public int id;
        public string LabelContent { get; set; }

        public Brush GridBackground
        {
            get { return LabelContent.Equals("")? Brushes.Transparent: (Brush)FindResource("GridSelected"); }
        }

        public RdlLocationControl(int locId,string name)
        {
            InitializeComponent();
            id = locId;
            LabelContent = name;
            DataContext = this;
        }

        public RdlLocationControl()
        {
            InitializeComponent();
        }
    }
}