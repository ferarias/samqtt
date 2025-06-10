## MQTT listeners

THIS IS WORK-IN-PROGRESS. DO NOT BELIEVE WHAT YOU SEE HERE.

### Monitor

Topic: `samqtt/{hostname}/monitor/set`
Value: `0` or `1`

Response:
Published to `samqtt/{hostname}/monitor` after setting

### Power state operations

Tries to put system into different power states

**Hibernate**
Tries to put system into a hibernation power mode

Topic: `samqtt/{hostname}/hibernate`

**Suspend**
Tries to put system into a suspended power mode

Topic: `samqtt/{hostname}/suspend `

**Shutdown**
Shutdown immediately the computer

Topic: `samqtt/{hostname}/shutdown`

**Reboot**
Reboot immediatly the computer

Topic: `samqtt/{hostname}/reboot`

### Send Message

Shows a popup notification message on the Windows computer.

Topic: `samqtt/{hostname}/sendmessage`
Value: the following JSON payload is expected:

```json
{ 
	"Lines": [],
	"Image": "{image}"
}
```
`Lines` is a list of text lines, such as `["Hello", "This is a message"]`
`Image` is an optional path to an image file (must exist)

Examples: 

Topic: `samqtt/{hostname}/sendmessage`
Value:
```json
{ 
	"Lines": ["Hello from Home Assistant!"],
	"Image": "{image}"
}
```

If you want to add an image to the message, make sure the image file exists in the Windows filesystem.

Topic: `samqtt/{hostname}/sendmessage`
Value:
```json
{ 
	"Lines": ["Home Assistant", "Text 1", "Text 2"],
	"Image": "C:\\Windows\\System32\\ComputerToastIcon.contrast-white.png"
}
```

### Running processes

Topic: `samqtt/{hostname}/process/running`
Message: `{appname}`

`appname` is the name of the exe to check

Return

Topic: `samqtt/{hostname}/process/running/{appname}`
Value:
* 0 = Not running
* 1 = Found running

Example:

Topic: `samqtt/{hostname}/process/running`
Message:
```
pdf24.exe
```

Returns:
Topic: `samqtt/{hostname}/process/running/pdf24.exe`
Message:
```
1
```

### Execute

Executes the provided command in the system

Topic: `samqtt/{hostname}/exec`

Value: the following payload is expected:

```json
{ 
	"CommandString": "{command}",
	"WindowStyle": "{style}",
	"ExecParameters": "{commandParams}",
	"MonitorId": ""
}
```
* `command` is the command name; eg `"dotnet"`
* `style` can be any one of:

  * 0 = Normal
  * 1 = Hidden
  * 2 = Minimized
  * 3 = Maximized

* `commandParams` is the parameters for the command; eg `"build c:/dev/src/myproject"`
