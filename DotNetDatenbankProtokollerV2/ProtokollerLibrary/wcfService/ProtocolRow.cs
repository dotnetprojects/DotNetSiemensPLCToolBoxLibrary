using System;

namespace DotNetSimaticDatabaseProtokollerLibrary.wcfService
{
    public class ProtocolRow
    {
        #region Property Timestamp
        public DateTime Timestamp
        {
            get { return this._Timestamp; }
            set { this._Timestamp = value; }
        }
        private DateTime _Timestamp = DateTime.MinValue;
        #endregion
        
        public string Telegram { get; set; }
    }
}
