using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using CsvHelper;
using System.Threading.Tasks;

namespace ProcessAndRecommendProcMon
{
    public class ProcessCSV
    {
        public static async Task<SQLServerOverview> ProcessAsync(StreamReader source_file)
        {
            SQLServerOverview sql = new SQLServerOverview();
            using (MemoryStream procmon_data = new MemoryStream())
            {
                using (var mem_write = new StreamWriter(procmon_data))
                {
                    bool first_line = true;
                    while (source_file.Peek() >= 0)
                    {
                        if (first_line)
                        {

                            string header = Regex.Replace(source_file.ReadLine(), @"\\\\(.*?)\\", "\\", RegexOptions.IgnoreCase);
                            // Minus 2 because of 1 extra occurance for total and 1 extra item when splitting
                            sql.CoreCount = header.Split("% Processor Time").Length - 2;
                            await mem_write.WriteLineAsync(header);
                            first_line = false;

                            //Skip second row as it usually has no valid data.
                            await source_file.ReadLineAsync();
                        }
                        else
                        {
                            await mem_write.WriteLineAsync(source_file.ReadLine());
                        }
                    }
                    mem_write.Flush();
                    procmon_data.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(procmon_data))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        await csv.ReadAsync();
                        csv.ReadHeader();
                        while (await csv.ReadAsync())
                        {
                            SQLProcmonData procmon = new SQLProcmonData
                            {
                                Timestamp = csv.GetField<DateTime>(0),
                                BatchesPerSecond = csv.GetField<float>("\\SQLServer:SQL Statistics\\Batch Requests/sec"),
                                UserConnections = csv.GetField<int>("\\SQLServer:General Statistics\\User Connections"),
                                TotalServerMemory = csv.GetField<float>("\\SQLServer:Memory Manager\\Total Server Memory (KB)"),
                                TotalProcessorUsagePercent = csv.GetField<float>("\\Processor(_Total)\\% Processor Time"),
                                DiskReadsPerSecond = csv.GetField<float>("\\LogicalDisk(_Total)\\Disk Reads/sec"),
                                DiskWritesPerSecond = csv.GetField<float>("\\LogicalDisk(_Total)\\Disk Writes/sec"),
                                LogBytesFlushedPerSecond = csv.GetField<float>("\\SQLServer:Databases(_Total)\\Log Bytes Flushed/sec"),
                                TotalSize = csv.GetField<float>("\\SQLServer:Databases(_Total)\\Data File(s) Size (KB)")
                            };
                            sql.procmon.Add(procmon);
                        }
                    }
                }
            }
            return sql;
        }
    }
}
