using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCollectorConnect.Models.Base;
using DataCollectorConnect.Models.PlcDriver;
using Siemens.HMI;

namespace Siemens.PLC
{
    public class SiemensPlc : Plc
    {
        public SiemensPlc() { }

        public SiemensPlc(
            string id,
            List<SiemensPlcSubnet> plcNetwork,
            string address,
            string type,
            string rack,
            string slot,
            string partNumber,
            string firmwareVersion,
            bool status
        )
        {
            this.Id = id;
            this.Type = type;
            this.Address = address;
            this.PlcNetwork = plcNetwork;
            this.Rack = rack;
            this.Slot = slot;
            this.PartNumber = partNumber;
            this.FirmwareVersion = firmwareVersion;
            this.Status = status;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string PartNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string Rack { get; set; }
        public string Slot { get; set; }
        public bool Status { get; set; }
        public List<SiemensPlcSubnet> PlcNetwork { get; set; }
        public List<SiemensPlcSignal> Signals { get; set; }
        public List<SiemensPlcMetadata> Metadatas { get; set; }
        public List<SiemensPlcEvent> Events { get; set; }
        public SiemensHmisList hmiList { get; set; }
    }

    public class SiemensPlcsList
    {
        public SiemensPlcsList() { }

        public List<SiemensPlc> Plcs { get; set; } = new List<SiemensPlc>();

        public void InsertPlc(SiemensPlc plc)
        {
            this.Plcs.Add(plc);
        }

        /// <summary>
        /// Inserts HMI data into each PLC in the current list of PLCs. For each PLC, the method checks if it is connected
        /// to any HMIs based on the PLC's address. If a match is found, the HMI is added to the PLC's HMI list, and its events 
        /// are converted and added to the PLC's events list.
        /// </summary>
        /// <param name="hmiList">The list of Siemens HMIs to be inserted into the PLCs.</param>
        public void InsertHmis(SiemensHmisList hmiList)
        {
            foreach (SiemensPlc plc in this.Plcs)
            {
                plc.hmiList = new SiemensHmisList();

                foreach (SiemensHmi hmi in hmiList.Hmis)
                {
                    if (hmi.ConnectedPlcs == null || plc.Address == null)
                        continue;

                    foreach (string PlcIp in hmi.ConnectedPlcs)
                    {
                        if (PlcIp == plc.Address)
                        {
                            plc.hmiList.Hmis.Add(hmi);
                            plc.Events.AddRange(ConvertHmiToPlcEvents(hmi.Events));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts a list of Siemens HMI events into Siemens PLC events. For each HMI event, the method creates a corresponding 
        /// PLC event and maps relevant properties. It also maps the metadata from HMI to PLC event format.
        /// </summary>
        /// <param name="hmiEvents">The list of Siemens HMI events to convert.</param>
        /// <returns>A list of Siemens PLC events corresponding to the provided HMI events.</returns>
        private static List<SiemensPlcEvent> ConvertHmiToPlcEvents(List<SiemensHmiEvent> hmiEvents)
        {
            List<SiemensPlcEvent> plcEvents = new List<SiemensPlcEvent>();

            foreach (SiemensHmiEvent hmiEvent in hmiEvents)
            {
                // Create a new SiemensPlcEvent instance using data from hmiEvent
                SiemensPlcEvent plcEvent = new SiemensPlcEvent(
                    hmiEvent.PlcIp,
                    hmiEvent.Type,
                    hmiEvent.ProductionEntity,
                    hmiEvent.TriggerStart,
                    hmiEvent.TriggerEnd,
                    hmiEvent.Translations
                );

                // Cast HmiMetadata to Metadata
                SiemensPlcEvent.Metadata plcMetadata = new SiemensPlcEvent.Metadata(
                    hmiEvent.HmiMetadata.Id,
                    hmiEvent.HmiMetadata.IsDynamic,
                    hmiEvent.HmiMetadata.IsSymbolic,
                    hmiEvent.HmiMetadata.AlarmClass,
                    hmiEvent.HmiMetadata.AlarmClassName,
                    hmiEvent.HmiMetadata.SymbolicTag,
                    hmiEvent.HmiMetadata.BlockName,
                    hmiEvent.HmiMetadata.HmiName,
                    hmiEvent.HmiMetadata.ImportFileName
                );

                // Assign the metadata to the plcEvent
                plcEvent.PlcMetadata = plcMetadata;

                plcEvents.Add(plcEvent);
            }

            return plcEvents;
        }
    }
}
