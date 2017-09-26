using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdConfig_Click(object sender, EventArgs e)
        {
            DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration("Toolreader", true);
        }

        private DotNetSiemensPLCToolBoxLibrary.Communication.PLCConnection myConn = null;
        private void cmdConnect_Click(object sender, EventArgs e)
        {
            DoConnect();
        }

        private void DoConnect()
        {
            try
            {
                lblStatus.Text = "";
                myConn = new PLCConnection("Toolreader");
                myConn.Connect();
                lblStatus.Text = "Connected!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }


        public List<ToolData> GetToolList()
        {
            //toolData = new List<ToolData>();
            bool error = false;


            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (myConn == null)
                DoConnect();
            // Meherere Datenbausteine müssen eingelesen werden

            /////////////////////////////////////////////////////
            // Beginn mit Datenbaustein TP (MagazinPlatzdaten):

        

            List<ToolData> toolData = new List<ToolData>();

            PLCNckTag tag = new PLCNckTag();

            // Anzahl der Werkzeuge
            tag.NckArea = NCK_Area.AreaTool;
            tag.NckModule = (int)NCK_Block.BlockTV;

            tag.NckLinecount = 1;
            tag.NckUnit = 1;

            tag.NckLine = 1;
            tag.NckColumn = 1;
            tag.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;


            myConn.ReadValue(tag);
            ushort numberOfTools = 0;
            object o = new object();
            numberOfTools = (UInt16)tag.Value;


            List<PLCNckTag> tagList = new List<PLCNckTag>();

            int line = 1;
            for (int i = 0; i < numberOfTools; i++)
            {
                try
                {
                    //---------------------  InternalNumber Tno   -------------------------------
                    PLCNckTag internalToolNumber = new PLCNckTag();
                    internalToolNumber.NckArea = NCK_Area.AreaTool;
                    internalToolNumber.NckModule = (int)NCK_Block.BlockTV;

                    internalToolNumber.NckLinecount = 1;
                    internalToolNumber.NckUnit = 1;

                    internalToolNumber.NckLine = line;
                    internalToolNumber.NckColumn = (int)NCK_BlockTVColumn.TNo;
                    internalToolNumber.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
                    internalToolNumber.Tag = "InternalToolNumber";
                    tagList.Add(internalToolNumber);


                    //---------------------  IdentNumber  -------------------------------

                    PLCNckTag identNumber = new PLCNckTag();
                    identNumber.NckArea = NCK_Area.AreaTool;
                    identNumber.NckModule = (int)NCK_Block.BlockTV;

                    identNumber.NckLinecount = 1;
                    identNumber.NckUnit = 1;

                    identNumber.NckLine = line;
                    identNumber.NckColumn = (int)NCK_BlockTVColumn.Identnumber;

                    identNumber.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.CharArray;
                    identNumber.ArraySize = 32;
                    identNumber.Tag = "ToolIdentNumber";
                    tagList.Add(identNumber);
                    //---------------------  Duplo  -------------------------------
                    PLCNckTag duploNumber = new PLCNckTag();
                    duploNumber.NckArea = NCK_Area.AreaTool;
                    duploNumber.NckModule = (int)NCK_Block.BlockTV;

                    duploNumber.NckLinecount = 1;
                    duploNumber.NckUnit = 1;

                    duploNumber.NckLine = line;
                    duploNumber.NckColumn = (int)NCK_BlockTVColumn.Duplo;
                    duploNumber.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
                    duploNumber.Tag = "Duplo";
                    tagList.Add(duploNumber);

                    //---------------------  Edges  -------------------------------

                    PLCNckTag edges = new PLCNckTag();
                    edges.NckArea = NCK_Area.AreaTool;
                    edges.NckModule = (int)NCK_Block.BlockTV;

                    edges.NckLinecount = 1;
                    edges.NckUnit = 1;

                    edges.NckLine = line;
                    edges.NckColumn = (int)NCK_BlockTVColumn.Edges;
                    edges.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
                    edges.Tag = "Edges";
                    tagList.Add(edges);


                    //---------------------  Depot  -------------------------------

                    PLCNckTag depot = new PLCNckTag();
                    depot.NckArea = NCK_Area.AreaTool;
                    depot.NckModule = (int)NCK_Block.BlockTV;

                    depot.NckLinecount = 1;
                    depot.NckUnit = 1;

                    depot.NckLine = line;
                    depot.NckColumn = (int)NCK_BlockTVColumn.Depot;
                    depot.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
                    depot.Tag = "Depot";
                    tagList.Add(depot);



                    //---------------------  Place  -------------------------------
                    PLCNckTag place = new PLCNckTag();
                    place.NckArea = NCK_Area.AreaTool;
                    place.NckModule = (int)NCK_Block.BlockTV;

                    place.NckLinecount = 1;
                    place.NckUnit = 1;

                    place.NckLine = line;
                    place.NckColumn = (int)NCK_BlockTVColumn.Place;
                    place.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
                    place.Tag = "Place";
                    tagList.Add(place);



                    ++line;
                }
                catch (Exception ex)
                {

                }
            }

            //PLCNckTag p = new PLCNckTag();
            //p.NckArea = (int)NCK_Area.AreaTool;
            //p.NckModule = (int)NCK_Block.BlockTV;

            //p.NckLinecount = 1;
            //p.NckUnit = 1;

            //p.NckLine = 1;
            //p.NckColumn = (int)NCK_BlockTVColumn.Place;
            //p.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word;
            //p.Tag = "Place";
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);
            //tagList.Add(p);

            myConn.ReadValues(tagList);

            
            foreach (var nckTag in tagList)
            {
                if (nckTag.Tag != null)
                {


                    ToolData td = toolData.Where(w => w.Id == nckTag.NckLine).FirstOrDefault();
                    if (td == null)
                    {
                        td = new ToolData();
                        td.Id = nckTag.NckLine;
                        toolData.Add(td);
                    }

                    PropertyInfo prop = typeof(ToolData).GetProperty(nckTag.Tag.ToString(), BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        try
                        {
                            prop.SetValue(td, Convert.ChangeType(nckTag.Value, prop.PropertyType), null);
                        }catch{}
                    }


                }
            }
            tagList = new List<PLCNckTag>();
            foreach (var td in toolData)
            {

                /////////////////////////////////////////////////////////
                ////// Beginn mit Datenbaustein TS (Schneidedaten, Korrekturdaten):

                //    //P1 = Vorwarngrenze Standzeit in Minuten ($TC_MOP1)
                //    //P2 = Verbleibende Standzeit in Minuten ($TC_MOP2)
                //    //P3 = Vorwarngrenze Stückzahl ($TC_MOP3)
                //    //P4 = verbleibende Stückzahl ($TC_MOP4)
                //    //P5 = Sollstandzeit ($TC_MOP11)
                //    //P6 = Sollstückzahl ($TC_MOP13)
                //    //P7 = Vorwarngrenze Verschleiß (Vorwarngrenze) (ab SW 5.1) ($TC_MOP5)
                //    //    Dieser Parameter kann nur gesetzt werden, wenn Bit 5 von Maschinendatum     $MN_MM_TOOL_MANAGEMENT_MASK entsprechend gesetzt ist.
                //    //P8 = verbleibender Verschleiß (Istwert) (ab SW 5.1) ($TC_MOP6) nicht schreibbar
                //    //P9 = Sollwert Verschleiß (ab SW 5.1) ($TC_MOP15)
                //    //    Dieser Parameter kann nur gesetzt werden, wenn Bit 5 von Maschinendatum
                //    //    $MN_MM_TOOL_MANAGEMENT_MASK entsprechend gesetzt ist.
                PLCNckTag restdurabillity = new PLCNckTag();
                restdurabillity.NckArea = NCK_Area.AreaTool;
                restdurabillity.NckModule = (int)NCK_Block.BlockTS;

                restdurabillity.NckLinecount = 1;
                restdurabillity.NckUnit = 1;

                restdurabillity.NckLine = (int)NCK_BlockTSLine.Resdurabillity;
                restdurabillity.NckColumn = int.Parse(td.InternalToolNumber);
                restdurabillity.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.LReal;
                restdurabillity.Tag = "RestDurability";
                tagList.Add(restdurabillity);

                PLCNckTag setTime = new PLCNckTag();
                setTime.NckArea = NCK_Area.AreaTool;
                setTime.NckModule = (int)NCK_Block.BlockTS;

                setTime.NckLinecount = 1;
                setTime.NckUnit = 1;

                setTime.NckLine = (int)NCK_BlockTSLine.SetTime;
                setTime.NckColumn = int.Parse(td.InternalToolNumber);
                setTime.TagDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.LReal;
                setTime.Tag = "MaxTime";
                tagList.Add(setTime);

            }


            myConn.ReadValues(tagList);

            foreach (var nckTag in tagList)
            {
                if (nckTag.Tag != null)
                {
                    ToolData td = toolData.Where(w => w.InternalToolNumber == nckTag.NckColumn.ToString()).FirstOrDefault();
                    if (td != null)
                    {
                        PropertyInfo prop = typeof(ToolData).GetProperty(nckTag.Tag.ToString(), BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            try
                            {
                                if (prop.PropertyType == typeof(TimeSpan))
                                {
                                    TimeSpan ts = TimeSpan.FromSeconds(double.Parse(nckTag.Value.ToString()));
                                    prop.SetValue(td, ts, null);
                                }
                                else
                                {
                                    prop.SetValue(td, Convert.ChangeType(nckTag.Value, prop.PropertyType), null);
                                }
                            }
                            catch
                            {
                               
                            }
                        }

                    }

                }
            }

            if (error)
                Console.WriteLine("Error GetTools in Aglink happened");


            sw.Stop();

            lblStatus.Text = sw.ElapsedMilliseconds.ToString() + " ms";
            if (toolData != null)
            {
                dataGridView1.DataSource = toolData;
            }
            return toolData;
        }

        private void btnReadTools_Click(object sender, EventArgs e)
        {
            GetToolList();
            GetParts();
        }

        private void GetParts()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (myConn == null)
                DoConnect();
            
            List<PLCNckTag> tagList = new List<PLCNckTag>();

            try
            {
                //---------------------  ActualParts   -------------------------------

                NC_Var actualParts = new NC_Var();
                actualParts.Bereich_u_einheit = 0x40;
                actualParts.Spalte = 0x79;
                actualParts.Zeile = 0x1;
                actualParts.Bausteintyp = 0x7F;
                actualParts.ZEILENANZAHL = 0x1;
                actualParts.Typ = 0xF;
                actualParts.Laenge = 0x8;



                PLCNckTag actualPartsTag = actualParts.GetNckTag(1, 0);
                actualPartsTag.Tag = "ActualParts";
                tagList.Add(actualPartsTag);
                
                //---------------------  TotalParts  -------------------------------

                NC_Var totalParts = new NC_Var();
                totalParts.Bereich_u_einheit = 0x40;
                totalParts.Spalte = 0x78;
                totalParts.Zeile = 0x1;
                totalParts.Bausteintyp = 0x7F;
                totalParts.ZEILENANZAHL = 0x1;
                totalParts.Typ = 0xF;
                totalParts.Laenge = 0x8;
                
                PLCNckTag totalPartsTag = totalParts.GetNckTag(1, 0);
                totalPartsTag.Tag = "TotalParts";
                tagList.Add(totalPartsTag);



                NC_Var partsCounter = new NC_Var(0x82, 0x40, 0x6CE8, 0x1, 0x1A, 0x1, 0x7, 0x4);  // soll 272 sein, dann ists richtig aktiviert
                PLCNckTag counterPartsTag = partsCounter.GetNckTag(1, 0);
                tagList.Add(counterPartsTag);
                                

                NC_Var cycleTime = new NC_Var(0x82, 0x40, 0x9, 0x1, 0x3B, 0x1, 0xF, 0x8);
                PLCNckTag cycleTimeTag = cycleTime.GetNckTag(1, 0);
                tagList.Add(cycleTimeTag);
                
                NC_Var oldProgNetTime = new NC_Var(0x82, 0x40, 0x12A, 0x1, 0x7F, 0x1, 0xF, 0x8);
                PLCNckTag oldProgNetTimeTag = oldProgNetTime.GetNckTag(1, 0);
                tagList.Add(oldProgNetTimeTag);

                oldProgNetTimeTag.Value = 12.3;
                myConn.WriteValue(oldProgNetTimeTag);

                myConn.ReadValues(tagList);

                lblActualParts.Text = String.Format("Actual: {0}", actualPartsTag.Value.ToString());
                lblTotalParts.Text = String.Format("Total: {0}", totalPartsTag.Value.ToString());
                lblCounter.Text = String.Format("Counter: {0}", counterPartsTag.Value.ToString());
                lblCycleTime.Text = String.Format("CycleTime: {0}", cycleTimeTag.Value.ToString());
                lblOldProgNetTime.Text = String.Format("OldProgNetTime: {0}", oldProgNetTimeTag.Value.ToString());

            }
            catch
            {

            }

            sw.Stop();

            lblStatus.Text += "           "+ sw.ElapsedMilliseconds.ToString() + " ms";
        }

    }
}
