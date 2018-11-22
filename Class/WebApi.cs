using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
   public static class WebApi
    {
        internal static ObservableCollection<CooperativeClinic> GetXmlByMedicalNum(string pharmcyMedicalNum, string cusIdnum = null)
        {
            Dictionary<string, string> keyValues;
                keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"cusIdnum",cusIdnum }
                };
            ObservableCollection<CooperativeClinic> cooperativeClinics = new ObservableCollection<CooperativeClinic>();
             HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://localhost:60145/api/GetXmlByMedicalNum", keyValues);
            foreach (XmlDocument xmlDocument in table) {
                cooperativeClinics.Add(new CooperativeClinic(xmlDocument));
            }
            return cooperativeClinics;
        }
    }
}
