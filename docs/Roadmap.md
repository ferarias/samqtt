## Features

* Control MQTT connection status.  
  Currently if it disconnects, it will not reconnect.
* Implement more sensors
    * Charging
    * CPU load per processor
    * Media playing
    * Temperatures
    * Network statistics
    * Network interfaces (IP, MAC, gateway, dns...)
    * OS version
    * hostname
* New listeners
    * Get/Set volume
    * Get/Set brightness
    * Lock computer
    * Play media
* Improve some sensors
    * Drive space is in bytes. Try to publish in MB or GB, or even better, publish desired unit.
* Deploy with pipelines a release when deploying to master branch

* Installation via installer msix