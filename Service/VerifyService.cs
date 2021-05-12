using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace His_Pos.Service
{
    public class VerifyService
    {
        public static bool VerifyIDNumber(string id)
        {
            int[] verifyNums = new[] { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            Dictionary<char, string> verifyKeyDictionary = new Dictionary<char, string>()
                {
                    { 'A', "10" }, { 'B', "11" }, { 'C', "12" }, { 'D', "13" }, { 'E', "14" }, { 'F', "15" }, { 'G', "16" }, { 'H', "17" }, { 'I', "34" },
                    { 'J', "18" }, { 'K', "19" }, { 'L', "20" }, { 'M', "21" }, { 'N', "22" }, { 'O', "35" }, { 'P', "23" }, { 'Q', "24" }, { 'R', "25" },
                    { 'S', "26" }, { 'T', "27" }, { 'U', "28" }, { 'V', "29" }, { 'W', "32" }, { 'X', "30" }, { 'Y', "31" }, { 'Z', "33" }
                };

            Regex taiwanRegex = new Regex("[A-Z][12][0-9]{8}");
            Regex foreignRegex = new Regex("[A-Z][ABCD][0-9]{8}");
            Regex newForeignRegex = new Regex("[A-Z][89][0-9]{8}");
            Match taiwanMatch = taiwanRegex.Match(id);
            Match foreignMatch = foreignRegex.Match(id);
            Match newForeignMatch = newForeignRegex.Match(id);

            if (taiwanMatch.Success)
            {
                string verifyString = verifyKeyDictionary[id[0]] + id.Substring(1, 9);
                int sum = 0;

                for (int x = 0; x < verifyNums.Length; x++)
                {
                    sum += (verifyString[x] - 48) * verifyNums[x];
                }

                int sumCheck = (10 - sum % 10) % 10;
                int checkSum = verifyString[10] - 48;

                if (sumCheck == checkSum)
                    return true;
            }
            else if (foreignMatch.Success)
            {
                string verifyString = verifyKeyDictionary[id[0]] + verifyKeyDictionary[id[1]].Substring(1, 1) + id.Substring(2, 8);
                int sum = 0;

                for (int x = 0; x < verifyNums.Length; x++)
                {
                    sum += (verifyString[x] - 48) * verifyNums[x];
                }

                int sumCheck = (10 - sum % 10) % 10;
                int checkSum = verifyString[10] - 48;

                if (sumCheck == checkSum)
                    return true;
            }
            else if (newForeignMatch.Success)
            {
                return true;
            }

            return false;
        }
    }
}