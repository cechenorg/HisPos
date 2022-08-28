using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Extention
{
    internal static class StringFormatExtention
    {
        public static string ToPatientCellPhone(this string input)
        {
            if (input.Length == 10)
            {
                return $"{input.Substring(0, 4)}-{input.Substring(4, 3)}-{input.Substring(7, 3)}";
            }

            return input;
        }


        public static string ToPatientTel(this string input)
        {
            switch (input.Length)
            {

                case 7:
                    return $"{input.Substring(0, 3)}-{input.Substring(3, 4)}";
                case 9:
                    return $"{input.Substring(0, 2)}-{input.Substring(2, 3)}-{input.Substring(5, 4)}";
                case 10:
                    return $"{input.Substring(0, 2)}-{input.Substring(2, 4)}-{input.Substring(6, 4)}";
            }

            return input;
        }

        public static string ToPatientNote(this string input)
        {

            var splitLines = input.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (splitLines.First().Length > 66)
            {
                splitLines[0] = splitLines.First().Substring(0, 66);
            }

            string result = default;

            for (int i = 0; i < splitLines.Length; i++)
            {
                result += splitLines[i];
                if (i < splitLines.Length - 1)
                    result += "\r\n";
            }

            return result;
        }
    }
}
