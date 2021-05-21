using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessAndRecommendProcMon;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SQLAzureSKUEstimator.Pages
{
    public class ManagedInstanceModel : PageModel
    {
        private IWebHostEnvironment _environment;
        public ManagedInstanceModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public bool SSD { get; set; }

        [BindProperty]
        public string Region { get; set; }

        public List<SelectListItem> Regions { get; } = PricingData.GetRegionsSelect().Result;

        public List<BestFitResult> bf;

        public SQLServerOverview sql;

        public string procmonchartdata;

        [BindProperty]
        public IFormFile Upload { get; set; }
        
        public async Task OnPostAsync()
        {

            //await Upload.CopyToAsync(fileStream);
            using (var source_file = new StreamReader(Upload.OpenReadStream()))
            {
                sql = await ProcessCSV.ProcessAsync(source_file);

                sql.SSD = SSD;
                sql.Region = Region;
                sql.ProcessCounters();

                ManagedInstanceMapping mi = new ManagedInstanceMapping();

                bf = await mi.FindBestFitAsync(sql);
                var procmon_chart_obj = sql.GenerateProcmonChartData();
                procmonchartdata = JsonSerializer.Serialize(procmon_chart_obj);
            }

        }

    }
}
