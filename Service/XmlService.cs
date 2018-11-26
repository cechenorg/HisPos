using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.RDLC;

namespace His_Pos.Service
{
    public static class XmlService
    {
        public static T Deserialize<T>(string s)
        {
            XmlDocument xdoc = new XmlDocument();

            try
            {
                xdoc.LoadXml(s);
                XmlNodeReader reader = new XmlNodeReader(xdoc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                object obj = ser.Deserialize(reader);

                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }

        public static string SerializeObject<T>(this T value)
        {
            if (value == null)
                return string.Empty;
            try
            {
                var xmlserializer = new XmlSerializer(value.GetType());
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    var document = XDocument.Parse(ReportService.PrettyXml(stringWriter));
                    document.Descendants()
                        .Where(e => e.IsEmpty || string.IsNullOrWhiteSpace(e.Value))
                        .Remove();
                    document.Root?.RemoveAttributes();
                    return document.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Serialize exception", ex);
            }
        }
    }
}
