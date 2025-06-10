## Setup

### Microsoft Windows

Install as a **Windows Service**

First, build a a self-containing exe:

```powershell
dotnet publish .\src\Samqtt\ --configuration Release --framework net8.0-windows8.0 --runtime win-x64 --self-contained  --output c:\SAMQTT
```

Then create and start the service (you will need Administration privileges):

```powershell
sc.exe create "SAMQTT Service" binpath= "C:\SAMQTT\Samqtt.exe"
sc.exe start "SAMQTT Service"
```

More information in [this article](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)

### Linux

Install as a **Linux Service**

First, build a a self-containing exe:

```bash
dotnet publish ./src/samqtt \
--configuration Release --framework net8.0 --runtime linux-x64 \
--self-contained  --output /opt/samqtt
```

Then create and install as a Linux Service

### Install as a Linux Service (systemd)

To run `samqtt` as a background service (daemon) on Linux, you can use systemd. Follow these steps:
   
Adjust the output path as needed.

2. **Create a systemd service file**

   Create a file at `/etc/systemd/system/samqtt.service` with the following content (edit paths if necessary):
   
   ```ini
   [Unit]
   Description=SAMQTT Service
   After=network.target

   [Service]
   Type=simple
   WorkingDirectory=/opt/samqtt
   ExecStart=/usr/bin/dotnet /opt/samqtt/samqtt.dll
   Restart=on-failure
   User=samqtt
   Environment=ASPNETCORE_ENVIRONMENT=Production

   [Install]
   WantedBy=multi-user.target
   ```

3. **Create the service user (optional but recommended)**

   ```bash
   sudo useradd --system --no-create-home --group samqtt
   sudo chown -R samqtt:samqtt /opt/samqtt
   ```

4. **Reload systemd and enable the service**

   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable samqtt
   sudo systemctl start samqtt
   ```

5. **Check the service status**

   ```bash
   sudo systemctl status samqtt
   ```

`samqtt` will now run as a background service and start automatically on boot.