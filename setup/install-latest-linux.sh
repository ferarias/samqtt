#!/usr/bin/env bash
set -e

OWNER="ferarias"
REPO="samqtt"
INSTALL_SCRIPT="install.sh"
UNINSTALL_SCRIPT="uninstall.sh"

# Detect architecture
case "$(uname -m)" in
  x86_64|amd64) ARCH="x64";;
  arm64|aarch64) ARCH="arm64";;
  armv7l) ARCH="armv7";;
  i386|i686) ARCH="x86";;
  *) ARCH="$(uname -m)";;
esac

echo "Architecture: $ARCH"

# Get correct URL
API_URL="https://api.github.com/repos/${OWNER}/${REPO}/releases/latest"
ASSET_URL=$(curl -s "$API_URL" \
  | grep "browser_download_url" \
  | grep -i "linux" \
  | grep -i "$ARCH" \
  | cut -d '"' -f 4 \
  | head -n 1)

# Fallback
if [ -z "$ASSET_URL" ]; then
  echo "No assets for linux-$ARCH, using linux generic..."
  ASSET_URL=$(curl -s "$API_URL" \
    | grep "browser_download_url" \
    | grep -i "linux" \
    | cut -d '"' -f 4 \
    | head -n 1)
fi

if [ -z "$ASSET_URL" ]; then
  echo "No appropriate linux asset." >&2
  exit 1
fi

FILE_NAME=$(basename "$ASSET_URL")
echo "Downloading $FILE_NAME from $ASSET_URL"

if command -v wget >/dev/null 2>&1; then
  wget -q --show-progress "$ASSET_URL" -O "$FILE_NAME"
elif command -v curl >/dev/null 2>&1; then
  curl -L "$ASSET_URL" -o "$FILE_NAME"
else
  echo "wget/curl required." >&2
  exit 1
fi

# Extract the tar.gz file
tar xvfz "$FILE_NAME"

# Move into the extracted directory and run INSTALL_SCRIPT
cd samqtt
chmod +x "$INSTALL_SCRIPT"
chmod +x "$UNINSTALL_SCRIPT"
./"$INSTALL_SCRIPT"
