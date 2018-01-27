using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using His_Pos.Class;
using ImeLib;

namespace His_Pos
{
    
    class Function
    {

        //用途:將中文字轉成注音符號再轉成英文字母
        //輸入:中文字串
        //輸出:英文字串
        public string ChangeNameToEnglish( string txtInput)
        {
            string[] result;
            string resultOutput = string.Empty;
            using (MsImeFacade ime = new MsImeFacade(ImeClass.Taiwan))
            {

                 result = ime.GetBopomofo(txtInput);
                for (int i = 0; i < result.Length; i++)
                {
                    resultOutput += result[i].Substring(0, 1);
                }
            }
            return resultOutput;
        }//ChangeNameToEnglish

        //用途:取得Enum Description value
        //輸入:Enum值
        //輸出:Description值
        public string GetEnumDescription(string type, string value)
        {
            var reply = string.Empty;
            if (type == "ErrorCode")
            {
                var evalue = (ErrorCode)Enum.Parse(typeof(ErrorCode), value);
                var fi = evalue.GetType().GetField(evalue.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                    reply = attributes[0].Description;
                else
                    reply = value;
            }
            if (type == "Alphabat")
            {
                var evalue = (Alphabat)Enum.Parse(typeof(Alphabat), value);
                var fi = evalue.GetType().GetField(evalue.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                    reply = attributes[0].Description;
                else
                    reply = value;
            }
            return reply;
        } //GetEnumDescription

        public string dateFormatConvert(DateTime selectedDate)
        {
            string month;
            if (selectedDate.Month < 10)
                month = "0" + selectedDate.Month;
            else
            {
                month = selectedDate.Month.ToString();
            }
            string day;
            if (selectedDate.Day < 10)
                day = "0" + selectedDate.Day;
            else
            {
                day = selectedDate.Day.ToString();
            }
            
            string date = (int.Parse(selectedDate.Year.ToString()) - 1911) + month + day;
            return date;
        }
        public string BirthdayFormatConverter(string birthday)
        {
            var year = birthday.Substring(0, 3).StartsWith("0") ? birthday.Substring(1, 2) : birthday.Substring(0, 3);
            return year + "/" + birthday.Substring(3, 2) + "/" + birthday.Substring(5, 2);
        }

        public void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
        {
            // Checks should be made, but preferably one time before calling.
            // And here it is assumed that the programmer has taken into
            // account all of these conditions and checks are not needed.
            //if ((parent == null) || (childName == null) || (<Type T is not inheritable from FrameworkElement>))
            //{
            //    return;
            //}
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                // Get the child
                var child = VisualTreeHelper.GetChild(parent, i);

                // Compare on conformity the type

                // Not compare - go next
                if (!(child is T childTest))
                {
                    // Go the deep
                    FindChildGroup(child, childName, ref list);
                }
                else
                {
                    // If match, then check the name of the item
                    FrameworkElement childElement = childTest as FrameworkElement;

                    Debug.Assert(childElement != null, nameof(childElement) + " != null");
                    if (childElement.Name == childName)
                    {
                        // Found
                        list.Add(childTest);
                    }

                    // We are looking for further, perhaps there are
                    // children with the same name
                    FindChildGroup(child, childName, ref list);
                }
            }
        }
    }
}
