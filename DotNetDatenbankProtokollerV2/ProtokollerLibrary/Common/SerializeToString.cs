using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.Common
{
    public static class SerializeToString<T>
    {
        public static string Serialize(T obj)
        {
            // XML-Serialisieren in String
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            // Serialisieren in MemoryStream
            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create(ms, settings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, obj, namespaces);

            // Stream in String umwandeln 
            StreamReader r = new StreamReader(ms);
            r.BaseStream.Seek(0, SeekOrigin.Begin);

            return r.ReadToEnd();
        }

        public static string DataContractSerialize(T obj)
        {

            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());

            // Serialisieren in MemoryStream
            MemoryStream ms = new MemoryStream();


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(ms, settings);

            serializer.WriteObject(writer, obj);
            writer.Flush();
            // Stream in String umwandeln 
            StreamReader r = new StreamReader(ms);
            r.BaseStream.Seek(0, SeekOrigin.Begin);

            return r.ReadToEnd();
        }

        public static T DeSerialize(string txt)
        {
            T retVal = default(T);
            if (txt == null)
                return retVal;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(txt);
                XmlTextReader xmlReader = new XmlTextReader(stringReader);
                retVal = (T)ser.Deserialize(xmlReader);
                xmlReader.Close();
                stringReader.Close();
            }
            catch (Exception)
            {
            }
            return retVal;
        }

        public static T DataContractDeSerialize(string txt)
        {
            if (txt == null)
                return default(T);
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            byte[] byteArray = Encoding.ASCII.GetBytes(txt);
            MemoryStream ms = new MemoryStream(byteArray); 

            //MemoryStream ms = new MemoryStream();
            //StreamWriter r = new StreamWriter(ms);
            //r.Write(txt);
            //r.BaseStream.Seek(0, SeekOrigin.Begin);
            T retVal = (T)serializer.ReadObject(ms);
            //r.Close();
            ms.Close();
            return retVal;
        }
    }
}
