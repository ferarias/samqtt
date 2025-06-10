#ifndef MyAppVersion
  #define MyAppVersion "0.0.0"
#endif

[Setup]
AppName=SAMQTT
AppVersion={#MyAppVersion}
DefaultDirName={commonpf}\SAMQTT
DefaultGroupName=SAMQTT
OutputDir=..\..\publish\setup
OutputBaseFilename=SamqttSetup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

WizardStyle=modern
SetupIconFile=samqtt.ico
WizardImageFile=wizard.bmp
WizardSmallImageFile=header.bmp

[Dirs]
Name: "{commonappdata}\SAMQTT"

[Files]
Source: "..\..\publish\windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: ".\README.md"; DestDir: "{app}"; Flags: isreadme
Source: ".\samqtt.png"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\samqtt.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\samqtt.appsettings.template.json"; DestDir: "{commonappdata}\SAMQTT"; Flags: ignoreversion

[Icons]
Name: "{group}\SAMQTT Configuration file"; Filename: "{commonappdata}\SAMQTT\samqtt.appsettings.json"
Name: "{group}\SAMQTT Readme"; Filename: "{app}\README.md"

[Run]
Filename: "sc.exe"; Parameters: "create ""SAMQTT Service"" binPath=""{app}\Samqtt.exe"" start=auto"; Description: "Create Samqtt service"; Flags: runhidden

[UninstallRun]
Filename: "sc.exe"; Parameters: "stop ""SAMQTT Service"" "; RunOnceId: "SamqttStop"
Filename: "sc.exe"; Parameters: "delete ""SAMQTT Service"" "; RunOnceId: "SamqttUninstall"

[Code]
var
  Page1, Page2: TInputQueryWizardPage;
  MQTTServer: AnsiString;
  MQTTPort: AnsiString;
  MQTTUsername: AnsiString;
  MQTTPassword: AnsiString;
  DeviceIdentifier: AnsiString;
  StartServiceCheckbox: TCheckBox;

procedure InitializeWizard;
begin
  Page1 := CreateInputQueryPage(wpWelcome,
    'Samqtt Configuration',
    'Step 1 of 2: MQTT Connection',
    'Enter the MQTT broker settings.');

  Page1.Add('MQTT Broker hostname:', False);
  Page1.Add('MQTT Broker port:', False);
  Page1.Add('MQTT Username (optional):', False);
  Page1.Add('MQTT Password (optional):', True);
  
  Page1.Values[0] := 'localhost';
  Page1.Values[1] := '1883';
  Page1.Values[2] := '';
  Page1.Values[3] := '';
  
  Page2 := CreateInputQueryPage(Page1.ID,
    'Samqtt Configuration',
    'Step 2 of 2: Device Info',
    'Enter device identifier.');

  Page2.Add('Device Identifier:', False);
  
  Page2.Values[0] := ExpandConstant('{computername}');

  StartServiceCheckbox := TCheckBox.Create(WizardForm);
  StartServiceCheckbox.Parent := WizardForm.FinishedPage;
  StartServiceCheckbox.Caption := 'Start SAMQTT service after installation';
  StartServiceCheckbox.Checked := True;
  StartServiceCheckbox.Left := ScaleX(ScaleX(0) + 16);
  StartServiceCheckbox.Top := WizardForm.FinishedLabel.Top + WizardForm.FinishedLabel.Height + ScaleY(16);
end;


procedure CurStepChanged(CurStep: TSetupStep);
var
  TemplateFile, ConfigFile, FinalContent: String;
  TemplateContent: AnsiString;
  NeedsAuth: Boolean;
  ResultCode: Integer;

begin
  if CurStep = ssPostInstall then
  begin
    MQTTServer := Page1.Values[0];
    MQTTPort := Page1.Values[1];
    MQTTUsername := Page1.Values[2];
    MQTTPassword := Page1.Values[3];
    DeviceIdentifier := Page2.Values[0];
    
    TemplateFile := ExpandConstant('{commonappdata}\SAMQTT\samqtt.appsettings.template.json');
    ConfigFile := ExpandConstant('{commonappdata}\SAMQTT\samqtt.appsettings.json');

    if LoadStringFromFile(TemplateFile, TemplateContent) then
    begin
      NeedsAuth := (Trim(MQTTUsername) <> '') or (Trim(MQTTPassword) <> '');

      FinalContent := TemplateContent;
      StringChangeEx(FinalContent, '{{MQTT_SERVER}}', MQTTServer, True);
      StringChangeEx(FinalContent, '{{MQTT_PORT}}', MQTTPort, True);
      StringChangeEx(FinalContent, '{{MQTT_USERNAME}}', MQTTUsername, True);
      StringChangeEx(FinalContent, '{{MQTT_PASSWORD}}', MQTTPassword, True);
      StringChangeEx(FinalContent, '{{DEVICE_IDENTIFIER}}', DeviceIdentifier, True);


      if not SaveStringToFile(ConfigFile, FinalContent, False) then
        MsgBox('Failed to write config file to: ' + ConfigFile, mbError, MB_OK);
    end
    else
      MsgBox('Failed to read template config file from: ' + TemplateFile, mbError, MB_OK);

    if StartServiceCheckbox.Checked then
    begin
      if Exec('sc.exe', 'start "SAMQTT Service"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
        MsgBox('The SAMQTT service has been started successfully.', mbInformation, MB_OK)
      else
        MsgBox('Failed to start SAMQTT service. Please start it manually.', mbError, MB_OK);
    end;
  end;
end;
