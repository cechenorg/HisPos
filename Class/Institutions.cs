using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class Institutions:ISelection
    {
        public readonly ObservableCollection<string> InstitutionsCollection = new ObservableCollection<string>();
        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var institutionTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[INSTITUTION]",dbConnection);
            foreach (DataRow institution in institutionTable.Rows)
            {
                var i = new Institution(institution["INS_ID"].ToString(), institution["INS_NAME"].ToString());
                InstitutionsCollection.Add(i.Id + " " + i.Name);
            }
            InstitutionsCollection.Add("N" + "  " + "藥事居家照護/協助辦理門診戒菸計畫且直接交付指示用藥/提供「戒菸個案追蹤」/「戒菸衛教暨個案管理」");
        }
    }
}
