#!/bin/bash

SERVICE_NAME=samqtt
INSTALL_DIR=/opt/$SERVICE_NAME

CONFIG_DIR=/etc/$SERVICE_NAME
SERVICE_FILE=$SERVICE_NAME.service
ENV_FILE=$CONFIG_DIR/env

CONFIG_FILE=$CONFIG_DIR/samqtt.appsettings.json
USER_NAME=$SERVICE_NAME

set -e

echo "Uninstalling $SERVICE_NAME..."

# Stop and disable service
if systemctl list-units --full -all | grep -q "$SERVICE_NAME.service"; then
    sudo systemctl stop $SERVICE_NAME || true
    sudo systemctl disable $SERVICE_NAME || true
    sudo systemctl daemon-reload
    sudo rm -f /etc/systemd/system/$SERVICE_FILE
fi

# Remove installed files and config
sudo rm -rf $INSTALL_DIR
sudo rm -rf $CONFIG_DIR

# Remove user (only if it's not being used elsewhere)
if id "$USER_NAME" &>/dev/null; then
    echo "Removing user $USER_NAME..."
    sudo userdel --force "$USER_NAME" || true
fi

echo "Service '$SERVICE_NAME' uninstalled."
