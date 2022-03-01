using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
                return String.Empty;
            try
            {
                var xmlserializer = new XmlSerializer(value.GetType());
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    var document = XDocument.Parse(PrettyXml(stringWriter));
                    document.Descendants()
                        .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
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

        public static XDocument SerializeObjectToXDocument<T>(this T value)
        {
            try
            {
                var xmlserializer = new XmlSerializer(value.GetType());
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    var document = XDocument.Parse(PrettyXml(stringWriter));
                    document.Descendants()
                        .Where(e => e.IsEmpty || string.IsNullOrWhiteSpace(e.Value))
                        .Remove();
                    document.Root?.RemoveAttributes();
                    return document;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Serialize exception", ex);
            }
        }

        public static string SerializeDailyUploadObject<T>(this T value)
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
                    var document = XDocument.Parse(PrettyXml(stringWriter));
                    document.Root?.RemoveAttributes();
                    return document.ToString().Replace("<A18/>", "<A18></A18>").Replace("<A18 />", "<A18></A18>");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Serialize exception", ex);
            }
        }

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static string PrettyXml(StringWriter writer)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(writer.ToString());

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }
            return stringBuilder.ToString();
        }
    }
}