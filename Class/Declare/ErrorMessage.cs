using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace His_Pos.Class.Declare
{
    [XmlRoot(ElementName = "Error")]
    public class Error
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Content")]
        public string Content { get; set; }
    }

    [XmlRoot(ElementName = "ErrorList")]
    public class ErrorList
    {
        [XmlElement(ElementName = "PrescriptionId")]
        public string PrescriptionId { get; set; }
        [XmlElement(ElementName = "Error")]
        public List<Error> Error { get; set; }
    }

    [XmlRoot(ElementName = "ErrorPrescriptions")]
    public class ErrorPrescriptions
    {
        [XmlElement(ElementName = "ErrorList")]
        public List<ErrorList> ErrorList { get; set; }
    }
}
