using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCollectorConnect.Models.Enums;
using DataCollectorConnect.Models.Base;
using Siemens.HMI;

namespace Siemens.PLC
{
    public class SiemensPlcEvent
    {
        public SiemensPlcEvent() { }

        public SiemensPlcEvent(string ip, SeverityCodes type, string productionEntity, string triggerStart, string triggerEnd, List<EventTranslation> translations)
        {
            Ip = ip;
            Type = type;
            ProductionEntity = productionEntity;
            TriggerStart = triggerStart;
            TriggerEnd = triggerEnd;
            Translations = translations;
        }

        public string Ip { get; set; }
        public SeverityCodes Type { get; set; }
        public string ProductionEntity { get; set; }
        public string TriggerStart { get; set; }
        public string TriggerEnd { get; set; }
        public Metadata PlcMetadata { get; set; }
        public List<DataCollectorConnect.Models.Base.EventTranslation> Translations { get; set; }

        public class Metadata
        {
            public Metadata(uint id, bool isDynamic, bool isSymbolic, uint alarmClass, string alarmClassName, string symbolicTag, string blockName, string hmiName, string importFileName)
            {
                Id = id;
                IsDynamic = isDynamic;
                IsSymbolic = isSymbolic;
                AlarmClass = alarmClass;
                AlarmClassName = alarmClassName;
                SymbolicTag = symbolicTag;
                BlockName = blockName;
                HmiName = hmiName;
                ImportFileName = importFileName;
            }

            public uint Id { get; set; }
            public bool IsDynamic { get; set; }
            public bool IsSymbolic { get; set; }
            public uint AlarmClass { get; set; }
            public string AlarmClassName { get; set; }
            public string SymbolicTag { get; set; }
            public string BlockName { get; set; }
            public string HmiName { get; set; }
            public string ImportFileName { get; set; }
        }
    }
}
