using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackupS7
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine();
                Console.WriteLine("Bitte so starten:");
                Console.WriteLine();
                Console.WriteLine("BackupS7 Configfile Zielverzeichnis");
                Console.WriteLine();
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Creating Directory...");
                var configFile = args[0];
                var targetDirectory = Path.Combine(Environment.CurrentDirectory, args[1]);
                
                var text = File.ReadAllText(configFile);
                var config = JsonConvert.DeserializeObject<Config>(text);

                Directory.CreateDirectory(targetDirectory);

                Console.WriteLine("Connecting to PLC...");
                var connection = new PLCConnection(new PLCConnectionConfiguration("BackupS7", LibNodaveConnectionConfigurationType.ObjectSavedConfiguration) { ConnectionType = LibNodaveConnectionTypes.ISO_over_TCP, CpuIP = config.Ip, CpuRack = config.Rack, CpuSlot = config.Slot });
                connection.Connect();

                var szlDat = connection.PLCGetSZL(0x0111, 1);
                if (szlDat.SZLDaten.Length > 0)
                {
                    xy11Dataset xy11Szl = szlDat.SZLDaten[0] as xy11Dataset;
                    if (xy11Szl != null)
                        Console.WriteLine("Connected, MLFB: " + xy11Szl.MlfB);
                }

                Console.WriteLine("Read Blocks...");
                var existingBlocks = connection.PLCListBlocks(PLCBlockType.AllEditableBlocks);

                var backupBlocks = new List<string>();
                if (config.BackupBlocks != null)
                {
                    foreach (var b in config.BackupBlocks)
                    {
                        var txt = b.Trim().ToUpper().Replace(" ", "");
                        if (txt.Contains("-"))
                        {
                            var range = txt.Split('-');
                            var type = range[0].Substring(0, 2);
                            var start = int.Parse(range[0].Substring(2));
                            var end = int.Parse(range[1].Substring(2));
                            backupBlocks.AddRange(Enumerable.Range(start, (end-start)+1).Select(x => type + x));
                        }
                        else
                            backupBlocks.Add(txt);
                    }
                }

                Console.WriteLine("Backup Blocks...");
                foreach (var b in existingBlocks)
                {
                    bool backUp = false;
                    if (config.BackupType.HasFlag(BackupType.Datablocks) && b.StartsWith("DB"))
                        backUp = true;
                    if (config.BackupType.HasFlag(BackupType.Functions) && b.StartsWith("FC"))
                        backUp = true;
                    if (config.BackupType.HasFlag(BackupType.FunctionBlocks) && b.StartsWith("FB"))
                        backUp = true;
                    if (backupBlocks.Contains(b.ToUpper().Trim()))
                        backUp = true;

                    if (backUp)
                    {
                        var blk = connection.PLCGetBlockInMC7(b);

                        string file = Path.Combine(targetDirectory, b + ".blk");
                        BinaryWriter wrt = new BinaryWriter(File.Open(file, FileMode.Create));
                        wrt.Write(blk);
                        wrt.Close();
                    }
                }
                connection.Disconnect();
                Console.WriteLine("Finish");
            }
        }
    }
}
