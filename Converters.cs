﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class.Division;

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
            if (value is Product)
            {
                if (String.IsNullOrEmpty((value as Product).Id))
                    return false;
                return true;
            }
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
            ObservableCollection<Division> divisions = DivisionDb.GetData();
            string divisionName = "";
            foreach (var d in divisions)
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

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dateStr = ((string) value);
            string result = dateStr.Substring(0, 3) + "/" + dateStr.Substring(3, 2) + "/" + dateStr.Substring(5, 2);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}