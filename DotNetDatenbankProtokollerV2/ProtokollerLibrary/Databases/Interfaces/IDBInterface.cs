using System;
using System.Collections.Generic;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces
{
    public interface IDBInterface : IDisposable
    {
        /// <summary>
        /// Connects to the Database
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        void Connect_To_Database(StorageConfig config); 

        /*/// <summary>
        /// Creates the Table and the Fields (or modifys them)
        /// </summary>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig);*/

        void Initiate(DatasetConfig dsConfig);

        /// <summary>
        /// Writes the Values to the database
        /// </summary>
        /// <param name="values"></param>
        void Write(IEnumerable<object> values);
        
        /// <summary>
        /// Closes Connection to the Database
        /// </summary>
        void Close();

        event ThreadExceptionEventHandler ThreadExceptionOccured;
    }    
}
