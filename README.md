# Timbo Tool - Professional Phone Servicing Software (Windows)

Timbo Tool is a native Windows application (C# / WPF) designed for professional phone servicing, featuring real hardware detection and a premium interface.

## 🚀 Features

- **Real Hardware Detection**: Monitors USB and COM ports for connected devices.
- **Professional Dashboard**: Clean, tech-focused interface for FRP, Unlocking, and Repair operations.
- **Static Authentication**: Login with `Barry` / `@123456`.
- **Credit System**: Integrated account credit management (2000 starting credits).
- **Automated Builds**: GitHub Actions automatically generates the `.exe` for every release.

## 💻 Installation (.exe)

1.  **Repository**: [https://github.com/Beusco/timbo](https://github.com/Beusco/timbo)
2.  **Download**: 
    - Go to the **"Actions"** tab on GitHub.
    - Select the latest **"Build Windows Application"** run.
    - Download the **"TimboTool-Windows-App"** artifact from the bottom of the page.
    - Unzip and run `TimboToolApp.exe`.

## 🛠 For Developers

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Build Externally
```bash
dotnet restore
dotnet build
dotnet publish -c Release -r win-x64 --self-contained true
```

## 📄 License
Professional use only. Simulated for demonstration.
