using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;
using System;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class TiaObjectId : IComparable<TiaObjectId>, IComparable, IEquatable<TiaObjectId>
    {
        public TiaObjectId(int typeId, long instanceId)
        {
            TypeId = typeId;
            InstanceId = instanceId;
        }

        public TiaObjectId(TiaFixedRootObjectInstanceIds tiaFixedRootObjectInstanceIds)
        {
            TypeId = 0;
            InstanceId = (long)tiaFixedRootObjectInstanceIds;
        }

        public TiaObjectId(string tiaObjectId)
        {
            var s = tiaObjectId.Split(new[] { '-' });
            TypeId = int.Parse(s[0]);
            InstanceId = long.Parse(s[1]);
        }

        public int TypeId { get; set; }

        public long InstanceId { get; set; }

        public int CompareTo(object obj)
        {
            return this.CompareTo((TiaObjectId)obj);
        }

        public int CompareTo(TiaObjectId other)
        {
            if (this.TypeId == other.TypeId)
            {
                if (this.InstanceId == other.InstanceId)
                    return 0;
                if (this.InstanceId >= other.InstanceId)
                    return 1;

                return -1;
            }
            if (this.TypeId >= other.TypeId)
                return 1;
            return -1;
        }

        public bool Equals(TiaObjectId other)
        {
            return (this == (other));
        }

        public static bool operator ==(TiaObjectId obj1, TiaObjectId obj2)
        {
            return ((obj1.TypeId == obj2.TypeId) && (obj1.InstanceId == obj2.InstanceId));
        }

        public static bool operator !=(TiaObjectId obj1, TiaObjectId obj2)
        {
            return ((obj1.TypeId != obj2.TypeId) || (obj1.InstanceId != obj2.InstanceId));
        }

        public override string ToString()
        {
            return (this.TypeId + "-" + this.InstanceId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ InstanceId.GetHashCode();
                result = (result * 397) ^ TypeId.GetHashCode();
                return result;
            }
        }
    }
}