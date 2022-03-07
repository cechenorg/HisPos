using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InfraStructure.SQLService.SQLServer.StoreOrder;

namespace InfraStructure.Test
{
    [TestClass]
    public class StoreOrderDBServiceTest
    {
        private StoreOrderDBService storeOrderDbService;
        public StoreOrderDBServiceTest()
        {
            storeOrderDbService = new StoreOrderDBService(SQLString.sqlDevelopstring,string.Empty);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///取得或設定提供目前測試回合
        ///相關資訊與功能的測試內容。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void Test_Set_InsertStoreOrderCommonMedicine()
        {
            storeOrderDbService.Set_InsertStoreOrderCommonMedicine(0);
        }
    }
}
