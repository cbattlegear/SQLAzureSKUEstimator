using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAndRecommendProcMon
{
    public class ProcMonChartData
    {
        public List<DateTime> labels { get; set; }
        public List<ChartDataset> datasets { get; set; }

        public ProcMonChartData(string label)
        {
            datasets = new List<ChartDataset>();
            datasets.Add(new ChartDataset() { label = label });
            labels = new List<DateTime>();
        }
    }

    public class ChartDataset
    {
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public List<float> data { get; set; }
        public ChartDataset()
        {
            Random r = new Random();
            int red = r.Next(256);
            int green = r.Next(256);
            int blue = r.Next(256);
            data = new List<float>();

            backgroundColor = "rgba(" + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ", 0.2)";
            borderColor = "rgba(" + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ", 1)";
        }
    }
}
