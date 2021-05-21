using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAndRecommendProcMon
{
    public class BestFitResult : IComparable
    {
        public string Name { get; set; }
        public string MIStat { get; set; }
        public string InfluencingStat { get; set; }
        public string StatName { get; set; }
        public double MonthlyComputeCost { get; set; }
        public double MonthlyStorageCost { get; set; }
        public double MonthlyLicensingCost { get; set; }
        public double TotalCost { get
            {
                return MonthlyComputeCost + MonthlyStorageCost + MonthlyLicensingCost;
            } 
        }
        public ManagedInstance RecommendedMI { get; set; }

        public bool Recommended { get; set; }

        public int CompareTo(object obj)
        {
            //When Doing sorts, use the contained ManagedInstance Object to sort
            if (obj == null) return 1;

            BestFitResult other = obj as BestFitResult;
            if (other != null)
                return this.RecommendedMI.CompareTo(other.RecommendedMI);
            else
                throw new ArgumentException("Object is not a BestFitResult");
        }
    }
}
