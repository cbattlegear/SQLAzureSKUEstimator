Param(
[int]$Runtime = 86400 # Runtime in seconds, defaults to 24 hours
)

$output = logman query SQLAzurePerfCounters

if(!($output.Indexof("Error"))) {
    & logman remove "SQLAzurePerfCounters"
}

& logman create counter -si 10 -rf $Runtime -c "\SQLServer:SQL Statistics\Batch Requests/sec" "\SQLServer:General Statistics\User Connections" "\SQLServer:Memory Manager\Target Server Memory (KB)" "\SQLServer:Memory Manager\Total Server Memory (KB)" "\Processor(*)\% Processor Time" "\LogicalDisk(_Total)\Disk Reads/sec" "\LogicalDisk(_Total)\Disk Writes/sec" "\SQLServer:Databases(_Total)\Log Bytes Flushed/sec" -f csv -n "SQLAzurePerfCounters" -o "SQLAzurePerfCounters.csv" -ow

$output_second = logman query SQLAzurePerfCounters

if($output_second.Indexof("Error")) {
    Write-Error "SQLAzurePerfCounters procmon capture could not be created, please run as an Administrator"
} else {
    & logman start "SQLAzurePerfCounters"
}

