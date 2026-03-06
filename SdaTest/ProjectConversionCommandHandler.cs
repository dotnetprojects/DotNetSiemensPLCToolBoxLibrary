using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Hardware.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;
using Newtonsoft.Json;

namespace SdaTest
{
    public class DeviceInfoEthernetCommunication
    {
        [JsonProperty("ip_address")] public string IpAddress { get; set; }
        [JsonProperty("subnet_mask")] public string SubnetMask { get; set; }
    }

    public class DeviceInfo
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("order_number")] public string OrderNumber { get; set; }
        [JsonProperty("device_type")] public string DeviceType { get; set; }

        [JsonProperty("origin_device_type")]
        public string OriginDeviceType { get; set; }

        [JsonProperty("ethernet_communication")]
        public List<DeviceInfoEthernetCommunication> EthernetCommunication { get; set; }

        [JsonProperty("password_protected")]
        public bool PasswordProtected { get; set; }
    }

    public class ProjectConversionCommandHandler
    {
        private static string _outputDir = "";
        private const string CpuDir = "cpu";
        private const string CpDir = "cp";
        private const string SymbolsDir = "symbols";
        private const string SourcesDir = "sources";
        private const string BlocksDir = "blocks";

        private static void CreateWorkingDirectory(string directory, bool cleanup = false)
        {
            if (cleanup && Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            Directory.CreateDirectory(directory);
        }

        public static void Export(string projectPath, string outputPath, string? username = null,
            string? password = null)
        {
            _outputDir = outputPath;
            CreateWorkingDirectory(_outputDir);
            CreateWorkingDirectory(Path.Combine(_outputDir, CpuDir), true);
            CreateWorkingDirectory(Path.Combine(_outputDir, CpDir), true);
            CreateWorkingDirectory(Path.Combine(_outputDir, SymbolsDir), true);
            CreateWorkingDirectory(Path.Combine(_outputDir, SourcesDir), true);
            CreateWorkingDirectory(Path.Combine(_outputDir, BlocksDir), true);

            Credentials? credentials = null;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                credentials = new Credentials() { Username = username, Password = new System.Security.SecureString() };
                foreach (var c in password)
                {
                    credentials.Password.AppendChar(c);
                }
            }

            var project = Projects.LoadProject(projectPath, false, credentials);
            // Setting the project language here (always use English)
            project.ProjectLanguage = MnemonicLanguage.English;

            var projectItem = new XElement("ProjectItem",
                new XAttribute("Name", "Project"),
                new XAttribute("Type", "Folder"));

            // Parse the project and create the XML structure and write the XML files to the outputPath
            AddXmlNodes(projectItem, project.ProjectStructure.SubItems);

            var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), projectItem);
            xmlDocument.Save(Path.Combine(outputPath, "project_tree.xml"));

            var stationConfigurations = project.ProjectStructure.SubItems
                .FindAll(sc => sc is StationConfigurationFolder)
                .ConvertAll(sc => (StationConfigurationFolder)sc);
            GenerateDeviceConnectionInfo(stationConfigurations);
        }

        private static string HashContent(string content)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /**
         * Create a new file with the given parameters and return the File XML item
         */
        private static XElement CreateFile(XElement item, string subDirectory, string uniqueFileName,
            string contentType)
        {
            var hashedFileName = $"{HashContent(uniqueFileName)}.xml";
            var relativeFilePath = Path.Combine(subDirectory, hashedFileName);
            var absoluteFilePath = Path.Combine(_outputDir, relativeFilePath);
            var xmlRelativeFilePath = $"{subDirectory}/{hashedFileName}";
            var newXmlDoc = new XDocument(item);
            var newXmlContent = newXmlDoc.ToString();
            File.WriteAllText(absoluteFilePath, newXmlContent);
            var fileHash = HashContent(newXmlContent);
            return new XElement("File", new XAttribute("DiffHash", fileHash),
                new XAttribute("ContentType", contentType),
                xmlRelativeFilePath);
        }

        private static XElement CreateItemElement(string name, string type)
        {
            return new XElement("Item", new XAttribute("Name", name), new XAttribute("Type", type));
        }

        private static XElement CreateSdaAttributeListElement(List<List<string>> sdaAttributes, List<string> columns,
            string? title = null, string? comment = null)
        {
            var listRoot = new XElement("SdaTreeTable");

            if (title is not null)
            {
                listRoot.Add(new XElement("Title", title));
            }

            if (comment is not null)
            {
                listRoot.Add(new XElement("Comment", comment));
            }

            var columnConfig = new XElement("ColumnsConfig", new XAttribute("KeyColumn", columns.First()));
            foreach (var column in columns)
            {
                columnConfig.Add(new XElement("Column", new XAttribute("Col", column),
                    new XAttribute("Label", column)));
            }

            listRoot.Add(columnConfig);

            var rows = new XElement("Rows");
            foreach (var sdaAttribute in sdaAttributes)
            {
                var row = new XElement("Row");

                for (var i = 0; i < sdaAttribute.Count; i++)
                {
                    row.Add(new XElement("Cell", new XAttribute("Col", columns[i]), sdaAttribute[i]));
                }

                rows.Add(row);
            }

            listRoot.Add(rows);
            return listRoot;
        }

        private static XElement GetNetworkInterfaceList(string title, List<NetworkInterface> networkInterfaces)
        {
            var interfaceColumns = new List<string> { "Name", "Type", "Address" };
            var interfaceAttributes = new List<List<string>>();

            foreach (var networkInterface in networkInterfaces)
            {
                var address = networkInterface.ToString();

                switch (networkInterface)
                {
                    case EthernetNetworkInterface ethernetNetworkInterface:
                        var mac = string.Join(":",
                            Array.ConvertAll(ethernetNetworkInterface.Mac.GetAddressBytes(), b => b.ToString("X2")));
                        address = ethernetNetworkInterface.IpAddress + " (" + mac + ")";
                        break;
                    case MpiProfiBusNetworkInterface mpiProfiBusNetworkInterface:
                        address = "Address: " + mpiProfiBusNetworkInterface.Address;
                        break;
                }

                interfaceAttributes.Add(new List<string>
                {
                    networkInterface.Name,
                    networkInterface.NetworkInterfaceType.ToString(),
                    address
                });
            }

            return CreateSdaAttributeListElement(interfaceAttributes, interfaceColumns, title);
        }

        private static void AddStationConfigurationDetails(XElement rootXml,
            StationConfigurationFolder stationConfiguration)
        {
            const string treeTableType = "SdaTreeTables";
            var tableRoot = new XElement(treeTableType);
            // Set the type of the parent item
            rootXml.SetAttributeValue("Type", treeTableType);

            foreach (var masterSystem in stationConfiguration.MasterSystems)
            {
                var interfaceColumns = new List<string> { "Type", "Module" };
                var title = masterSystem.Name + " Interfaces";
                var interfaceAttributes = (from networkInterface in masterSystem.Children
                    let type = networkInterface.GetType().Name + " (" + networkInterface.NodeId + ")"
                    select new List<string> { type, networkInterface.Name }).ToList();
                tableRoot.Add(CreateSdaAttributeListElement(interfaceAttributes, interfaceColumns, title));
            }

            var uniqueStationName = stationConfiguration.StructuredFolderName;
            rootXml.Add(CreateFile(tableRoot, CpuDir, uniqueStationName, treeTableType));
        }

        private static void AddCpuDetails(XElement rootXml, CPUFolder cpuFolder)
        {
            if (cpuFolder.NetworkInterfaces == null) return;
            const string treeTableType = "SdaTreeTables";
            var tableRoot = new XElement(treeTableType);
            // Set the type of the parent item
            rootXml.SetAttributeValue("Type", treeTableType);
            tableRoot.Add(GetNetworkInterfaceList("Network Interfaces", cpuFolder.NetworkInterfaces));
            var uniqueCpuName = cpuFolder.StructuredFolderName;
            rootXml.Add(CreateFile(tableRoot, CpuDir, uniqueCpuName, treeTableType));
        }

        private static void AddCpDetails(XElement rootXml, CPFolder cpFolder)
        {
            if (cpFolder.NetworkInterfaces == null) return;
            const string treeTableType = "SdaTreeTables";
            var tableRoot = new XElement(treeTableType);
            // Set the type of the parent item
            rootXml.SetAttributeValue("Type", treeTableType);
            
            tableRoot.Add(GetNetworkInterfaceList("Network Interfaces", cpFolder.NetworkInterfaces));
            var uniqueCpName = cpFolder.StructuredFolderName;
            rootXml.Add(CreateFile(tableRoot, CpDir, uniqueCpName, treeTableType));
        }

        private static void AddInterfaceParameters(XElement rootXml, List<S7VATRow> parameters)
        {
            foreach (var parameter in parameters)
            {
                try
                {
                    if (parameter.LibNoDaveValue == null)
                    {
                        // This can be the case if empty (placeholders) are used
                        rootXml.Add(new XElement("variable", new XAttribute("Name", "")));
                        continue;
                    }

                    var varItem = new XElement("variable", new XAttribute("Name", parameter.LibNoDaveValue.ToString()));

                    if (!string.IsNullOrEmpty(parameter.Comment))
                    {
                        varItem.Add(new XElement("comment", parameter.Comment));
                    }

                    varItem.Add(new XElement("dataType", parameter.LibNoDaveValue.DataTypeStringFormat.ToString()));
                    varItem.Add(new XElement("address", parameter.S7FormatAddress));

                    rootXml.Add(varItem);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void AddInterfaceParameters(XElement rootXml, IDataRow parameter)
        {
            var interfaceItem = new XElement("interface");

            AddInterfaceParameters(interfaceItem, parameter.Children);

            rootXml.Add(interfaceItem);
        }

        private static void AddInterfaceParameters(XElement rootXml, List<IDataRow> parameters)
        {
            foreach (var parameter in parameters)
            {
                var varItem = new XElement("variable", new XAttribute("Name", parameter.Name));

                if (parameter is DataBlockRow dataBlockRow)
                {
                    varItem.Add(new XElement("dataType", dataBlockRow.DataTypeAsString));
                }
                else
                {
                    varItem.Add(new XElement("dataType", parameter.DataType.ToString()));
                }

                if (!string.IsNullOrEmpty(parameter.Comment))
                {
                    varItem.Add(new XElement("comment", parameter.Comment));
                }

                varItem.Add(new XElement("address", parameter.BlockAddress.ToString()));

                if (parameter is S7DataRow { StartValueAsString: not null } s7DataRowParameter)
                {
                    varItem.Add(new XElement("initialValue", s7DataRowParameter.StartValueAsString));
                }

                AddInterfaceParameters(varItem, parameter.Children);

                rootXml.Add(varItem);
            }
        }

        // Method to determine if a character is valid for XML
        private static bool IsValidXmlChar(char c)
        {
            return (c == 0x9 || c == 0xA || c == 0xD || (c >= 0x20 && c <= 0xD7FF) || (c >= 0xE000 && c <= 0xFFFD) || (c >= 0x10000 && c <= 0x10FFFF));
        }
        
        private static void AddBlocks(XElement rootXml, BlocksOfflineFolder blocksOfflineFolder)
        {
            var blocks = blocksOfflineFolder.BlockInfos.Select(blockIt => blockIt.GetBlock());
            
            foreach (var block in blocks)
            {
                try
                {
                    var blockType = block.GetType().Name;
                    var blockItem = CreateItemElement(block.BlockName, blockType);
                    var blockContent = new XElement(blockType, new XAttribute("Name", block.BlockName),
                        new XAttribute("Type", blockType));


                    switch (block)
                    {
                        case S7FunctionBlock s7FunctionBlock:
                            // TODO: Check the mnemonic language
                            s7FunctionBlock.MnemonicLanguage = MnemonicLanguage.English;
                            
                            if (!string.IsNullOrEmpty(s7FunctionBlock.Title))
                            {
                                blockContent.Add(new XAttribute("Title", s7FunctionBlock.Title));
                            }

                            if (!string.IsNullOrEmpty(s7FunctionBlock.Description))
                            {
                                blockContent.Add(new XAttribute("Description", s7FunctionBlock.Description));
                            }

                            AddInterfaceParameters(blockContent, s7FunctionBlock.Parameter);

                            var networksItem = new XElement("Networks");
                            
                            foreach (var fbNetwork in s7FunctionBlock.Networks)
                            {
                                var networkItem = new XElement("Network", new XAttribute("Title", fbNetwork.Name));
                                networkItem.Add(new XElement("comment", fbNetwork.Comment));
                                var rawAwl = fbNetwork.AWLCodeToString();
                                var sanitizedAwl = new string(rawAwl.Select(c => IsValidXmlChar(c) ? c : '?').ToArray());
                                networkItem.Add(new XElement("AWL", sanitizedAwl));
                                // Add the network item to the networks
                                networksItem.Add(networkItem);
                            }

                            blockContent.Add(networksItem);
                            break;
                        case S7DataBlock s7DataBlock:
                            if (!string.IsNullOrEmpty(s7DataBlock.Title))
                            {
                                blockContent.Add(new XAttribute("Title", s7DataBlock.Title));
                            }

                            AddInterfaceParameters(blockContent, s7DataBlock.Structure.Children);
                            break;
                        case S7VATBlock s7VatBlock:
                            AddInterfaceParameters(blockContent, s7VatBlock.VATRows);
                            break;
                    }

                    var uniqueBlockName = blocksOfflineFolder.StructuredFolderName + "/" + block.BlockName;
                    blockItem.Add(
                        CreateFile(blockContent, BlocksDir, uniqueBlockName, blockType));

                    rootXml.Add(blockItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error when extracting block {block.BlockName} of type {block.BlockType.ToString()} {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }

        private static void AddSymbols(XElement rootXml, SymbolTable symbolTable)
        {
            var symbolTableType = $"S7{symbolTable.GetType().Name}";
            var symbolTableContent = new XElement(symbolTableType, new XAttribute("Name", symbolTable.Name),
                new XAttribute("Type", symbolTableType));
            
            // Set the type of the parent item
            rootXml.SetAttributeValue("Type", symbolTableType);

            var symbolEntries = symbolTable.SymbolTableEntrys;
            symbolEntries.Sort((x, y) => string.Compare(x.Symbol, y.Symbol, StringComparison.Ordinal));

            foreach (var symbolTableEntry in symbolEntries)
            {
                var symbolItem = new XElement("symbol");
                symbolItem.Add(new XAttribute("Symbol", symbolTableEntry.Symbol));
                symbolItem.Add(new XAttribute("Comment", symbolTableEntry.Comment));
                symbolItem.Add(new XAttribute("Type", symbolTableEntry.DataType));
                symbolItem.Add(new XAttribute("Address", symbolTableEntry.OperandIEC));

                symbolTableContent.Add(symbolItem);
            }

            var uniqueSymbolName = symbolTable.StructuredFolderName;
            rootXml.Add(
                CreateFile(symbolTableContent, SymbolsDir, uniqueSymbolName, symbolTableType));
        }

        private static void AddSources(XElement rootXml, SourceFolder sources)
        {
            foreach (var blockInfo in sources.BlockInfos)
            {
                var sourceBlockType = blockInfo.GetType().Name;
                var sourceBlockItem = CreateItemElement(blockInfo.Name, sourceBlockType);
                var sourceBlockContent = new XElement(sourceBlockType, new XAttribute("Name", blockInfo.Name),
                    new XAttribute("Type", sourceBlockType));

                if (blockInfo is S7ProjectSourceInfo s7ProjectSourceInfo)
                {
                    var rawSource = sources.GetSource(s7ProjectSourceInfo);
                    var source = new string(rawSource.Select(c => IsValidXmlChar(c) ? c : '?').ToArray());
                    sourceBlockContent.Add(source);
                    sourceBlockContent.Add(source);
                }

                var uniqueSourceName = sources.StructuredFolderName + "/" + blockInfo.Name;
                sourceBlockItem.Add(
                    CreateFile(sourceBlockContent, SourcesDir, uniqueSourceName, sourceBlockType));
                rootXml.Add(sourceBlockItem);
            }
        }

        private static void AddXmlNodes(XElement rootXml, List<ProjectFolder> items)
        {
            foreach (var subitem in items)
            {
                if (subitem is MasterSystem)
                {
                    continue;
                }

                var subItemElement = CreateItemElement(subitem.Name, "item");

                switch (subitem)
                {
                    case StationConfigurationFolder stationConfiguration:
                        AddStationConfigurationDetails(subItemElement, stationConfiguration);
                        break;
                    case CPUFolder cpuFolder:
                        AddCpuDetails(subItemElement, cpuFolder);
                        break;
                    case CPFolder cpFolder:
                        AddCpDetails(subItemElement, cpFolder);
                        break;
                    case SymbolTable symbolTable:
                        AddSymbols(subItemElement, symbolTable);
                        break;
                    case BlocksOfflineFolder blocksOfflineFolder:
                        AddBlocks(subItemElement, blocksOfflineFolder);
                        break;
                    case SourceFolder sourceFolder:
                        AddSources(subItemElement, sourceFolder);
                        break;
                }

                if (subitem.SubItems != null) AddXmlNodes(subItemElement, subitem.SubItems);
                rootXml.Add(subItemElement);
            }
        }

        private static void GenerateDeviceConnectionInfo(List<StationConfigurationFolder> stationConfigurations)
        {
            try
            {
                // Create device information list and append a new device
                var deviceInformation = new List<DeviceInfo>();

                foreach (var stationConfiguration in stationConfigurations)
                {
                    var cpu = stationConfiguration.SubItems.FirstOrDefault(x => x is CPUFolder) as CPUFolder;
                    var cp = stationConfiguration.SubItems.FirstOrDefault(x => x is CPFolder) as CPFolder;
                    var ethernetInterfaces = cp?.NetworkInterfaces
                        .FindAll(ni => ni is EthernetNetworkInterface)
                        .ConvertAll(ni => (EthernetNetworkInterface)ni);

                    var ethernetCommunication = ethernetInterfaces?
                        .Select(ni => new DeviceInfoEthernetCommunication
                        {
                            IpAddress = ni.IpAddress.ToString(),
                            SubnetMask = ni.SubnetMask.ToString()
                        })
                        .ToList() ?? new List<DeviceInfoEthernetCommunication>();

                    deviceInformation.Add(new DeviceInfo
                    {
                        Name = stationConfiguration.Name,
                        Description = cpu?.Name ?? "Unknown",
                        OrderNumber = cpu?.MLFB_OrderNumber,
                        DeviceType = "plc",
                        OriginDeviceType = stationConfiguration.StationType.ToString(),
                        EthernetCommunication = ethernetCommunication,
                        PasswordProtected = cpu?.PasswdHard != null,
                    });
                }


                var deviceJsonFilePath = Path.Combine(_outputDir, "devices_connection_information.json");
                File.WriteAllText(deviceJsonFilePath, JsonConvert.SerializeObject(deviceInformation, Formatting.Indented));

                Console.WriteLine("Device information has been written to JSON file.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error when generating the devices connection info {e.Message}");
            }
        }
    }
}