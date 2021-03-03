using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.ProductType
{
    public class ProductType : ObservableObject
    {
        #region ----- Define Variables -----

        private string name;

        public int ID { get; set; }
        public int ParentID { get; set; }

        public string Name
        {
            get => name;
            set { Set(() => Name, ref name, value); }
        }

        #endregion ----- Define Variables -----

        public ProductType(DataRow row)
        {
            ID = row.Field<int>("Type_ID");
            ParentID = row.Field<int>("Type_Parent");
            Name = row.Field<string>("Type_Name");
        }
    }
}