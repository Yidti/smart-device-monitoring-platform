# 開發日誌

本文檔記錄了「智慧設備監控與預警平台」專案開發過程中按時間順序執行的步驟、命令和重要決策。

## 2025年10月22日 星期三

### 1. 專案初始化

**目標：** 初始化 ASP.NET MVC Web 應用程式和 .NET Console 應用程式的專案結構，並設定版本控制。

*   **動作：** 建立 `src` 目錄以及 Web 和 Console 專案的子目錄。
    *   **命令：** `mkdir -p src/SmartDeviceMonitoring.Web src/SmartDeviceMonitoring.ConsoleApp`
    *   **結果：** 目錄成功建立。

*   **動作：** 建立新的 ASP.NET Core MVC 專案。
    *   **命令：** `dotnet new mvc -n SmartDeviceMonitoring.Web -o src/SmartDeviceMonitoring.Web`
    *   **結果：** ASP.NET Core MVC 專案 `SmartDeviceMonitoring.Web` 成功建立。

*   **動作：** 建立新的 .NET Console 應用程式專案。
    *   **命令：** `dotnet new console -n SmartDeviceMonitoring.ConsoleApp -o src/SmartDeviceMonitoring.ConsoleApp`
    *   **結果：** .NET Console 應用程式專案 `SmartDeviceMonitoring.ConsoleApp` 成功建立。

*   **動作：** 在 `GEMINI.md` 中記錄專案資料夾結構。
    *   **結果：** `GEMINI.md` 已更新，包含資料夾結構圖。

*   **動作：** 為 .NET 專案建立 `.gitignore` 檔案。
    *   **命令：** `write_file` (內容包含標準 .NET 忽略模式)
    *   **結果：** `.gitignore` 成功建立。

*   **動作：** 在 `GEMINI.md` 和 `README.md` 中將「初始化專案結構」任務標記為完成。
    *   **結果：** 核取方塊從 `[ ]` 更新為 `[x]`。

*   **動作：** 提交初始專案設定、文件和 `.gitignore`。
    *   **命令：** `git add GEMINI.md README.md src/ .gitignore`
    *   **命令：** `git commit -m "feat: Initialize project structure, document schema, and add .gitignore"`
    *   **結果：** 變更已提交到本地儲存庫。

*   **動作：** 將提交的變更推送到遠端儲存庫。
    *   **命令：** `git push`
    *   **結果：** 變更成功推送。

### 2. 資料庫設計與設定 (Entity Framework Core)

**目標：** 使用 Entity Framework Core 模型定義資料庫架構，配置資料庫上下文，並使用遷移建立資料庫。

*   **動作：** 在 `GEMINI.md` 中記錄詳細的資料庫架構計劃（包括 Mermaid ERD）。
    *   **結果：** `GEMINI.md` 已更新，包含資料庫架構和 ERD。

*   **動作：** 根據架構計劃在 `src/SmartDeviceMonitoring.Web/Models` 中建立 C# 模型類別 (`Device.cs`, `SensorType.cs`, `Sensor.cs`, `SensorData.cs`, `Alert.cs`)。
    *   **結果：** 模型檔案成功建立。

*   **動作：** 由於 NuGet 套件相容性問題，將 `SmartDeviceMonitoring.Web` 專案的目標框架升級到 .NET 8.0。
    *   **命令：** `replace` 在 `src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj` 中（將 `<TargetFramework>net7.0</TargetFramework>` 更改為 `<TargetFramework>net8.0</TargetFramework>`）
    *   **故障排除：** 遇到 `NETSDK1045` 錯誤（SDK 不支援 .NET 8.0）。指示用戶安裝 .NET 8.0 SDK。
    *   **結果：** 用戶確認 .NET 8.0 SDK 安裝。專案目標框架已更新。

*   **動作：** 將必要的 Entity Framework Core 和配置 NuGet 套件新增到 `SmartDeviceMonitoring.Web` 專案。
    *   **命令：**
        *   `dotnet add src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj package Microsoft.EntityFrameworkCore.SqlServer`
        *   `dotnet add src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj package Microsoft.EntityFrameworkCore.Tools`
        *   `dotnet add src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj package Microsoft.EntityFrameworkCore.Design`
        *   `dotnet add src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj package Microsoft.Extensions.Configuration.Json`
        *   `dotnet add src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj package Microsoft.Extensions.Configuration.EnvironmentVariables`
    *   **結果：** 套件成功新增。

*   **動作：** 建立 `src/SmartDeviceMonitoring.Web/Data/ApplicationDbContext.cs` 以定義資料庫上下文。
    *   **結果：** `ApplicationDbContext.cs` 建立。

*   **動作：** 使用佔位符 SQL Server 連接字串更新 `src/SmartDeviceMonitoring.Web/appsettings.json`。
    *   **結果：** `appsettings.json` 已更新。

*   **動作：** 修改 `src/SmartDeviceMonitoring.Web/Program.cs` 以向依賴注入容器註冊 `ApplicationDbContext`。
    *   **結果：** `Program.cs` 已更新，包含 `DbContext` 註冊和 `using` 語句。

*   **動作：** 安裝 `dotnet-ef` 作為全域工具並將其新增到當前會話的 PATH。
    *   **命令：** `dotnet tool install --global dotnet-ef`
    *   **命令：** `export PATH="$PATH:/Users/rekam/.dotnet/tools"`
    *   **結果：** `dotnet-ef` 工具可用。

