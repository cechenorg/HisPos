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

namespace His_Pos.GeneralCustomControl
{
    /// <summary>
    /// DateTextBox.xaml 的互動邏輯
    /// </summary>
    public partial class DateTextBox : UserControl
    {
        public DateTime Date
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime),
                typeof(DateTextBox), new PropertyMetadata(null));

        public DateTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
