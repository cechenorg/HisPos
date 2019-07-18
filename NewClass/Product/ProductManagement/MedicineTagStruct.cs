using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class MedicineTagStruct
    {
        #region ----- Define Variables -----
        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public bool IsControl { get; set; }
        public int? ControlLevel { get; set; }
        public bool IsFrozen { get; set; }
        public string Ingredient { get; set; }
        #endregion

        public MedicineTagStruct(string iD, string chineseName, string englishName, bool isControl, int? controlLevel, bool isFrozen, string ingredient)
        {
            ID = iD;
            ChineseName = chineseName;
            EnglishName = englishName;
            IsControl = isControl;
            ControlLevel = controlLevel;
            IsFrozen = isFrozen;
            Ingredient = ingredient;
        }
    }
}
