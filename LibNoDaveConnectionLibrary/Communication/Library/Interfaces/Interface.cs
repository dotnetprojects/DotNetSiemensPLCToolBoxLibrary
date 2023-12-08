﻿using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces
{
    public interface Interface : IDisposable
    {
        /// <summary>
        /// default 1,5s
        /// </summary>
        TimeSpan TimeOut { get; set; }

        Connection ConnectPlc(ConnectionConfig config);

        List<int> ListReachablePartners();

        void SendPdu(Pdu pdu, Connection connection);

        void ExchangePdu(Pdu pdu, Connection connection);

        void DisconnectPlc(Connection conn);
    }
}