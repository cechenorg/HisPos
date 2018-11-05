using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.H6_DECLAREFILE.Export;
using His_Pos.Service;

namespace His_Pos
{
    public class TextBoxDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateValue;
            if (DateTime.TryParse(value.ToString(), out dateValue))
            {
                return dateValue.AddYears(-1911).ToString("yyyy/MM/dd").Substring(1, 9);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateValue;
            var tempvalue = value;
            if (tempvalue.ToString().Length >= 7)
            {
                switch (tempvalue.ToString().Substring(0, 1))
                {
                    case "1":
                        if (!tempvalue.ToString().Contains("/"))
                            tempvalue = Int32.Parse(tempvalue.ToString()) + 19110000;
                        break;

                    case "2":
                        break;

                    default:
                        return value;
                }
                tempvalue = tempvalue.ToString().Insert(6, "/");
                tempvalue = tempvalue.ToString().Insert(4, "/");
            }
            if (tempvalue.ToString().Length == 10 && DateTime.TryParse(tempvalue.ToString(), out dateValue))
            {
                return tempvalue;
            }
            return value;
        }
    }

    public class AutoCompleteIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value.ToString().Equals("")) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class TaiwanCalenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(value.ToString())) return result;
            result = (int.Parse(value.ToString().Split('/')[0]) - 1911) + "/" + value.ToString().Split('/')[1] + "/" +
                     value.ToString().Split('/')[2].Substring(0,2);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalender(value.ConvertTo<DateTime>(), true)
                : string.Empty;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString().Contains(" ") || value.ToString().Length != 9) return string.Empty;
            var year = int.Parse(value.ToString().Substring(0, 3)) + 1911;
            var month = int.Parse(value.ToString().Substring(4, 2));
            var date = int.Parse(value.ToString().Substring(7, 2));
            var result = new DateTime(year, month, date);
            return result;
        }
    }

    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Contains("..") || value.ToString() == string.Empty)
                return value.ToString().Replace("..", ".");
            else if (value.ToString().Contains("0.000"))
                return double.Parse(value.ToString());
            else if (value.ToString().Length > 5)
                return value.ToString().Substring(0, 5);
            else if (value.ToString().Substring(value.ToString().Length - 1, 1) == ".")
                return value;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class LastRowIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Product product)) return false;
            return !string.IsNullOrEmpty(product.Id);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NullTextBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? string.Empty : String.Format(culture, "{0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(String.Format(culture, "{0}", value)) ? null : value;
        }
    }

    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return new SolidColorBrush(Colors.DarkSeaGreen); ;
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DivisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string divisionName = "";
            foreach (var d in ExportView.Instance.DivisionCollection)
            {
                if (d.Id.Equals(value))
                    divisionName = d.Name;
            }
            return divisionName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReleaseInsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string divisionName = "";
            foreach (var d in ExportView.Instance.HospitalCollection)
            {
                if (d.Id.Equals(value))
                    divisionName = d.Name;
            }
            return divisionName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateValidationRule : ValidationRule
    {
        private const string InvalidInput = "日期格式錯誤";

        // Implementing the abstract method in the Validation Rule class
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string) value)) return new ValidationResult(true, null);
            if (!value.ToString().Contains(" ") && value.ToString().Length == 9)
            {
                var year = int.Parse(value.ToString().Substring(0, 3)) + 1911;
                var month = int.Parse(value.ToString().Substring(4, 2));
                var date = int.Parse(value.ToString().Substring(7, 2));
                var dateStr = year + "/" + month + "/" + date;
                // Validates weather Non numeric values are entered as the Age
                if (!DateTime.TryParse(dateStr, out _))
                {
                    return new ValidationResult(false, InvalidInput);
                }
            }
            else if(value.ToString().Contains(" ") || value.ToString().Length != 9)
            {
                return new ValidationResult(false, InvalidInput);
            }
            return new ValidationResult(true, null);
        }
    }
    
    public class EnumBooleanConverter : IValueConverter
    { enum RadioOptions  { Option1 = 0, Option2 = 1, Option3 = 2, Option4 = 3}
    public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
                return Enum.Parse(typeof(RadioOptions), targetValue);
            return null;
        }
    }
}