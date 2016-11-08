using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ToolReader
{
    public class ToolData : IDisposable
    {


        #region CTOR

        public ToolData()
        {
            RestDurability = TimeSpan.MinValue;
        }

        public ToolData(string depot, string place)
            : this()
        {
            SetLocation(depot, place);
        }

        #endregion


        public int Id { get; set; }
        private string _ToolIdentNumber;

        /// <summary>
        /// Tool Number called from withing NcProgram
        /// </summary>
        public string ToolIdentNumber
        {
            get
            {
                return _ToolIdentNumber;
            }
            set
            {
                if (_ToolIdentNumber != value)
                {
                    _ToolIdentNumber = value;
                }
            }
        }

        /// <summary>
        /// Sister tool number.
        /// </summary>
        public int Duplo { get; set; }
        public int Edges { get; set; }

        /// <summary>
        /// Unique number of tool (Siemens T-Number).
        /// </summary>
        public string InternalToolNumber { get; set; }

        public string Depot { get; set; }

        public string Place { get; set; }

        private bool _Locked = false;

        public bool Locked
        {
            get { return _Locked; }
            set
            {

                if (_Locked != value)
                {
                    _Locked = value;
                }
            }
        }

        public TimeSpan LastRestDurability { get; set; }

        private TimeSpan _MaxTime = TimeSpan.MinValue;

        public double RestDurabilityTotalMinutes
        {
            get
            {
                return RestDurability.TotalMinutes;
            }
        }

        public double MaxTimeTotalMinutes
        {
            get
            {
                return MaxTime.TotalMinutes;
            }
        }


        public string Status
        {
            get
            {
                return Locked ? "gesperrt" : "ok"; // Todo: Übersetzung, weitere Statusse
            }
        }

        public TimeSpan MaxTime
        {
            get
            {
                return _MaxTime;
            }
            set
            {
                if (_MaxTime != value)
                {
                    _MaxTime = value;
                }
            }
        }

        public TimeSpan CurrTime { get; set; }


        TimeSpan _RestDurability = TimeSpan.MinValue;

        /// <summary>
        /// Rest durability.
        /// </summary>
        public TimeSpan RestDurability
        {
            get { return _RestDurability; }
            set
            {
                _RestDurability = value;

            }
        }
        public void SetLocation(string depot, string place)
        {
            Depot = depot;
            Place = place;
        }

        public override string ToString()
        {
            try
            {
                return String.Format("Ident:{0} Internal:{1} RestDura:{2} Location:{3}/{4}", ToolIdentNumber, InternalToolNumber, RestDurability.TotalSeconds, Depot, Place);
            }
            catch
            {
                return base.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            ToolData comp = obj as ToolData;
            bool result = false;
            try
            {
                if (comp == null && obj != null)
                {
                    result = true;
                }
                else
                {
                    if ((comp == null && obj != null) || (comp != null && obj == null))
                        result = false;
                    else
                        result = this.ToolIdentNumber == comp.ToolIdentNumber && this.Duplo == comp.Duplo && this.InternalToolNumber == comp.InternalToolNumber;
                }
            }
            catch
            {
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Dispose()
        {
        }
    }

}
