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

        public double MedBagRangeTop
        {
            get { return (double)GetValue(MedBagRangeTopProperty); }
            set { SetValue(MedBagRangeTopProperty, value); }
        }

        public static readonly DependencyProperty MedBagRangeTopProperty =
            DependencyProperty.Register("MedBagRangeTop",
                typeof(double),
                typeof(RdlLocationControl),
                new PropertyMetadata(0.0));

        public double MedBagRangeBottom
        {
            get { return (double)GetValue(MedBagRangeBottomProperty); }
            set { SetValue(MedBagRangeBottomProperty, value); }
        }

        public static readonly DependencyProperty MedBagRangeBottomProperty =
            DependencyProperty.Register("MedBagRangeBottom",
                typeof(double),
                typeof(RdlLocationControl),
                new PropertyMetadata(0.0));

        public double MedBagRangeLeft
        {
            get { return (double)GetValue(MedBagRangeLeftProperty); }
            set { SetValue(MedBagRangeLeftProperty, value); }
        }

        public static readonly DependencyProperty MedBagRangeLeftProperty =
            DependencyProperty.Register("MedBagRangeLeft",
                typeof(double),
                typeof(RdlLocationControl),
                new PropertyMetadata(0.0));

        public double MedBagRangeRight
        {
            get { return (double)GetValue(MedBagRangeRightProperty); }
            set { SetValue(MedBagRangeRightProperty, value); }
        }

        public static readonly DependencyProperty MedBagRangeRightProperty =
            DependencyProperty.Register("MedBagRangeRight",
                typeof(double),
                typeof(RdlLocationControl),
                new PropertyMetadata(0.0));

        public RdlLocationControl(int locId)
        {
            InitializeComponent();
            id = locId;
        }

        public RdlLocationControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}