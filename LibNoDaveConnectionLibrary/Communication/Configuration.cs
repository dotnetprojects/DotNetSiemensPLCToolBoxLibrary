/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    /// <summary>
    /// This is a Static class to show the Edit Window for the PLCConnectionConfiguration
    /// </summary>
    public class Configuration
    {
#if !IPHONE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultConnectionName"></param>
        /// <param name="fixedConnection">If this Parameter is used, a DefaultConnectionName has to be specified!</param>
        public static void ShowConfiguration(String defaultConnectionName, bool fixedConnection)
        {
            var myConnectionEditor = new ConnectionEditor { ConnectionNameFixed = fixedConnection, DefaultConnectionName = defaultConnectionName };
            myConnectionEditor.ShowDialog();
        }

        public static void ShowConfiguration()
        {
            var myConnectionEditor = new ConnectionEditor();
            myConnectionEditor.ShowDialog();
        }

        public static void ShowConfiguration(ICollection<PLCConnectionConfiguration> PLCConnectionConfigurationCollection)
        {
            var myConnectionEditor = new ConnectionEditor();
            myConnectionEditor.InternalConnectionList = PLCConnectionConfigurationCollection;
            myConnectionEditor.ShowDialog();
        }

        public static PLCConnectionConfiguration ShowConfiguration(PLCConnectionConfiguration myConfig)
        {
            if (myConfig == null)
                myConfig = new PLCConnectionConfiguration("PLC-Connection", LibNodaveConnectionConfigurationType.ObjectSavedConfiguration);

            var myConnectionEditor = new ConnectionEditor { ConnectionNameFixed = true, DefaultConnectionName = myConfig.ConnectionName, ObjectSavedConfiguration = true };
            myConnectionEditor.myConfig = myConfig;
            myConnectionEditor.ShowDialog();
            return myConfig;
        }
#endif
    }
}
