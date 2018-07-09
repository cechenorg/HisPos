using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace His_Pos.Class.WorkSchedule
{
    public class UserIconData
    {
        public UserIconData(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            BackBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(dataRow["EMP_COLOR"].ToString()));
            IsMed = Boolean.Parse(dataRow["ISMED"].ToString());
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Brush BackBrush { get; set; }
        public bool IsMed { get; }
    }
}
