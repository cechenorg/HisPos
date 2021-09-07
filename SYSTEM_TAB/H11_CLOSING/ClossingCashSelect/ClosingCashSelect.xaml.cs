using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.ClossingCashSelect
{
    /// <summary>
    /// ClosingCashSelect.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingCashSelect : UserControl
    {
        public ClosingCashSelect()
        {
            InitializeComponent();
            DataContext = new ClosingCashSelectViwModel();
        }
    }

    public class RowIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            DependencyObject item = (DependencyObject)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);

            return ic.ItemContainerGenerator.IndexFromContainer(item) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}