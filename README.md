# Pomodoro Timer

Pomodoro Timer is a Windows desktop application that helps you track your work sessions and breaks using the Pomodoro Technique.

## Features

- **Work and Break Sessions:** Automatically switches between work sessions, short breaks, and long breaks.
- **Auto-Start:** Optionally set the application to run at system startup.
- **Visual Notifications:** Brings the application window to the front and shakes it to alert you when a session completes.
- **Data Persistence:** Saves your daily completed pomodoros and total focus time.

## Project Structure

```
PomodoroTimer/
├─ PomodoroTimer/             # Main application folder
│  ├─ MainForm.cs             # Main form of the application
│  ├─ HistoryForm.cs          # Displays session history
│  ├─ Models/                # Data models (e.g., PomodoroData.cs)
│  ├─ Services/              # Core services (e.g., PomodoroService.cs)
│  └─ ...                   
├─ PomodoroTimer.Tests/       # Unit tests
├─ PomodoroTimer.sln         # Visual Studio solution file
└─ ...
```

## Prerequisites

- [.NET 5+ SDK (or .NET 9.0 as used)](https://dotnet.microsoft.com/download)
- Windows OS

## Building the Application

1. Open a command prompt in the workspace directory (e.g., `C:\Work\Pomodoro`).
2. Build the solution:

   ```bash
   dotnet build PomodoroTimer.sln
   ```

## Running the Application

Run the application with:

```bash
   dotnet run --project PomodoroTimer/PomodoroTimer/PomodoroTimer.csproj
```

## Running Tests

Execute the unit tests via:

```bash
   dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj
```

## License

This project is licensed under the MIT License.

## Acknowledgements

- Inspired by the Pomodoro Technique.
- Utilizes [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON serialization.

Enjoy using Pomodoro Timer to boost your productivity!
