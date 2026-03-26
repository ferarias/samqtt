### Disk drives sensors
 
THIS IS WORK-IN-PROGRESS. DO NOT BELIEVE WHAT YOU SEE HERE.

Status of each mounted drive in the system

`samqtt/{hostname}/drive`

A subtopic with each drive letter, each having the following subtopics:

- `sizetotal`
- `sizefree`
- `percentfree`

Example : `samqtt/lechuck/drive/c/sizetotal` → `13455527`

### FreeMemory sensor

Available physical memory in bytes, read from `/proc/meminfo` (`MemAvailable`) on Linux and `GlobalMemoryStatusEx` on Windows.

`samqtt/{hostname}/sensor/freememory`

Example: `samqtt/lechuck/sensor/freememory` → `4294967296`

**Platforms:** Linux, Windows

### Network sensors

Get network status: `1` if available, else `0`

`samqtt/{hostname}/binary_sensor/network_available`

Example : `samqtt/lechuck/binary_sensor/network_available` → `1`

#### In use

`samqtt/{hostname}/binary_sensor/inuse`: `on` if the system has had some input for the last 30 seconds, else `off`

Example : `samqtt/lechuck/binary_sensor/inuse` → `on`

### CpuProcessorTime sensor

CPU usage as a percentage (0–100), calculated from `/proc/stat` deltas between polls.

`samqtt/{hostname}/sensor/cpuprocessortime`

Example: `samqtt/lechuck/sensor/cpuprocessortime` → `42.5`

**Platforms:** Linux