*   **動作：** 在 `src/SmartDeviceMonitoring.Web/Data/ApplicationDbContextFactory.cs` 中實作 `IDesignTimeDbContextFactory<ApplicationDbContext>`，以啟用 EF Core 遷移的設計時操作。
    *   **故障排除：** 最初在設計時遇到 `ConfigurationBuilder` 無法找到 `appsettings.json` 的問題。嘗試了多種修復方法，包括明確路徑和硬編碼連接字串以進行偵錯。
    *   **結果：** `ApplicationDbContextFactory.cs` 已實作並改進，以正確解析配置。

*   **動作：** 設定 SQL Server Docker 容器，因為 macOS 不支援 LocalDB。
    *   **命令：** `docker pull mcr.microsoft.com/mssql/server:2022-latest`
    *   **命令：** `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password123" -p 1433:1433 --name sql_server_db -d mcr.microsoft.com/mssql/server:2022-latest`
    *   **結果：** SQL Server Docker 容器正在運行。（注意：根據用戶要求，`YourStrong@Password123` 用作臨時密碼）。

*   **動作：** 使用 Docker 化 SQL Server 的連接字串更新 `src/SmartDeviceMonitoring.Web/appsettings.json`。
    *   **結果：** `appsettings.json` 已更新。

*   **動作：** 建立初始 Entity Framework Core 遷移。
    *   **命令：** `dotnet ef migrations add InitialCreate`
    *   **結果：** 遷移 `InitialCreate` 成功建立。

*   **動作：** 應用初始遷移以在 SQL Server Docker 容器中建立資料庫。
    *   **命令：** `dotnet ef database update`
    *   **結果：** 資料庫成功建立並應用遷移。

*   **動作：** 在 `GEMINI.md` 和 `README.md` 中將「設計並建立 SQL Server 資料庫」任務標記為完成。
    *   **結果：** 核取方塊從 `[ ]` 更新為 `[x]`。

### 3. 實作設備與感測器管理模組 (CRUD)

**目標：** 實作設備的 CRUD 功能。

*   **動作：** 建立 `DevicesController.cs` 處理設備的 CRUD 邏輯。
    *   **結果：** `DevicesController.cs` 成功建立。

*   **動作：** 建立 `src/SmartDeviceMonitoring.Web/Views/Devices` 目錄。
    *   **結果：** 目錄成功建立。

*   **動作：** 建立設備相關的 Razor 視圖 (`Index.cshtml`, `Details.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Delete.cshtml`)。
    *   **結果：** 所有視圖檔案成功建立。

*   **動作：** 在 `GEMINI.md` 和 `README.md` 中將「實作設備與感測器管理模組 (CRUD)」任務標記為完成。
    *   **結果：** 核取方塊從 `[ ]` 更新為 `[x]`。

### 4. 實作資料種子 (Data Seeding)

**目標：** 在應用程式啟動時，使用虛擬資料填充資料庫。

*   **動作：** 建立 `src/SmartDeviceMonitoring.Web/Data/seeddata.json` 檔案，包含範例設備資料。
    *   **結果：** `seeddata.json` 檔案成功建立。

*   **動作：** 將 `Microsoft.Extensions.DependencyInjection` NuGet 套件新增到 `SmartDeviceMonitoring.Web` 專案。
    *   **結果：** 套件成功新增。

*   **動作：** 建立 `src/SmartDeviceMonitoring.Web/Data/SeedData.cs` 類別，負責讀取 JSON 資料並將其插入資料庫。
    *   **結果：** `SeedData.cs` 檔案成功建立。

*   **動作：** 修改 `src/SmartDeviceMonitoring.Web/Program.cs`，在應用程式啟動時呼叫 `SeedData.Initialize` 方法，以執行資料種子。
    *   **結果：** `Program.cs` 已更新，包含資料種子邏輯和必要的 `using` 語句。

*   **動作：** 提交資料種子相關的變更。
    *   **命令：** `git add src/SmartDeviceMonitoring.Web/Program.cs src/SmartDeviceMonitoring.Web/SmartDeviceMonitoring.Web.csproj src/SmartDeviceMonitoring.Web/Data/SeedData.cs src/SmartDeviceMonitoring.Web/Data/seeddata.json`
    *   **命令：** `git commit -m "feat: Implement data seeding with JSON file"`
    *   **結果：** 變更已提交到本地儲存庫。

*   **動作：** 將提交的變更推送到遠端儲存庫。
    *   **命令：** `git push`
    *   **結果：** 變更成功推送。

### 5. 實作 SensorData 顯示功能

**目標：** 在 Web 應用程式中顯示感測器數據。

*   **動作：** 建立 `SensorDataController.cs` 處理感測器數據的顯示邏輯。
    *   **結果：** `SensorDataController.cs` 成功建立。

*   **動作：** 建立 `src/SmartDeviceMonitoring.Web/Views/SensorData` 目錄。
    *   **結果：** 目錄成功建立。

*   **動作：** 建立感測器數據相關的 Razor 視圖 (`Index.cshtml`, `Details.cshtml`)。
    *   **結果：** 所有視圖檔案成功建立。