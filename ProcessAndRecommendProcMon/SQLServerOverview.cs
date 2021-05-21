using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAndRecommendProcMon
{
    public class SQLServerOverview
    {
        public List<SQLProcmonData> procmon = new List<SQLProcmonData>();
        public bool SSD { get; set; }
        public float CoreCount { get; set; }
        public float MaxUsedCores { get; set; }
        public float MaxTotalIops { get; set; }
        public float MemoryAssignedinGB { get; set; }
        public float MaxTlogBandwidthinMB { get; set; }
        public float MaxConnections { get; set; }
        public float MaxRequests { get; set; }
        public float TotalSizeinGB { get; set; }
        public string Region { get; set; }

        public Dictionary<string, ProcMonChartData> GenerateProcmonChartData()
        {
            Dictionary<string, ProcMonChartData> procmonchartdata = new Dictionary<string, ProcMonChartData>();

            procmonchartdata.Add("RequestsAndConnections", new ProcMonChartData("Requests"));
            procmonchartdata["RequestsAndConnections"].datasets.Add(new ChartDataset() { label = "Connections" });

            procmonchartdata.Add("MemoryUsage", new ProcMonChartData("Memory Usage"));
            procmonchartdata.Add("CPUUsage", new ProcMonChartData("CPU Usage"));

            procmonchartdata.Add("TotalIOPS", new ProcMonChartData("Total IOPS"));
            procmonchartdata["TotalIOPS"].datasets.Add(new ChartDataset() { label = "Read IOPS" });
            procmonchartdata["TotalIOPS"].datasets.Add(new ChartDataset() { label = "Write IOPS" });
            
            procmonchartdata.Add("TLogBandwidth", new ProcMonChartData("TLog Bandwidth"));
            procmonchartdata.Add("TotalSizeofDatabases", new ProcMonChartData("Total Size of Databases"));

            foreach (SQLProcmonData proc in procmon)
            {
                procmonchartdata["RequestsAndConnections"].labels.Add(proc.Timestamp);
                procmonchartdata["RequestsAndConnections"].datasets[0].data.Add(proc.BatchesPerSecond);
                procmonchartdata["RequestsAndConnections"].datasets[1].data.Add(proc.UserConnections);

                procmonchartdata["MemoryUsage"].labels.Add(proc.Timestamp);
                procmonchartdata["MemoryUsage"].datasets[0].data.Add(proc.TotalServerMemory / 1000 / 1000);

                procmonchartdata["CPUUsage"].labels.Add(proc.Timestamp);
                procmonchartdata["CPUUsage"].datasets[0].data.Add(proc.TotalProcessorUsagePercent);

                procmonchartdata["TotalIOPS"].labels.Add(proc.Timestamp);
                procmonchartdata["TotalIOPS"].datasets[0].data.Add(proc.DiskReadsPerSecond + proc.DiskWritesPerSecond);
                procmonchartdata["TotalIOPS"].datasets[1].data.Add(proc.DiskReadsPerSecond);
                procmonchartdata["TotalIOPS"].datasets[2].data.Add(proc.DiskWritesPerSecond);

                procmonchartdata["TLogBandwidth"].labels.Add(proc.Timestamp);
                procmonchartdata["TLogBandwidth"].datasets[0].data.Add(proc.LogBytesFlushedPerSecond / 1000 / 1000);

                procmonchartdata["TotalSizeofDatabases"].labels.Add(proc.Timestamp);
                procmonchartdata["TotalSizeofDatabases"].datasets[0].data.Add(proc.TotalSize / 1000 / 1000);
            }

            return procmonchartdata;
        }

        public void ProcessCounters()
        {
            ProcessCPUUsage();
            ProcessMaxIops();
            ProcessMemoryAssigned();
            ProcessTLogBandwidth();
            ProcessMaxConnections();
            ProcessMaxRequests();
            ProcessTotalSize();
        }

        private void ProcessCPUUsage()
        {
            MaxUsedCores = CoreCount * (procmon.Max(cpu => cpu.TotalProcessorUsagePercent)/100);
        }

        private void ProcessMaxIops()
        {
            MaxTotalIops = procmon.Max(io => io.DiskReadsPerSecond + io.DiskWritesPerSecond);
        }

        private void ProcessMemoryAssigned()
        {
            // Convert to GB from KB
            MemoryAssignedinGB = procmon.Max(mem => mem.TotalServerMemory) / 1000 / 1000;
        }

        private void ProcessTLogBandwidth()
        {
            // Convert to MB from Bytes
            MaxTlogBandwidthinMB = procmon.Max(log => log.LogBytesFlushedPerSecond) / 1000 / 1000;
        }

        private void ProcessMaxConnections()
        {
            MaxConnections = procmon.Max(conn => conn.UserConnections);
        }

        private void ProcessMaxRequests()
        {
            MaxRequests = Convert.ToInt32(procmon.Max(conn => conn.BatchesPerSecond));
        }

        private void ProcessTotalSize()
        {
            // Convert to GB from KB
            TotalSizeinGB = procmon.Max(size => size.TotalSize) / 1000 / 1000;
        }

    }
}
