using System.Data;
using System.Linq;
using System.Xml;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.Declare;
using His_Pos.Service;

namespace His_Pos.Class.Division
{
    public class Division : Selection
    {
        public Division()
        {
        }
        public Division(XmlNode xml) {
            Id = xml.SelectSingleNode("d13") == null ? null : xml.SelectSingleNode("d13").InnerText;
        }
       
        public Division(DataRow dataRow)
        {
            Id = dataRow["HISDIV_ID"].ToString();
            Name = dataRow["HISDIV_NAME"].ToString();
            FullName = dataRow["HISDIV_FULLNAME"].ToString();
        }

        public Division(string divisionId = "")
        {
            if (!string.IsNullOrEmpty(divisionId))
            {
                var tmpDivision = ViewModelMainWindow.Divisions.SingleOrDefault(d => d.Id.Equals(divisionId)).DeepCloneViaJson();
                Id = tmpDivision.Id;
                Name = tmpDivision.Name;
                FullName = tmpDivision.FullName;
            }
            else
            {
                Id = string.Empty;
                Name = string.Empty;
                FullName = string.Empty;
            }
        }
    }
}
