# Contributing to Samqtt

First off, thank you for considering contributing to Samqtt! It's people like you that make Samqtt such a great tool.

This document provides instructions on how to set up your development environment to build and debug Samqtt locally.

## Prerequisites

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [Git](https://git-scm.com/downloads)

## Building and Debugging on Windows

### Using the Command Line

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/your-username/Samqtt.git
    cd Samqtt
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore Samqtt.sln
    ```

3.  **Build the solution:**

    ```bash
    dotnet build Samqtt.sln --configuration Debug
    ```

4.  **Run the application:**

    To run the application, you need to specify the target framework.

    *   **For the Windows-specific version:**

        ```bash
        dotnet run --project src\Samqtt\Samqtt.csproj --framework net8.0-windows8.0
        ```

    *   **For the generic .NET version:**

        ```bash
        dotnet run --project src\Samqtt\Samqtt.csproj --framework net8.0
        ```

### Using Visual Studio 2022

1.  **Open the solution:**

    Open `Samqtt.sln` in Visual Studio 2022.

2.  **Set the startup project:**

    In the Solution Explorer, right-click on the `Samqtt` project and select "Set as Startup Project".

3.  **Select the target framework:**

    In the toolbar, you will see a dropdown to select the target framework.

    *   To debug the Windows-specific version, select `net8.0-windows8.0`.
    *   To debug the generic .NET version, select `net8.0`.

4.  **Start debugging:**

    Press F5 or click the "Start Debugging" button.

### Using Visual Studio Code

1.  **Open the folder:**

    Open the root folder of the repository in Visual Studio Code.

2.  **Install recommended extensions:**

    Visual Studio Code will recommend installing the "C#" extension. Please install it.

3.  **Configure the debugger:**

    Open the `launch.json` file and add the following configurations:

    ```json
    {
        "version": "0.2.0",
        "configurations": [
            {
                "name": ".NET Core Launch (Windows)",
                "type": "coreclr",
                "request": "launch",
                "preLaunchTask": "build",
                "program": "${workspaceFolder}/src/Samqtt/bin/Debug/net8.0-windows8.0/Samqtt.dll",
                "args": [],
                "cwd": "${workspaceFolder}/src/Samqtt",
                "stopAtEntry": false,
                "console": "internalConsole"
            },
            {
                "name": ".NET Core Launch (Generic)",
                "type": "coreclr",
                "request": "launch",
                "preLaunchTask": "build",
                "program": "${workspaceFolder}/src/Samqtt/bin/Debug/net8.0/samqtt.dll",
                "args": [],
                "cwd": "${workspaceFolder}/src/Samqtt",
                "stopAtEntry": false,
                "console": "internalConsole"
            }
        ]
    }
    ```

4.  **Start debugging:**

    Select the desired launch configuration from the Debug panel and press F5.

## Building and Debugging on Linux

### Using the Command Line

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/your-username/Samqtt.git
    cd Samqtt
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore Samqtt.sln
    ```

3.  **Build the solution:**

    ```bash
    dotnet build Samqtt.sln --configuration Debug
    ```

4.  **Run the application:**

    ```bash
    dotnet run --project src/Samqtt/Samqtt.csproj --framework net8.0
    ```

### Using Visual Studio Code

1.  **Open the folder:**

    Open the root folder of the repository in Visual Studio Code.

2.  **Install recommended extensions:**

    Visual Studio Code will recommend installing the "C#" extension. Please install it.

3.  **Configure the debugger:**

    Open the `launch.json` file and add the following configuration:

    ```json
    {
        "version": "0.2.0",
        "configurations": [
            {
                "name": ".NET Core Launch (Linux)",
                "type": "coreclr",
                "request": "launch",
                "preLaunchTask": "build",
                "program": "${workspaceFolder}/src/Samqtt/bin/Debug/net8.0/samqtt.dll",
                "args": [],
                "cwd": "${workspaceFolder}/src/Samqtt",
                "stopAtEntry": false,
                "console": "internalConsole"
            }
        ]
    }
    ```

4.  **Start debugging:**

    Select the ".NET Core Launch (Linux)" launch configuration from the Debug panel and press F5.
