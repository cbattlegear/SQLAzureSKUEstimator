using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAndRecommendProcMon
{
    public class SQLProcmonData
    {
        public DateTime Timestamp { get; set; }
        public float BatchesPerSecond { get; set; }
        public float UserConnections { get; set; }
        public float TotalServerMemory { get; set; }
        public float TotalProcessorUsagePercent { get; set; }
        public float DiskReadsPerSecond { get; set; }
        public float DiskWritesPerSecond { get; set; }
        public float LogBytesFlushedPerSecond { get; set; }
        public float TotalSize { get; set; }
    }
}
