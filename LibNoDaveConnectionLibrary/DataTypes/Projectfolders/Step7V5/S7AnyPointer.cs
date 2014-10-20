using System;
using System.Collections.Generic;
using System.Globalization;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7AnyPointer
    {
        private static readonly List<S7DataRowType> suportedDataTypes = new List<S7DataRowType>
                                                                   {
                                                                       S7DataRowType.NIL, S7DataRowType.BOOL, S7DataRowType.BYTE, S7DataRowType.CHAR,
                                                                       S7DataRowType.WORD, S7DataRowType.INT, S7DataRowType.DWORD,S7DataRowType.DINT,
                                                                       S7DataRowType.REAL, S7DataRowType.DATE, S7DataRowType.TIME_OF_DAY,S7DataRowType.TIME,
                                                                       S7DataRowType.S5TIME,S7DataRowType.DATE_AND_TIME, S7DataRowType.STRING,
                                                                   };

        public S7DataRowType Type { get; set; }
        public int RepetitionFactor { get; set; }
        public string Datablock { get; set; }
        public string AddressPointer { get; set; }

        public S7AnyPointer(string codedDataType, string repetitionFactor, string dbNumber, string pointer)
        {
            var dataType = codedDataType.Replace("W#16#10", ""); //Remove the step 7 header
            int dataTypeValue;
            if (!Int32.TryParse(dataType, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out dataTypeValue)) Type = S7DataRowType.UNKNOWN;
            else
            {
                var drt = (S7DataRowType)dataTypeValue;
                Type = suportedDataTypes.Contains(drt) ? drt : S7DataRowType.UNKNOWN;
            }
            RepetitionFactor = 0;
            int repFact;
            if (int.TryParse(repetitionFactor, out repFact)) RepetitionFactor = repFact;
            if (RepetitionFactor < 0)
            {
                RepetitionFactor = BitConverter.ToUInt16(BitConverter.GetBytes(RepetitionFactor), 0);
            }
            int dbNr;
            if (int.TryParse(dbNumber, out dbNr))
            {
                if (dbNr > 0)
                    Datablock = "DB" + dbNr;
            }
            AddressPointer = pointer.Replace("P#", "");
            //if(CodedDataTypes.ContainsKey(dataType))
            //    Type = CodedDataTypes.ContainsKey(dataType) ? CodedDataTypes[dataType]: "UNKNOWN_TYPE"
        }

        public override string ToString()
        {
            var ptr = "P#";
            if (!string.IsNullOrEmpty(Datablock)) ptr += Datablock + ".";
            ptr += AddressPointer + " ";
            if (RepetitionFactor > 0)
            {
                ptr += Type + " ";
                ptr += RepetitionFactor;
            }
            return ptr;
        }
    }
}