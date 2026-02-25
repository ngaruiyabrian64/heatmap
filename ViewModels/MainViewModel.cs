using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocationTrackerApp.Data;
using LocationTrackerApp.Models;
using LocationTrackerApp.Services;
using Microsoft.Maui.Controls.Maps;
namespace LocationTrackerApp.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly LocationRepository _repository;
    private readonly LocationTrackingService _trackingService;
    private readonly HeatmapService _heatmapService;
    [ObservableProperty]
    private string _statusText = "Ready";
    [ObservableProperty]
    private int _pointCount;
    [ObservableProperty]
    private bool _isTracking;
    [ObservableProperty]
    private int _captureIntervalSeconds = 15;
    public event EventHandler<IReadOnlyList<Circle>>? HeatmapChanged;
    public event EventHandler<Location>? CameraRequested;
    public MainViewModel(
        LocationRepository repository,
        LocationTrackingService trackingService,
        HeatmapService heatmapService)
    {
        _repository = repository;
        _trackingService = trackingService;
        _heatmapService = heatmapService;
        _trackingService.LocationCaptured += OnLocationCaptured;
    }
    public async Task InitializeAsync()
    {
        await _trackingService.InitializeAsync();
        await RefreshHeatmapAsync();
    }
    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            var hasPermission = await _trackingService.RequestPermissionsAsync();
            if (!hasPermission)
            {
                StatusText = "Location permission denied.";
                return;
            }
            var interval = TimeSpan.FromSeconds(Math.Clamp(CaptureIntervalSeconds, 5, 120));
            _trackingService.StartTracking(interval);
            IsTracking = true;
            StatusText = "Tracking started.";
        }
        catch (Exception ex)
        {
            StatusText = $"Start failed: {ex.Message}";
        }
    }
    [RelayCommand]
    private Task StopAsync()
    {
        _trackingService.StopTracking();
        IsTracking = false;
        StatusText = "Tracking stopped.";
        return Task.CompletedTask;
    }
    [RelayCommand]
    private async Task RefreshAsync()
    {
        await RefreshHeatmapAsync();
    }
    [RelayCommand]
    private async Task ClearAsync()
    {
        await _repository.ClearAllAsync();
        await RefreshHeatmapAsync();
        StatusText = "All points cleared.";
    }
    private async void OnLocationCaptured(object? sender, LocationPoint point)
    {
        await RefreshHeatmapAsync();
        CameraRequested?.Invoke(this, new Location(point.Latitude, point.Longitude));
    }
    private async Task RefreshHeatmapAsync()
    {
        var points = await _repository.GetAllAsync();
        PointCount = points.Count;
        HeatmapChanged?.Invoke(this, _heatmapService.BuildHeatmap(points));
        if (points.Count == 0)
        {
            StatusText = "No saved locations yet.";
        }
        else
        {
            var latest = points[^1];
            StatusText = $"Last sample: {latest.CapturedAtUtc:yyyy-MM-dd HH:mm:ss} UTC";
            CameraRequested?.Invoke(this, new Location(latest.Latitude, latest.Longitude));
        }
    }
}
