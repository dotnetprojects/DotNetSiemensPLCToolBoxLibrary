using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class SerializeToString<T>
    {
        public static string Serialize(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create(ms, settings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, obj, namespaces);

            StreamReader r = new StreamReader(ms);
            r.BaseStream.Seek(0, SeekOrigin.Begin);

            return r.ReadToEnd();
        }
        
        public static T DeSerialize(string txt)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(txt);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            T retVal = (T)ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return retVal;
        }        
    }
}
