using His_Pos.NewClass.Medicine;
using Microsoft.VisualBasic;
using System.Data;
using System.Text.RegularExpressions;

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

        #endregion ----- Define Variables -----

        private MedicineTagStruct(string iD, string chineseName, string englishName, bool isControl, int? controlLevel, string ingredient)
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
                if (string.IsNullOrEmpty(match.Groups[1].Value)) continue;
                double amount = double.Parse(match.Groups[1].Value);

                if (!Unit.Equals("")) Unit += "/";
                Unit += amount.ToString("0.##");
            }

            Unit += " " + match.Groups[2].Value;
        }

        internal static MedicineTagStruct GetDataByID(string productID)
        {
            DataTable dataTable = MedicineDb.GetTagDataByID(productID);

            if (dataTable is null || dataTable.Rows.Count == 0) return null;

            string name = dataTable.Rows[0].Field<string>("Pro_ChineseName");
            string engName = dataTable.Rows[0].Field<string>("Pro_EnglishName");
            bool isControl = dataTable.Rows[0].Field<bool>("IS_CONTROL");
            int? controlLevel = dataTable.Rows[0].Field<int?>("Med_Control");
            string ingredient = dataTable.Rows[0].Field<string>("Med_Ingredient");

            return new MedicineTagStruct(productID, name, engName, isControl, controlLevel, ingredient);
        }
    }
}