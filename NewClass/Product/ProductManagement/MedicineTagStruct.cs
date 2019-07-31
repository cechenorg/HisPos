using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class MedicineTagStruct
    {
        #region ----- Define Variables -----
        public string ID { get; set; }
        public string FirstLetter { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public bool IsControl { get; set; }
        public int? ControlLevel { get; set; }
        public string Ingredient { get; set; }
        public string Unit { get; set; }
        #endregion

        public MedicineTagStruct(string iD, string chineseName, string englishName, bool isControl, int? controlLevel, string ingredient)
        {
            var temp = Strings.StrConv(englishName, VbStrConv.Narrow, 0).Replace("\"", "");

            Regex regex = new Regex("[a-zA-Z-]*");
            Match match = regex.Match(temp);

            if (match.Groups.Count > 0)
            {
                FirstLetter = match.Groups[0].Value.Length > 1 ? match.Groups[0].Value.Substring(0, 1).ToUpper() : "";
                EnglishName = match.Groups[0].Value.Length > 1 ? match.Groups[0].Value.Substring(1) : "";
            }
            else
            {
                FirstLetter = "";
                EnglishName = "";
            }
            
            ID = iD;
            ChineseName = Strings.StrConv(chineseName, VbStrConv.Narrow, 0).Replace("\"", "");
            IsControl = isControl;
            ControlLevel = controlLevel;
            Ingredient = ingredient;
            Unit = "";

            regex = new Regex("[a-zA-Z()= ]* ([0-9.]+) ([a-zA-Z/]*)");

            var splitIngredient = ingredient.Split('+');

            foreach (var i in splitIngredient)
            {
                match = regex.Match(i.Trim());

                double amount = double.Parse(match.Groups[1].Value);

                if (!Unit.Equals("")) Unit += "/";
                Unit += amount.ToString("0.##");
            }

            Unit += " " + match.Groups[2].Value;
        }
    }
}
