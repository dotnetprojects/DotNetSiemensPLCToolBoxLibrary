using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    [ServiceContract]
    public interface IPolicyRetriever
    {
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
        Stream GetSilverlightPolicy();
        [OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
        Stream GetFlashPolicy();
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ProtokollerInstance : IPolicyRetriever
    {
        /// <summary>
        /// Create a UTF-8 encoded Stream based on a string
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private Stream StringToStream(string result)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(result));
        }

        /// <summary>
        /// Fetch policy file for Silverlight access
        /// </summary>
        /// <returns>Silverlight policy access xml</returns>
        public Stream GetSilverlightPolicy()
        {
            string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
    <cross-domain-access>
        <policy>
            <allow-from http-request-headers=""*"">
                <domain uri=""*""/>
            </allow-from>
            <grant-to>
                <resource path=""/"" include-subpaths=""true""/>
            </grant-to>
        </policy>
    </cross-domain-access>
</access-policy>";
            return StringToStream(result);
        }

        /// <summary>
        /// Fetch policy file for Flash access
        /// </summary>
        /// <returns>Flash policy access xml</returns>
        public Stream GetFlashPolicy()
        {
            string result = @"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
    <allow-access-from domain=""*"" />
</cross-domain-policy>";
            return StringToStream(result);
        }
    }
}
