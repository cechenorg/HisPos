using System;
using System.Windows;
using System.Windows.Data;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    public class GenderConverter : ResourceDictionary,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var Gender = (bool)value;
            var result = Gender ? "¨k" : "¤k";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? result = null;
            var Gender = value.ToString();
            if (Gender.Equals("¨k"))
                result = true;
            return result;
        }
    }
}