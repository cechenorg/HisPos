using System.Windows;
using System.Windows.Controls;

namespace His_Pos.GeneralCustomControl
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

        public string IntegerPart { get { return ((int)Decimal).ToString(); } }

        public string FloatPart
        {
            get
            {
                string doubleString = Decimal.ToString("0.00");
                return doubleString.Substring(doubleString.IndexOf(".") + 1, 2);
            }
        }

        public DecimalControl()
        {
            InitializeComponent();
            DecimalStack.DataContext = this;
        }
    }
}