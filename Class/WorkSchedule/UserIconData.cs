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
        public UserIconData(DataRow dataRow, string hex)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            BackBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Brush BackBrush { get; set; }
    }
}
