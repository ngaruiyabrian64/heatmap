using LocationTrackerApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace LocationTrackerApp;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.HeatmapChanged += (_, circles) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HeatMap.MapElements.Clear();
                foreach (var circle in circles)
                {
                    HeatMap.MapElements.Add(circle);
                }
            });
        };

        _viewModel.CameraRequested += (_, location) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HeatMap.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMeters(400)));
            });
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
