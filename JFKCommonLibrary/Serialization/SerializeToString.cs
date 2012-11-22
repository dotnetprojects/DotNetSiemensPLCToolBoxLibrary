using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JFKCommonLibrary.Serialization
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

        /*public static string SharpSerializerSerialize(T obj)
        {
            var settings = new SharpSerializerXmlSettings(); // for xml mode

            // configure the type serialization
            settings.IncludeAssemblyVersionInTypeName = false;
            settings.IncludeCultureInTypeName = false;
            settings.IncludePublicKeyTokenInTypeName = false;            

            // XML-Serialisieren in String
            var serializer = new SharpSerializer();

            // Serialisieren in MemoryStream
            MemoryStream ms = new MemoryStream();

            serializer.Serialize(obj, ms);
            
            // Stream in String umwandeln 
            StreamReader r = new StreamReader(ms);
            r.BaseStream.Seek(0, SeekOrigin.Begin);

            return r.ReadToEnd();
        }*/

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

        /*public static T SharpSerializerDeSerialize(string txt)
        {
            T retVal = default(T);
            if (txt == null)
                return retVal;
            try
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(txt);
                writer.Flush();

                var serializer = new SharpSerializer();
                retVal = (T)serializer.Deserialize(stream);               
            }
            catch (Exception)
            {
            }
            return retVal;
        }*/

        public static T DeSerialize(string txt)
        {
            T retVal = default(T);
            if (txt == null) return retVal;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(txt);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            retVal = (T)ser.Deserialize(xmlReader);
            xmlReader.Close();

            stringReader.Close();
            return retVal;
        }

        public static T DataContractDeSerialize(string txt)
        {
            if (txt == null)
                return default(T);
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            byte[] byteArray = Encoding.ASCII.GetBytes(txt);
            MemoryStream ms = new MemoryStream(byteArray); 

            T retVal = (T)serializer.ReadObject(ms);
            ms.Close();
            
            return retVal;
        }
    }
}
