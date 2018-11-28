using System;
using System.Data;
using System.Windows.Controls;
using His_Pos.Class.Pharmacy;

namespace His_Pos.SystemSettings.SettingControl
{
    /// <summary>
    /// MyPharmacyControl.xaml 的互動邏輯
    /// </summary>
    public partial class MyPharmacyControl : UserControl
    {
        #region ----- Define Struct -----
        public struct MyPharmacy
        {
            public MyPharmacy(DataRow dataRow)
            {
                Name = dataRow[""].ToString();
                Id = dataRow[""].ToString();
                Address = dataRow[""].ToString();
                Telephone = dataRow[""].ToString();
                IsReaderNew = Boolean.Parse(dataRow[""].ToString());
                ReaderCom = dataRow[""].ToString();
                VPN = dataRow[""].ToString();
            }

            public string Name { get; set; }
            public string Id { get; set; }
            public string Address { get; set; }
            public string Telephone { get; set; }
            public bool IsReaderNew { get; set; }
            public string ReaderCom { get; set; }
            public string VPN { get; set; }
        }
        #endregion

        #region ----- Define Variable -----
        public MyPharmacy myPharmacy { get; }
        #endregion

        public MyPharmacyControl()
        {
            InitializeComponent();

            myPharmacy = PharmacyDb.GetMyPharmacy();
            DataContext = myPharmacy;
        }
    }
}
