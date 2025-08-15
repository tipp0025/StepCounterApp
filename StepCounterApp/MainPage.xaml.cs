using Microsoft.Maui.ApplicationModel;
using StepCounterApp.Services;

namespace StepCounterApp;

public partial class MainPage : ContentPage
{
    private readonly IStepCounterService _steps;
    
    private int _stepCount;
    public int StepCount
    {
        get => _stepCount;
        set
        {
            if (_stepCount == value) return;
            _stepCount = value;
            OnPropertyChanged(nameof(StepCount));
        }
    }

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        _steps = ServiceHelper.GetRequiredService<IStepCounterService>();
        _steps.StepsChanged += (_, count) =>
        {
            MainThread.BeginInvokeOnMainThread(() => StepCount = count);
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        // Android 10+ requires ACTIVITY_RECOGNITION for step sensors
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            var status = await Permissions.CheckStatusAsync<ActivityRecognitionPermission>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<ActivityRecognitionPermission>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission needed",
                    "Physical Activity permission is required to count steps.", "OK");
                return;
            }
        }
#endif

        _steps.Start();
    }

    protected override void OnDisappearing()
    {
        _steps.Stop();
        base.OnDisappearing();
    }
}