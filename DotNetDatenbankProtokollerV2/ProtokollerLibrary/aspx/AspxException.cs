//---------------------------------------------------------------------
//  This file is part of the Microsoft .NET Framework SDK Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//This source code is intended only as a supplement to Microsoft
//Development Tools and/or on-line documentation.  See these other
//materials for detailed information regarding Microsoft code samples.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace DotNetSimaticDatabaseProtokollerLibrary.aspx
{
    [Serializable]
    public class AspxException : Exception
    {
        public AspxException() : base()
        {
        }

        public AspxException(String message, Exception e) : base(message, e)
        {
        }

        public AspxException(String message) : base(message)
        {

        }

        protected AspxException(SerializationInfo serializationInfo, StreamingContext context):base(serializationInfo,context)
        {
        }
    }
}
