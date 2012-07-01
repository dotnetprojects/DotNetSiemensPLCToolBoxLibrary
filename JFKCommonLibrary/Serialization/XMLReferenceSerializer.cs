using System.Xml.Serialization;

namespace JFKCommonLibrary.Serialization
{
    class XmlReferenceSerializer : XmlSerializer
    {
        protected override void Serialize(object o, XmlSerializationWriter writer)
        {
            base.Serialize(o, writer);
        }
    }
}
