using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.CommonProduct
{
   public static class CommonProductDb
    {
        internal static DataTable GetData( )
        { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[IndexCommonProduct]");
        }   
    }
}
