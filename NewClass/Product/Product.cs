using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product
{
    public class Product : ObservableObject
    {
        public Product(DataRow dataRow)
        {

        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName
        {
            get { return ""; }
        }
        #endregion

        #region ----- Define Functions -----


        #endregion
    }
}
