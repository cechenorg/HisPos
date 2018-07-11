using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class UsageDb
    {
        internal static ObservableCollection<Usage> GetUsages()
        {
            ObservableCollection<Usage> collection = new ObservableCollection<Usage>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetUsagesData]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new Usage(row));
            }
            return collection;
        }
    }
}