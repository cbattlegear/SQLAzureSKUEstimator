# SQL Azure Sizing Estimator

## The Goal

Sizing PaaS Solutions in Azure can be difficult. If you do not know the specific items to look at and how to map them 
you can get stuck with an improperly sized database or unexpected costs.

The goal of the tool is to use a lightweight capture to help you pick the best possible solution for your database in Azure.

## The Process

Currently we are mapping specific PerfMon counters to Azure SQL Managed Instance limits. In the future we will work to use other counters
for other Azure PaaS Database technologies.

### SQL Azure Managed Instance Mapping

**\SQLServer:SQL Statistics\Batch Requests/sec**  
Mapped to Max Workers


**\SQLServer:General Statistics\User Connections**   
Mapped to Max Logins

**\SQLServer:Memory Manager\Target Server Memory (KB)**  
**\SQLServer:Memory Manager\Total Server Memory (KB)**  
Mapped to Assigned Memory

**\Processor(*)\% Processor Time**  
Mapped to number of Cores and Cores used

**\LogicalDisk(_Total)\Disk Reads/sec**  
**\LogicalDisk(_Total)\Disk Writes/sec**  
Mapped to Total IOPS

**\SQLServer:Databases(_Total)\Log Bytes Flushed/sec**  
Mapped to TLog Bandwidth