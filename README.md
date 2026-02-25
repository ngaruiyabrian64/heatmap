# Location Tracker App (.NET MAUI)

This project implements the assignment brief from `Final_Assignment.docx`:
- Track user location with C# and .NET MAUI
- Save each captured location in SQLite
- Visualize saved points as a heat map on a map

## Features
- Start/Stop timed location capture
- SQLite persistence (`locationtracker.db3` in app data)
- Heat map rendering using map circles and intensity-based color/radius
- Refresh and clear stored data

## Requirements
- Visual Studio with .NET MAUI workload
- .NET 8 SDK
- Android emulator/device with location services

## NuGet Packages
- `CommunityToolkit.Mvvm`
- `Microsoft.Maui.Controls.Maps`
- `sqlite-net-pcl`

## Notes
- On first run, grant location permission when prompted.
- Capture interval is configurable in seconds (minimum 5).
- Current project target is `net8.0-android`.
- This environment did not have .NET SDK installed, so compilation was not executed here.

## GitHub Actions APK Build
- Push this project to GitHub.
- Open `Actions` tab -> `Build Android APK` -> `Run workflow`.
- After completion, download artifact `LocationTrackerApp-apk` (contains the generated `.apk`).
