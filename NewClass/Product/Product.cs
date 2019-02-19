using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product
{
    public class Product : ObservableObject
    {
        public Product()
        {

        }

        public Product(DataRow row)
        {
            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            Common = row.Field<bool>("Pro_IsCommon");
        }

        public Product(ProductStruct p)
        {
            ID = p.ID;
            ChineseName = p.ChineseName;
            EnglishName = p.EnglishName;
        }

        #region ----- Define Variables -----

        private string id;
        public string ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }

        public string FullName
        {
            get
            {
                if(!string.IsNullOrEmpty(EnglishName))
                    return (EnglishName.Contains(" ")? EnglishName.Substring(0, EnglishName.IndexOf(" ")) : EnglishName) + ChineseName;
                return !string.IsNullOrEmpty(ChineseName) ? ChineseName : string.Empty;
            }
        }
        private bool common;//常備品項
        public bool Common
        {
            get => common;
            set
            {
                if (common != value)
                {
                    Set(() => Common, ref common, value);
                }
            }
        }
        #endregion

        #region ----- Define Functions -----


        #endregion
    }
}
