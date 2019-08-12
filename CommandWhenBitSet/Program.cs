using System;
using System.Diagnostics;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace CommandWhenBitSet
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length>0)
            {
                if (args[0]=="/config" || args[0]=="-config" || args[0]=="--config")
                {
                    Configuration.ShowConfiguration("CommandWhenBitSetConn", true);
                }
            }
            else
            {
                try
                {
                    PLCConnection myConn = new PLCConnection("CommandWhenBitSetConn");
                    myConn.Connect();
                    PLCTag tag=new PLCTag(Settings.Default.PLCVar);
                    myConn.ReadValue(tag);
                    if ((bool)tag.Value == true)
                    {
                        Process P = new Process();
                        P.StartInfo.FileName = Settings.Default.CommandToRun;
                        P.StartInfo.Arguments = Settings.Default.CommandToRun;
                        P.Start();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
