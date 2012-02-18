using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Kopplungstester
{
    public static class DataSetConverter
    {
        public static string ToString(DataSet dsData)
        {
            MemoryStream objStream = new MemoryStream();
            dsData.WriteXml(objStream);

            XmlTextWriter objXmlWriter = new XmlTextWriter(objStream, Encoding.UTF8);
            objStream = (MemoryStream)objXmlWriter.BaseStream;

            UTF8Encoding objEncoding = new UTF8Encoding();

            return objEncoding.GetString(objStream.ToArray());
        }

        public static DataSet ToDataSet(string strXmlData)
        {
            StringReader objReader = new StringReader(strXmlData);
            DataSet dsData = new DataSet();
            dsData.ReadXml(objReader);

            return dsData;
        }

        public static System.Data.DataSet DatagridviewToDataset(DataGridView dgv)
        {
            System.Data.DataSet ds = new DataSet();

            ds.Tables.Add("Main");

            System.Data.DataColumn col;

            foreach (DataGridViewColumn dgvCol in dgv.Columns)
            {
                col = new System.Data.DataColumn(dgvCol.Name);
                ds.Tables["Main"].Columns.Add(col);
            }
            System.Data.DataRow row;
            int colcount = dgv.Columns.Count - 1;

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                row = ds.Tables["Main"].Rows.Add();

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    row[column.Index] = dgv.Rows[i].Cells[column.Index].Value;
                }
            }
            return ds;
        }
    }
}
