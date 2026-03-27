# SAMQTT - Linux Install Instructions

## Quick install (latest release)

```bash
wget https://raw.githubusercontent.com/ferarias/samqtt/master/setup/install-latest-linux.sh
chmod +x install-latest-linux.sh
./install-latest-linux.sh
```

## Manual install

**Download and extract** the release (replace `<version>` with the desired release tag):

```bash
wget https://github.com/ferarias/samqtt/releases/download/<version>/samqtt-linux-x64.tar.gz
tar -xzf samqtt-linux-x64.tar.gz
```

**Run the installer:**

```bash
cd samqtt
chmod +x install.sh uninstall.sh
./install.sh
```

## To uninstall:

```bash
cd samqtt
./uninstall.sh
```
