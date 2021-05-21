using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ProcessAndRecommendProcMon
{
    public class PricingData
    {
        private static readonly HttpClient client = new HttpClient();

        public List<Pricing> pricings = new List<Pricing>();

        public PricingData(string serviceName)
        {
            LoadPricing(serviceName).Wait();
        }

        public async Task LoadPricing(string serviceName)
        {
            await FetchPricingData("https://prices.azure.com/api/retail/prices?$filter=serviceFamily eq 'Databases' and serviceName eq '" + serviceName + "'");
        }

        private async Task FetchPricingData(string url)
        {
            var streamTask = client.GetStringAsync(url);
            var pricingItems = JsonConvert.DeserializeObject<dynamic>(await streamTask);

            foreach(var item in pricingItems.Items)
            {

                pricings.Add(new Pricing()
                {
                    currencyCode = item.currencyCode,
                    tierMinimumUnits = item.tierMinimumUnits,
                    retailPrice = item.retailPrice,
                    unitPrice = item.unitPrice,
                    armRegionName = item.armRegionName,
                    location = item.location,
                    effectiveStartDate = item.effectiveStartDate,
                    meterId = item.meterId,
                    meterName = item.meterName,
                    productId = item.productId,
                    skuId = item.skuId,
                    productName = item.productName,
                    skuName = item.skuName,
                    serviceName = item.serviceName,
                    serviceId = item.serviceId,
                    serviceFamily = item.serviceFamily,
                    unitOfMeasure = item.unitOfMeasure,
                    type = item.type,
                    isPrimaryMeterRegion = item.isPrimaryMeterRegion,
                    armSkuName = item.armSkuName
                }
                );
            }

            if(pricingItems.NextPageLink != null)
            {
                await FetchPricingData(pricingItems.NextPageLink.ToString());
            }
        }

        public static async Task<List<SelectListItem>> GetRegionsSelect()
        {
            List<SelectListItem> regionsList = new List<SelectListItem>();

            //Using Data Stored as the primary select point to get regions that DBMI is available in
            var streamTask = client.GetStringAsync("https://prices.azure.com/api/retail/prices?$filter=serviceFamily eq 'Databases' and serviceName eq 'SQL Managed Instance' and productName eq 'SQL Managed Instance General Purpose - Storage' and meterName eq 'Data Stored'");
            var regionItems = JsonConvert.DeserializeObject<dynamic>(await streamTask);

            foreach(var item in regionItems?.Items)
            {
                if(item.armRegionName == "eastus2")
                {
                    regionsList.Add(new SelectListItem(item.location.ToString(), item.armRegionName.ToString(), true));
                } else
                {
                    regionsList.Add(new SelectListItem(item.location.ToString(), item.armRegionName.ToString()));
                }
            }

            regionsList = regionsList.OrderBy(i => i.Text).ToList();

            return regionsList;
        }
    }
}
