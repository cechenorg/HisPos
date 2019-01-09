using System;
using System.Windows;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2
{
    public class GenderConverter : ResourceDictionary,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var gender = (bool)value;
            var result = gender ? "¨k" : "¤k";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? result = null;
            var gender = value.ToString();
            if (gender.Equals("¨k"))
                result = true;
            return result;
        }
    }
}