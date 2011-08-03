using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.Common
{
    class XmlReferenceSerializer : XmlSerializer
    {
        protected override void Serialize(object o, XmlSerializationWriter writer)
        {
            base.Serialize(o, writer);
        }
    }
}
