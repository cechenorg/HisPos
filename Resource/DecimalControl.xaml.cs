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

namespace His_Pos.Resource
{
    /// <summary>
    /// DecimalControl.xaml 的互動邏輯
    /// </summary>
    public partial class DecimalControl : UserControl
    {
        public double Decimal
        {
            get { return (double)GetValue(DecimalProperty); }
            set { SetValue(DecimalProperty, value); }
        }

        public static readonly DependencyProperty DecimalProperty =
            DependencyProperty.Register("Decimal",
                typeof(double),
                typeof(DecimalControl),
                new PropertyMetadata(0.0));

        public string BeforeDot
        {
            get
            {
                return Decimal.ToString("###0");
            }
        }

        public string AfterDot
        {
            get
            {
                string tempDec = Decimal.ToString("##.00");

                int dotIndex = tempDec.IndexOf('.') + 1;

                return tempDec.Substring(dotIndex);
            }
        }

        public DecimalControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
