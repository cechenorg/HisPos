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
using System.Windows.Shapes;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// CooperativePrescriptSelectWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativePrescriptSelectWindow : Window
    {
        public CooperativePrescriptSelectWindow()
        {
            InitializeComponent();
            WebApi.GetXmlByDate("5932084604",DateTime.Today.AddDays(-5), DateTime.Today.AddDays(-5));
        }
    }
}
