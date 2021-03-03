using System.Data;

namespace His_Pos.NewClass.Product.CommonProduct
{
    public static class CommonProductDb
    {
        internal static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[IndexCommonProduct]");
        }
    }
}