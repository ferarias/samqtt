#!/bin/bash

SERVICE_NAME=samqtt
INSTALL_DIR=/opt/$SERVICE_NAME

CONFIG_DIR=/etc/$SERVICE_NAME
SERVICE_FILE=$SERVICE_NAME.service
ENV_FILE=$CONFIG_DIR/env

CONFIG_FILE=$CONFIG_DIR/samqtt.appsettings.json
TEMPLATE_FILE=samqtt.appsettings.template.json
USER_NAME=$SERVICE_NAME

set -e

echo "Installing $SERVICE_NAME..."

echo "Enter the MQTT broker settings."
read -p "MQTT server [localhost]: " MQTT_SERVER
MQTT_SERVER=${MQTT_SERVER:-localhost}

read -p "MQTT port [1883]: " MQTT_PORT
MQTT_PORT=${MQTT_PORT:-1883}

read -p "MQTT username [empty]: " MQTT_USERNAME
MQTT_USERNAME=${MQTT_USERNAME}

read -p "MQTT password [empty]: " MQTT_PASSWORD
MQTT_PASSWORD=${MQTT_PASSWORD}

echo "Enter the device identifier."
DEFAULT_IDENTIFIER=$(hostname)
read -p "Device Identifier [$DEFAULT_IDENTIFIER]: " DEVICE_IDENTIFIER
DEVICE_IDENTIFIER=${DEVICE_IDENTIFIER:-$DEFAULT_IDENTIFIER}

# Create target install dir
sudo mkdir -p "$INSTALL_DIR"
sudo cp -r ./* "$INSTALL_DIR"

# Create systemd service
sudo cp "$SERVICE_FILE" "/etc/systemd/system/$SERVICE_FILE"

# Create config dir
sudo mkdir -p "$CONFIG_DIR"

# Generate appsettings.json from template
echo "Generating config at $CONFIG_FILE..."
sed -e "s|{{MQTT_SERVER}}|$MQTT_SERVER|g" \
    -e "s|{{MQTT_PORT}}|$MQTT_PORT|g" \
    -e "s|{{MQTT_USERNAME}}|$MQTT_USERNAME|g" \
    -e "s|{{MQTT_PASSWORD}}|$MQTT_PASSWORD|g" \
    -e "s|{{DEVICE_IDENTIFIER}}|$DEVICE_IDENTIFIER|g" \
    "$TEMPLATE_FILE" | sudo tee "$CONFIG_FILE" > /dev/null

# Create env file if missing
if [ ! -f "$ENV_FILE" ]; then
    echo "ASPNETCORE_ENVIRONMENT=Production" | sudo tee "$ENV_FILE" > /dev/null
fi

# Create user if not exists
if ! id "$USER_NAME" &>/dev/null; then
    sudo useradd --system --no-create-home --shell /usr/sbin/nologin "$USER_NAME"
fi

# Set permissions
sudo chown -R "$USER_NAME:$USER_NAME" "$INSTALL_DIR"
sudo chown -R "$USER_NAME:$USER_NAME" "$CONFIG_DIR"

# Reload and start service
sudo systemctl daemon-reexec
sudo systemctl daemon-reload
sudo systemctl enable "$SERVICE_NAME"
sudo systemctl restart "$SERVICE_NAME"

echo "✅ Service '$SERVICE_NAME' installed and started."
echo "📝 Configuration saved to: $CONFIG_FILE"
echo "📢 To view logs: journalctl -u $SERVICE_NAME -f"
