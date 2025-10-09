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
Source: "..\..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: ".\README.txt"; DestDir: "{app}"; Flags: isreadme
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
  SummaryLabel: TNewStaticText;

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
    'SAMQTT Configuration',
    'Step 2 of 2: Device Info',
    'Enter device identifier.');

  Page2.Add('Device Identifier:', False);
  
  Page2.Values[0] := ExpandConstant('{computername}');

  // Create summary label on the finished page
  SummaryLabel := TNewStaticText.Create(WizardForm);
  SummaryLabel.Parent := WizardForm.FinishedPage;
  SummaryLabel.AutoSize := False;
  SummaryLabel.Width := WizardForm.FinishedPage.Width - ScaleX(32);
  SummaryLabel.WordWrap := True;
  SummaryLabel.Left := ScaleX(16);
  SummaryLabel.Top := WizardForm.FinishedLabel.Top + WizardForm.FinishedLabel.Height + ScaleY(16);
  SummaryLabel.Height := ScaleY(110);

  // Add the checkbox below the summary
  StartServiceCheckbox := TCheckBox.Create(WizardForm);
  StartServiceCheckbox.Parent := WizardForm.FinishedPage;
  StartServiceCheckbox.Caption := 'Start SAMQTT service after installation';
  StartServiceCheckbox.Checked := True;
  StartServiceCheckbox.Left := ScaleX(16);
  StartServiceCheckbox.Top := SummaryLabel.Top + ScaleY(120); // Leave space for summary text
end;


procedure CurStepChanged(CurStep: TSetupStep);
var
  TemplateFile, ConfigFile, FinalContent: String;
  TemplateContent: AnsiString;
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
    end;
  end;

procedure DeinitializeSetup;
var
  ResultCode: Integer;
begin
  if StartServiceCheckbox.Checked then
  begin
    if Exec('sc.exe', 'start "SAMQTT Service"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      MsgBox('SAMQTT service started successfully.', mbInformation, MB_OK)
    else
      MsgBox('SAMQTT service failed to start! Please check config and start it manually.', mbError, MB_OK);
  end;
end;

procedure CurPageChanged(CurPageID: Integer);
var
  ConfigPath: String;
  SummaryText: String;
  NeedsAuth: Boolean;  
begin
  if CurPageID = wpFinished then
  begin
    NeedsAuth := (Trim(Page1.Values[2]) <> '') or (Trim(Page1.Values[3]) <> '');
    ConfigPath := ExpandConstant('{commonappdata}\SAMQTT\samqtt.appsettings.json');
    if (NeedsAuth = True) then
      SummaryText := 'Installation completed. Your configuration:' + #13#10#13#10 +
        'MQTT Broker: ' + Page1.Values[0] + ':' + Page1.Values[1] + #13#10 +
        'Username: ' + Page1.Values[2] + #13#10 +
        'Device ID: ' + Page2.Values[0] + #13#10#13#10 +
        'Configuration file location:' + #13#10 + ConfigPath
    else
      SummaryText := 'Installation completed. Your configuration:' + #13#10#13#10 +
        'MQTT Broker: ' + Page1.Values[0] + ':' + Page1.Values[1] + #13#10 +
        'Username: (none)' + #13#10 +
        'Device ID: ' + Page2.Values[0] + #13#10#13#10 +
        'Configuration file location:' + #13#10 + ConfigPath;
    
    SummaryLabel.Caption := SummaryText;
  end;
end;
