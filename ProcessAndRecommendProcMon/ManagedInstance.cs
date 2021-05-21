using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAndRecommendProcMon
{
    public class ManagedInstance : IComparable
    {
        public string Name { get; set; }
        public float Cores { get; set; }
        public float MaxMemory { get; set; }
        public float IOPS { get; set; }
        public float LogWrite { get; set; }
        public float MaxRequests { get; set; }
        public bool SSD { get; set; }
        public float SizeLimit { get; set; }
        public string productName { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ManagedInstance other = obj as ManagedInstance;
            if(other !=  null)
            {
                if(other.SSD == this.SSD)
                {
                    return this.Cores.CompareTo(other.Cores) +
                        this.MaxMemory.CompareTo(other.MaxMemory) +
                        this.IOPS.CompareTo(other.IOPS) +
                        this.LogWrite.CompareTo(other.LogWrite) +
                        this.MaxRequests.CompareTo(other.MaxRequests) +
                        this.SizeLimit.CompareTo(other.SizeLimit);
                } else
                {
                    // Assume if object specified has SSD (Business Critical) it's "greater" than one without (General Purpose)
                    return other.SSD ? -1 : 1; 
                }
            } else
            {
                throw new ArgumentException("Object is not a ManagedInstance");
            }
        }
    }
}
