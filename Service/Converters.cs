using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting;

namespace His_Pos.Service
{
    public class SentinelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
        }
    }

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
                     value.ToString().Split('/')[2].Substring(0, 2);
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
            if (string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
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

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;
            var result = value.ConvertTo<DateTime>().Year > 1911
                ? DateTimeExtensions.ConvertToTaiwanCalender(value.ConvertTo<DateTime>(), true) + " " +
                  value.ConvertTo<DateTime>().ToLongTimeString()
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
                ? DateTimeExtensions.ConvertToTaiwanCalender(value.ConvertTo<DateTime>(), true)
                : "---/--/--";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? result = null;
            if (value == null) return result;
            var dateStr = value.ToString().Replace("/", "").Replace("-", "");
            int year, month, date;
            switch (dateStr.Length)
            {
                case 5:
                    year = int.Parse(dateStr.Substring(0, 1)) + 1911;
                    month = int.Parse(dateStr.Substring(1, 2));
                    date = int.Parse(dateStr.Substring(3, 2));
                    result = new DateTime(year, month, date);
                    break;
                case 6:
                    year = int.Parse(dateStr.Substring(0, 2)) + 1911;
                    month = int.Parse(dateStr.Substring(2, 2));
                    date = int.Parse(dateStr.Substring(4, 2));
                    result = new DateTime(year, month, date);
                    break;
                case 7:
                    year = int.Parse(dateStr.Substring(0, 3)) + 1911;
                    month = int.Parse(dateStr.Substring(3, 2));
                    date = int.Parse(dateStr.Substring(5, 2));
                    result = new DateTime(year, month, date);
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
            if ((bool) value)
            {
                {
                    return new SolidColorBrush(Colors.DarkSeaGreen);
                    ;
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
            foreach (var d in ViewModelMainWindow.Divisions)
            {
                if (d.ID.Equals(value))
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
            string institutionName = "";
            foreach (var d in ViewModelMainWindow.Institutions)
            {
                if (d.ID.Equals(value))
                    institutionName = d.Name;
            }

            return institutionName;
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
            var valueStr = value.ToString().Replace("/", "").Replace("-", "");
            bool validDate = false;
            int year = 0, month = 0, date = 0;
            string checkStr = string.Empty;
            DateTime result;
            switch (valueStr.Length)
            {
                case 5:
                    year = int.Parse(valueStr.Substring(0, 1)) + 1911;
                    month = int.Parse(valueStr.Substring(1, 2));
                    date = int.Parse(valueStr.Substring(3, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
                case 6:
                    year = int.Parse(valueStr.Substring(0, 2)) + 1911;
                    month = int.Parse(valueStr.Substring(2, 2));
                    date = int.Parse(valueStr.Substring(4, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
                case 7:
                    year = int.Parse(valueStr.Substring(0, 3)) + 1911;
                    month = int.Parse(valueStr.Substring(3, 2));
                    date = int.Parse(valueStr.Substring(5, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
            }

            validDate = DateTimeExtensions.ValidateDateTime(checkStr, "yyyyMMdd");
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

    public class NullDateValidationRule : ValidationRule
    {
        private const string InvalidInput = "日期格式錯誤";

        // Implementing the abstract method in the Validation Rule class
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string) value)) return new ValidationResult(true, null);
            if (((string) value).Equals("---/--/--")) return new ValidationResult(true, null);
            var valueStr = value.ToString().Replace("/", "").Replace("-", "");
            bool validDate = false;
            int year = 0, month = 0, date = 0;
            string checkStr = string.Empty;
            DateTime result;
            switch (valueStr.Length)
            {
                case 5:
                    year = int.Parse(valueStr.Substring(0, 1)) + 1911;
                    month = int.Parse(valueStr.Substring(1, 2));
                    date = int.Parse(valueStr.Substring(3, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
                case 6:
                    year = int.Parse(valueStr.Substring(0, 2)) + 1911;
                    month = int.Parse(valueStr.Substring(2, 2));
                    date = int.Parse(valueStr.Substring(4, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
                case 7:
                    year = int.Parse(valueStr.Substring(0, 3)) + 1911;
                    month = int.Parse(valueStr.Substring(3, 2));
                    date = int.Parse(valueStr.Substring(5, 2));
                    checkStr = year + month.ToString().PadLeft(2, '0') + date.ToString().PadLeft(2, '0');
                    break;
            }

            validDate = DateTimeExtensions.ValidateDateTime(checkStr, "yyyyMMdd");
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
        enum RadioOptions
        {
            Option1 = 0,
            Option2 = 1,
            Option3 = 2,
            Option4 = 3
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

            bool useValue = (bool) value;
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
            var doubleType = (double) value;
            return doubleType.ToString(CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            return double.TryParse(strValue, out var resultDouble) ? resultDouble : 0;
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class IsGetCardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isGetCard = (bool) value;
            return isGetCard ? "已過卡" : "未過卡";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsMedicineEditable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MedicineNHI || value is MedicineOTC || value is MedicineSpecialMaterial)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
            if (string.IsNullOrEmpty((string) value)) return new ValidationResult(true, null);

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
                ? DateTimeExtensions.ConvertToTaiwanCalender(value.ConvertTo<DateTime>(), true).Substring(0, 6)
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

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            var intValue = (int) value;
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

    [ValueConversion(typeof(ObservableCollection<Appointment>), typeof(ObservableCollection<Appointment>))]
    public class AppointmentsConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date = (DateTime)values[1];

            ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();
            foreach (Appointment appointment in (ObservableCollection<Appointment>)values[0])
            {
                if (appointment.Date.Date == date)
                {
                    appointments.Add(appointment);
                }
            }

            return appointments;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
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
            DateTimeFormatInfo dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
            dt.Calendar = new GregorianCalendar();
            return dt;
        }
    }
}