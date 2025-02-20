# Pomodoro Timer

[English](#english) | [中文](#中文)

# English

A Windows Forms-based Pomodoro Timer application that helps you improve work efficiency and time management.

## Features

- 🍅 Standard Pomodoro Time Management
  - 25 minutes focus time
  - 5 minutes short break
  - 30 minutes long break (after 4 Pomodoro cycles)

- 💡 Smart Notifications
  - Automatic popup reminders when time is up
  - Window shake effect to ensure you don't miss notifications
  - Automatic rest/work state switching

- 📊 Statistics
  - Record daily completed Pomodoros
  - Track accumulated focus time
  - View history records

- ⚙️ Utilities
  - Auto-start option
  - Automatic data saving
  - Clean and intuitive interface

## Getting Started

### Prerequisites
- Windows OS
- .NET 9.0 or higher

### Build and Run
1. Clone the repository
```
git clone https://github.com/yourusername/PomodoroTimer.git
cd PomodoroTimer
```

2. Build the project
```
dotnet build --configuration Release
```

3. Run the application
```
dotnet run --configuration Release
```

Or you can open the solution in Visual Studio and run it directly.

### Usage
1. Start the application
2. Click the start button to begin timing
3. Focus on work for 25 minutes
4. Automatically enter break mode when time is up
   - Get 30 minutes long break after completing 4 Pomodoro cycles
   - Get 5 minutes short break otherwise
5. Start a new work cycle after the break

### Data Storage
Application data is saved in the local user data folder:
```
%LocalAppData%\PomodoroTimer\data.json
```

### Development Stack
- C# / .NET 9.0
- Windows Forms
- Newtonsoft.Json for data serialization

---

# 中文

一个基于 Windows Forms 开发的番茄钟计时器应用程序，帮助你提高工作效率和时间管理能力。

## 功能特性

- 🍅 标准番茄工作法时间管理
  - 25分钟专注工作时间
  - 5分钟短休息时间
  - 30分钟长休息时间（每完成4个番茄周期后）

- 💡 智能提醒
  - 时间结束时自动弹出提醒窗口
  - 窗口抖动效果，确保不会错过提醒
  - 休息/工作状态自动切换

- 📊 数据统计
  - 记录每日完成的番茄钟数量
  - 追踪累计专注时间
  - 查看历史记录

- ⚙️ 实用功能
  - 开机自启动选项
  - 数据自动保存
  - 简洁直观的界面

## 开始使用

### 系统要求
- Windows 操作系统
- .NET 9.0 或更高版本

### 编译和运行
1. 克隆仓库
```
git clone https://github.com/yourusername/PomodoroTimer.git
cd PomodoroTimer
```

2. 编译项目
```
dotnet build --configuration Release
```

3. 运行应用
```
dotnet run --configuration Release
```

或者您可以直接在 Visual Studio 中打开解决方案并运行。

### 使用说明
1. 启动应用程序
2. 点击开始按钮开始计时
3. 专注工作25分钟
4. 时间结束后自动进入休息模式
   - 完成4个番茄钟周期后会获得30分钟长休息
   - 其他情况获得5分钟短休息
5. 休息结束后，可以开始新的工作周期

### 数据存储
应用数据保存在本地用户数据文件夹：
```
%LocalAppData%\PomodoroTimer\data.json
```

### 开发技术
- C# / .NET 9.0
- Windows Forms
- Newtonsoft.Json 用于数据序列化