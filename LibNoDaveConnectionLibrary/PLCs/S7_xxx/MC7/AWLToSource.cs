using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7
{
    public static class AWLToSource
    {
        /// <summary>
        /// This is a Helper Function wich is used by GetSourceBlock
        /// </summary>
        /// <param name="datrw"></param>
        /// <param name="leerz"></param>
        /// <returns></returns>
        public static string DataRowToSource(S7DataRow datrw, string leerz)
        {
            string retval = "";
            foreach (S7DataRow s7DataRow in datrw.Children)
            {
                string arr = "";
                string val = "";
                string cmt = "";
                if (s7DataRow.IsArray)
                {
                    arr += "ARRAY  [";
                    for (int i = 0; i < s7DataRow.ArrayStart.Count; i++)
                    {
                        if (i > 1) arr += ", ";
                        arr += s7DataRow.ArrayStart[i].ToString() + " .. " + s7DataRow.ArrayStop[i].ToString() + " ";
                    }
                    arr += "] OF ";
                }
                if (s7DataRow.DataType == S7DataRowType.STRING)
                {
                    if (s7DataRow.StartValue != null && s7DataRow.StartValue.ToString() != "")
                    {
                        val += " := " + s7DataRow.StartValue.ToString() + "";
                    }
                }
                else if (s7DataRow.StartValue != null)
                {
                    val += " := " + s7DataRow.StartValue.ToString();
                }

                if (!string.IsNullOrEmpty(s7DataRow.Comment))
                    cmt += "    //" + s7DataRow.Comment;
                if (s7DataRow.DataType == S7DataRowType.STRUCT)
                {
                    retval += leerz + s7DataRow.Name + " : " + arr + s7DataRow.DataType + cmt + Environment.NewLine;
                    retval += DataRowToSource(s7DataRow, leerz + " ");
                    retval += leerz + "END_STRUCT ;" + Environment.NewLine;
                }
                else
                {
                    retval += leerz + s7DataRow.Name + " : " + arr;
                    retval += s7DataRow.DataType;
                    if (s7DataRow.DataType == S7DataRowType.STRING)
                    {
                        retval += " [" + s7DataRow.StringSize + "]";
                    }
                    retval += (s7DataRow.DataTypeBlockNumber != 0 ? s7DataRow.DataTypeBlockNumber.ToString() : "") + " " + val + ";" + cmt + Environment.NewLine;
                }
            }
            return retval;
        }
    }
}
