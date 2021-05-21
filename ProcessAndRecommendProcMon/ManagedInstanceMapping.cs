using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessAndRecommendProcMon
{
    public class ManagedInstanceMapping
    {
        public List<ManagedInstance> mis;

        public ManagedInstanceMapping()
        {
            if(File.Exists("managedinstancemappings.json"))
            {
                string MIJson = File.ReadAllText("managedinstancemappings.json");
                mis = JsonSerializer.Deserialize<List<ManagedInstance>>(MIJson);
            }
        }

        private static PricingData managedInstancePricing = new PricingData("SQL Managed Instance");

        private List<Pricing> regionalManagedInstancePricing = new List<Pricing>();

        public async Task<List<BestFitResult>> FindBestFitAsync(SQLServerOverview sql)
        {
            List<BestFitResult> bf = new List<BestFitResult>();

            regionalManagedInstancePricing = managedInstancePricing.pricings.Where(i => (i.armRegionName == sql.Region || i.armRegionName == "Global")  && i.type == "Consumption").ToList();

            //Core match
            Task<BestFitResult> coresTask = BestFitAsync(sql, mis => mis.Cores, sql => sql.MaxUsedCores, "Cores");

            // Memory match
            Task<BestFitResult> memoryTask = BestFitAsync(sql, mis => mis.MaxMemory, sql => sql.MemoryAssignedinGB, "Memory");

            // IOPS match
            Task<BestFitResult> iopsTask = BestFitAsync(sql, mis => mis.IOPS, sql => sql.MaxTotalIops, "IOPS");

            // TLog match
            Task<BestFitResult> logTask = BestFitAsync(sql, mis => mis.LogWrite, sql => sql.MaxTlogBandwidthinMB, "LogWrite");

            // Size match
            Task<BestFitResult> sizeTask = BestFitAsync(sql, mis => mis.SizeLimit, sql => sql.TotalSizeinGB, "SizeLimit");

            var results = await Task.WhenAll(coresTask, memoryTask, iopsTask, logTask, sizeTask);
            foreach(var result in results)
            {
                bf.Add(result);
            }

            // Get overall recommended, which will be the "largest" MI to prevent bottlenecks
            bf.Sort();
            var recommendedbf = bf.Last();
            recommendedbf.Recommended = true;

            bf.Reverse();

            return bf;
        }

        private async Task<BestFitResult> BestFitAsync(SQLServerOverview sql, Func<ManagedInstance, float> miProp, Func<SQLServerOverview, float> sqlProp, string PropertyName)
        {
            var mi = mis.Where(m => miProp(m) >= sqlProp(sql) && (m.SSD == sql.SSD || !sql.SSD)).First();
            string mi_family_search;
            if (mi.productName.Contains("Business Critical"))
            {
                mi_family_search = "SQL Managed Instance Business Critical";
            } else
            {
                mi_family_search = "SQL Managed Instance General Purpose";
            }

            int requiredStorage = RoundStorageUp(sql.TotalSizeinGB);
            double compute_cost = regionalManagedInstancePricing.Where(i => i.productName == mi.productName && i.skuName == mi.Cores.ToString() + " vCore").First().retailPrice * 730;
            double storage_cost = regionalManagedInstancePricing.Where(i => i.productName == mi_family_search + " - Storage" && i.meterName == "Data Stored").First().retailPrice * requiredStorage;
            double license_cost = regionalManagedInstancePricing.Where(i => i.productName == mi_family_search + " - SQL License").First().retailPrice * 730 * mi.Cores;

            return new BestFitResult
            {
                Name = mi.Name,
                MIStat = miProp(mi).ToString(),
                InfluencingStat = sqlProp(sql).ToString(),
                StatName = PropertyName,
                RecommendedMI = mi,
                Recommended = false,
                MonthlyComputeCost = compute_cost,
                MonthlyStorageCost = storage_cost,
                MonthlyLicensingCost = license_cost
            };
        }

        private int RoundStorageUp(float TotalStorage)
        {
            //Minus 32 because you get the first 32 GB free
            return (((int)Math.Ceiling(TotalStorage / 32)) * 32) -32;
        }
    }
}
