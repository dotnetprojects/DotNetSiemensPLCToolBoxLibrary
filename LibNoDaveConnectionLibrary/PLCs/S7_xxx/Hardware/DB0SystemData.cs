using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.Hardware
{
    public class DB0SystemData
    {
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public void ParseFromBytes(byte[] data)
        {
            Data = ParseDB0fromBytes(data);
        }

        public void ParseFromProject(Step7ProjectV5 proj, CPUFolder CpuToCheck)
        {
            ZipHelper _ziphelper = new ZipHelper(null);
            var _DirSeperator = '\\';
            DataTable dbfTbl = null;

            //There are two directories for the two PLC Types
            if (CpuToCheck.CpuType == DotNetSiemensPLCToolBoxLibrary.DataTypes.PLCType.Simatic300)
            {
                dbfTbl = DotNetSiemensPLCToolBoxLibrary.DBF.ParseDBF.ReadDBF(proj.ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk31ax" + _DirSeperator + "HATTRME1.DBF", _ziphelper, _DirSeperator);
            }
            else if (CpuToCheck.CpuType == DotNetSiemensPLCToolBoxLibrary.DataTypes.PLCType.Simatic400)
            {
                dbfTbl = DotNetSiemensPLCToolBoxLibrary.DBF.ParseDBF.ReadDBF(proj.ProjectFolder + "hOmSave7" + _DirSeperator + "s7hk41ax" + _DirSeperator + "HATTRME1.DBF", _ziphelper, _DirSeperator);
            }
            if (dbfTbl == null) return;

            //Now find the appropriate Attribute id
            foreach (DataRow row in dbfTbl.Rows)
            {
                if ((bool)row["DELETED_FLAG"]) continue; //we are not interested in deleted attributes

                //Unfortunately the ID field of the CPU folder is private, so we use reflection as a work around
                var PI = typeof(CPUFolder).GetField("ID", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                var ID = (int)PI.GetValue(CpuToCheck);
                if ((int)row["IDM"] != ID) continue;    //we want the one for our CPU

                if ((int)row["ATTRIIDM"] != 110717) continue; //110717 = DB0 System data, we only want system data

                if (row["MEMOARRAYM"] == DBNull.Value) continue;    //if there is no data yet, it means it was not compiled yet

                var memoarray = (byte[])row["MEMOARRAYM"];
                Data = ParseDB0fromBytes(memoarray);
            }
        }

        /// <summary>
        /// We could use that to simply load SDB0 from the plc and then parse the bytes directly
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Dictionary<string, object> ParseDB0fromBytes(byte[] data)
        {
            var dict = new Dictionary<string, object>();
            var s = new MemoryStream(data);

            //The data is stored in BigEndian format, so we have to use an converter Binary reader
            var r = new BigEndianBinaryReader(s);
            var Curr = 0;

            //SDB0 is composed of "Dataset-Lists" which each one containing an subset of the
            //hardware configurations data.
            //each dataset-list hast the following byte format, with the first Dataset starting at byte 0:
            //x+0   :Dataset Length. Basically indicates the start of the next data set list
            //x+1   :Dataset-list ID. This tells us how to parse the data, based on the XML file
            //x+n   :data to be parse based on XML file

            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var DataSetLength = r.ReadByte();   //x+0
                if (DataSetLength == 0) break;

                var DataSetID = r.ReadByte();       //x+1

                //this data must be parsed according to the XML configuration file
                //this file tells us which byte contains which information. The file can be found on the bottom of this file.
                //for example:
                //
                //<hwk:Dataset Name = "Startup" DSNr="1" DSLen="8">                                 //it is the data set with length 1, and it should be 8 bytes long
                //  <hwk:Par Name = "Test_on_Complete_Restart" PNr="1">                             //the name of the Parameter, and it is Parameter number 1. This parameter number has no relevance for us. it is not encoded in the binary data
                //      <hwk:datatype>FIELD1</hwk:datatype>                                         //it is an Filed of length 1 bit. So it is an simple "boolean"
                //      <hwk:byteAddr>1</hwk:byteAddr>                                              //it has the offset of 1
                //      <hwk:bitAddr>0</hwk:bitAddr>                                                //the Bit offset. this number refers to the "bit position"
                //  </hwk:Par>
                //  <hwk:Par Name = "Startup_if_Setpoint_Cfg_Not_Equal_to_Actual_Cfg" PNr="2">      //Name for next parameter
                //      <hwk:datatype>FIELD1</hwk:datatype>                                         //also an Boolean
                //      <hwk:byteAddr>1</hwk:byteAddr>                                              //on the second byte
                //      <hwk:bitAddr>1</hwk:bitAddr>                                                //this time the "second" bit of the byte
                //  </hwk:Par>
                //  ...
                //  ...
                //</hwk:Dataset>
                //
                //<hwk:Dataset Name = "Cycle" DSNr="2" DSLen="8">                                   //Data set with id 2, should also have length 8
                //  <hwk:Par Name = "Communication" PNr="1">                                        //This time the communication percentage
                //      <hwk:datatype>BYTE</hwk:datatype>                                           //it is an Byte 
                //      <hwk:byteAddr>0</hwk:byteAddr>                                              //at offset 0, the first one
                //      <hwk:bitAddr>0</hwk:bitAddr>                                                //the bit offset is irrelevant, because we are talking about an byte
                //  </hwk:Par>
                //  <hwk:Par Name = "Update_Process_Image_Table_Cyclically" PNr="2">
                //      <hwk:datatype>FIELD1</hwk:datatype>                                         //another Boolean
                //      <hwk:byteAddr>1</hwk:byteAddr>                                              //at the second byte
                //      <hwk:bitAddr>0</hwk:bitAddr>                                                //with that offset
                //  </hwk:Par>
                //  ...
                //  ...
                //</hwk:Dataset>

                switch (DataSetID)
                {
                    case 1: //Startup
                        {
                            var bits = r.ReadInt16();   //read 16, because the next read hast to be aligned to word size
                            dict.Add("Startup\\Test_on_Complete_Restart", (bits & 1) != 0);
                            dict.Add("Startup\\Startup_if_Setpoint_Cfg_Not_Equal_to_Actual_Cfg", (bits & 2) != 0);
                            dict.Add("Startup\\Disable_Hot_restart_on_Manual_Startup", (bits & 4) != 0);
                            dict.Add("Startup\\Startup_after_Power_On", (bits & 8) != 0);
                            dict.Add("Startup\\Delete_PIQ_on_Hot_restart", (bits & 061) != 0);
                            dict.Add("Startup\\Cold_restart", (bits & 32) != 0);
                            dict.Add("Startup\\Timebase", (bits & 64) != 0);

                            dict.Add("Startup\\Transfer_of_Parameters_to_Modules", r.ReadInt16());
                            dict.Add("Startup\\Ready_Message_from_Modules", r.ReadUInt16());
                            dict.Add("Startup\\Hot_restart", r.ReadInt16());
                            break;
                        }
                    case 2: //Cycle
                        {
                            dict.Add("Cycle\\Communication", r.ReadByte());

                            var bits = r.ReadByte(); //read only 8, because the next read hast to be aligned to word size
                            dict.Add("Cycle\\Update_Process_Image_Table_Cyclically", (bits & 1) != 0);
                            dict.Add("Cycle\\Size_of_process_image", (bits & 2) != 0);
                            dict.Add("Cycle\\Call_OB85_on_IO_Access_Error", (bits & 16) != 0);
                            dict.Add("Cycle\\Call_OB122_on_Direct_Access", (bits & 127) != 0);
                            dict.Add("Cycle\\Scan_Cycle_Monitoring_Time", r.ReadUInt16());
                            dict.Add("Cycle\\Minimum_Scan_Cycle_Time", r.ReadInt16());
                            dict.Add("Cycle\\Self_Test", r.ReadInt16());
                            break;
                        }
                    case 3: //MPI_Communication
                        {
                            dict.Add("MPI_Communication\\Total_Number_of_Connections", r.ReadByte());
                            dict.Add("MPI_Communication\\Number_of_Available_Connections", r.ReadByte());
                            dict.Add("MPI_Communication\\Number_of_Reserved_Connections_to_Programming_Device", r.ReadByte());
                            dict.Add("MPI_Communication\\Number_of_Reserved_Connections_to_OI", r.ReadByte());
                            dict.Add("MPI_Communication\\Station_Type_Passive", r.ReadByte());
                            dict.Add("MPI_Communication\\Medium_Redundancy", r.ReadByte());
                            dict.Add("MPI_Communication\\Highest_MPI_Address", r.ReadByte());
                            dict.Add("MPI_Communication\\MPI_Address", r.ReadByte());
                            dict.Add("MPI_Communication\\Retry_Counter", r.ReadByte());
                            dict.Add("MPI_Communication\\Transmission_Rate", r.ReadByte());
                            dict.Add("MPI_Communication\\Physical_Layer", r.ReadByte());
                            dict.Add("MPI_Communication\\In_Ring_Desired", r.ReadByte());
                            dict.Add("MPI_Communication\\Default_LSAP_ID", r.ReadByte());
                            dict.Add("MPI_Communication\\GAP_Update_Factor", r.ReadByte());
                            dict.Add("MPI_Communication\\Ready_Time", r.ReadByte());
                            dict.Add("MPI_Communication\\Transmitter_Fall_Time", r.ReadInt16());
                            dict.Add("MPI_Communication\\Slot_Time", r.ReadInt16());
                            dict.Add("MPI_Communication\\IDLE_1", r.ReadInt16());
                            dict.Add("MPI_Communication\\IDLE_2", r.ReadInt16());
                            dict.Add("MPI_Communication\\Factor_for_Target_Rotation_Time", r.ReadInt16());
                            break;
                        }
                    case 7: //Clock_Pulse_Memory
                        {
                            var bits = r.ReadInt16(); //read 16, because the next read hast to be aligned to word size
                            dict.Add("Clock_Pulse_Memory\\Clock_Memory", (bits & 1) != 0);

                            dict.Add("Clock_Pulse_Memory\\Memory_Byte", r.ReadInt16());
                            dict.Add("Clock_Pulse_Memory\\Number_of_S7_Timers", r.ReadInt16());
                            break;
                        }
                    default:
                        break;
                }

                Curr = Curr + DataSetLength;
                r.BaseStream.Seek(Curr, SeekOrigin.Begin);
            }
            return dict;
        }
    }
}


