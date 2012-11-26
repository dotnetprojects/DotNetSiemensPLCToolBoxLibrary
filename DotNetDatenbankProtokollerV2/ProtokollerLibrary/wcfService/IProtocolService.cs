using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace DotNetSimaticDatabaseProtokollerLibrary.wcfService
{
    [ServiceContract()]
    public interface IProtocolService
    {
#if !SILVERLIGHT
        [OperationContract()]
        IEnumerable<string> GetStorageNames();

        [OperationContract()]
        IEnumerable<ProtocolRow> GetProtocolData(string storageName, int startRow, int maxRows, string filter, DateTime? FromDate, DateTime? ToDate);
#else
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStorageNames(AsyncCallback callback, object state);

        IEnumerable<string> EndGetStorageNames(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetProtocolData(string storageName, int startRow, int maxRows, string filter, DateTime? FromDate, DateTime? ToDate, AsyncCallback callback, object state);
        IEnumerable<ProtocolRow> EndGetProtocolData(IAsyncResult result);
#endif
    }
}
