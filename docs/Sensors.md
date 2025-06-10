### Disk drives sensors
 
THIS IS WORK-IN-PROGRESS. DO NOT BELIEVE WHAT YOU SEE HERE.

Status of each mounted drive in the system

`samqtt/{hostname}/drive`

A subtopic with each drive letter, each having the following subtopics:

- `sizetotal`
- `sizefree`
- `percentfree`

Example : `samqtt/lechuck/drive/c/sizetotal` → `13455527`

### Memory sensors

Returns available memory in Megabytes

`samqtt/{hostname}/freememory `

Example : `samqtt/lechuck/freememory` → `234`

### Network sensors

Get network status: `1` if available, else `0`

`samqtt/{hostname}/binary_sensor/network_available`

Example : `samqtt/lechuck/binary_sensor/network_available` → `1`

#### In use

`samqtt/{hostname}/binary_sensor/inuse`: `on` if the system has had some input for the last 30 seconds, else `off`

Example : `samqtt/lechuck/binary_sensor/inuse` → `on`

### Cpu sensors

`samqtt/{hostname}/cpuprocessortime` (returns string 0-100%)

Example : `samqtt/lechuck/cpuprocessortime` →  `50`
