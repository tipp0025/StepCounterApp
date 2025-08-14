using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Hardware;
using Android.Content;
using Android.Runtime;
using AndroidX.Core.App;

namespace StepCounterApp;

[Activity(
    Theme = "@style/Maui.SplashTheme", 
    Label = "StepCounterApp", 
    MainLauncher = true, 
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode)]
public class MainActivity : MauiAppCompatActivity, ISensorEventListener
{
    SensorManager? sensorManager;
    Sensor? stepSensor;
    static int stepCount = 0;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        sensorManager = (SensorManager?)GetSystemService(SensorService);
        stepSensor = sensorManager?.GetDefaultSensor(SensorType.StepCounter);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            if (CheckSelfPermission(Android.Manifest.Permission.ActivityRecognition) != Permission.Granted)
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.ActivityRecognition }, 0);
            }
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        sensorManager?.RegisterListener(this, stepSensor, SensorDelay.Ui);    }

    protected override void OnPause()
    {
        base.OnPause();
        sensorManager?.UnregisterListener(this);
    }

    public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy) { }

    public void OnSensorChanged(SensorEvent? e)
    {
        if (e?.Sensor?.Type == SensorType.StepCounter)
        {
            var stepCount = (int)e.Values[0];
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send<object, int>(this, "StepUpdated", stepCount);
            });
        }
    }

}
