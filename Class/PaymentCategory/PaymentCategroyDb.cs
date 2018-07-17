using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.PaymentCategory
{
    public static class PaymentCategroyDb
    {

        public static ObservableCollection<PaymentCategory> GetData()
        {
            var paymentCategoryCollection = new ObservableCollection<PaymentCategory>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var paymentCategories = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetPaymentCategoriesData]", dbConnection);
            foreach (DataRow paymentCategory in paymentCategories.Rows)
            {
                var d = new PaymentCategory(paymentCategory);
                paymentCategoryCollection.Add(d);
            }
            return paymentCategoryCollection;
        }
        /*
         *回傳對應給付類別之id + name string
         */
        public static string GetPaymentCategory(string tag)
        {
            var result = string.Empty;
            foreach (var payment in GetData())
            {
                if (payment.Id == tag)
                {
                    result = payment.FullName;
                }
            }
            return result;
        }
    }
}
