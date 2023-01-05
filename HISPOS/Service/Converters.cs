using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Product;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace His_Pos.Service
{
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            return System.Convert.ToInt32(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || value.GetType().Name == "NamedObject") ? null : value;
        }
    }
    public class RowToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            return row.GetIndex() + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SentinelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || value.GetType().Name == "NamedObject") ? null : value;
        }

        /*public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null && string.Equals("{NewItemPlaceholder}", value.ToString(), StringComparison.Ordinal))
            {
                return null;
            }

            return value;
        }*/
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(value.ConvertTo<DateTime>())
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

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(value.ConvertTo<DateTime>()) + " " +
                  value.ConvertTo<DateTime>().ToString("HH:mm:ss")
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

    public class DateTimeToMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(value.ConvertTo<DateTime>()) + " " +
                  value.ConvertTo<DateTime>().ToString("HH:mm")
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

    public class NullableDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return "---/--/--";
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(value.ConvertTo<DateTime>())
                : "---/--/--";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? result = null;
            if (value == null) return result;
            var dateStr = value.ToString().Replace("/", "").Replace("-", "").Trim();
            int year, month, date;
            switch (dateStr.Length)
            {
                case 0:
                    result = null;
                    break;

                case 7:
                    year = int.Parse(dateStr.Substring(0, 3)) + 1911;
                    month = int.Parse(dateStr.Substring(3, 2));
                    date = int.Parse(dateStr.Substring(5, 2));
                    string dt = year.ToString() + "/" + month.ToString() + "/" + date.ToString();
                    if (DateTime.TryParse(dt, out DateTime res))
                    {
                        result = res;
                    }
                    else
                    {
                        result = null;
                    }
                    break;
            }

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
            if (value is Product) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class DateValidationRule : ValidationRule
    {
        private const string InvalidInput = "日期格式錯誤";

        // Implementing the abstract method in the Validation Rule class
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valueStr = value.ToString().Replace("/", "").Replace("-", "").Trim();
            if (string.IsNullOrEmpty(valueStr))
                value = "---/--/--";
            if (string.IsNullOrEmpty((string)value)) return new ValidationResult(true, null);
            if (((string)value).Equals("---/--/--")) return new ValidationResult(true, null);
            int year = 0, month = 0, date = 0;
            string checkStr = string.Empty;
            switch (valueStr.Length)
            {
                case 7:
                    year = int.Parse(valueStr.Substring(0, 3)) + 1911;
                    month = int.Parse(valueStr.Substring(3, 2));
                    date = int.Parse(valueStr.Substring(5, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
            }
            var validDate = DateTimeExtensions.ValidateDateTime(checkStr, "yyyyMMdd");
            if (validDate)
            {
                var dateStr = year + "/" + month + "/" + date;
                if (!DateTime.TryParse(dateStr, out _))
                    return new ValidationResult(false, InvalidInput);
            }
            else
                return new ValidationResult(false, InvalidInput);

            return new ValidationResult(true, null);
        }
    }

    public class EnumBooleanConverter : IValueConverter
    {
        private enum RadioOptions
        {
            Option1 = 0,
            Option2 = 1,
            Option3 = 2,
            Option4 = 3,
            Option5 = 4
        }

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

    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            var doubleType = (double)value;
            return doubleType.ToString(CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            return double.TryParse(strValue, out var resultDouble) ? resultDouble : 0;
        }
    }

    public class IntegerStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return string.Empty;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int _))
                return int.Parse(value.ToString());
            else
            {
                return null;
            }
        }
    }

    public class DateValidationRuleNoDate : ValidationRule
    {
        private const string InvalidInput = "日期格式錯誤";

        // Implementing the abstract method in the Validation Rule class
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value)) return new ValidationResult(true, null);

            if (value.ToString().Length == 6 && !value.ToString().Contains("-"))
            {
                var year = int.Parse(value.ToString().Substring(0, 3)) + 1911;
                var month = int.Parse(value.ToString().Substring(4, 2));
                var dateStr = year + "/" + month;
                // Validates weather Non numeric values are entered as the Age
                if (!DateTime.TryParse(dateStr, out _))
                {
                    return new ValidationResult(false, InvalidInput);
                }
            }
            else if (value.ToString().Contains("-") || value.ToString().Length != 9)
            {
                return new ValidationResult(false, InvalidInput);
            }

            return new ValidationResult(true, null);
        }
    }

    public class DateConverterNoDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalenderWithSplit(value.ConvertTo<DateTime>()).Substring(0, 6)
                : string.Empty;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? result = null;
            if (value == null || value.ToString().Length != 6) return result;
            var year = int.Parse(value.ToString().Substring(0, 3)) + 1911;
            var month = int.Parse(value.ToString().Substring(4, 2));
            result = new DateTime(year, month, 1);
            return result;
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            var intValue = (int)value;
            return intValue.ToString(CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            int? result = null;
            if (int.TryParse(strValue, out var resultInt))
                result = resultInt;
            return result;
        }
    }

    [ValueConversion(typeof(PharmacistSchedule), typeof(PharmacistSchedule))]
    public class PharmacistScheduleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var date = (DateTime)values[1];

            var pharmacistSchedule = new PharmacistSchedule();
            foreach (var item in (ObservableCollection<PharmacistScheduleItem>)values[0])
            {
                if (item.Date.Date == date)
                {
                    pharmacistSchedule.Add(item);
                }
            }

            return pharmacistSchedule;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class DayNameConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeFormatInfo dateTimeFormat = GetCurrentDateFormat();
            string[] shortestDayNames = dateTimeFormat.ShortestDayNames;
            string[] dayNames = dateTimeFormat.DayNames;

            for (int i = 0; i < shortestDayNames.Length; i++)
            {
                if (shortestDayNames[i] == value.ToString())
                {
                    return dayNames[i];
                }
            }

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }
            foreach (var cal in CultureInfo.CurrentCulture.OptionalCalendars)
            {
                if (cal is GregorianCalendar)
                {
                    var dtfi = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                    dtfi.Calendar = cal;
                    return dtfi;
                }
            }
            var dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
            dt.Calendar = new GregorianCalendar();
            return dt;
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Color.FromRgb(251, 60, 78));
            }
            return new SolidColorBrush(Color.FromArgb(255, 66, 64, 64));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiCommandParametersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Select(value => value.ToString()).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class PharmacistIsLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            return (bool)value ? "本店藥師" : "非本店藥師";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            if (parameter is null) return false;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualInverseParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            if (parameter is null) return false;
            return value.ToString() != parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}