
#if ANDROID
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using StepCounterApp.Services;

namespace StepCounterApp;

public class StepCounterService : Java.Lang.Object, IStepCounterService, ISensorEventListener
{
    private readonly SensorManager _sensorManager;
    private readonly Sensor? _stepCounter;   
    private readonly Sensor? _stepDetector;

    private int _baseCounter = -1;           
    private int _sessionSteps = 0;

    public event EventHandler<int>? StepsChanged;

    public StepCounterService()
    {
        _sensorManager = (SensorManager)Android.App.Application.Context!
            .GetSystemService(Context.SensorService)!;

        _stepCounter  = _sensorManager.GetDefaultSensor(SensorType.StepCounter);
        _stepDetector = _sensorManager.GetDefaultSensor(SensorType.StepDetector);
    }

    public void Start()
    {
        _sessionSteps = 0;
        _baseCounter  = -1;

        if (_stepCounter != null)
            _sensorManager.RegisterListener(this, _stepCounter, SensorDelay.Normal);
        else if (_stepDetector != null)
            _sensorManager.RegisterListener(this, _stepDetector, SensorDelay.Normal);
    }

    public void Stop() => _sensorManager.UnregisterListener(this);

    public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy) { }

    public void OnSensorChanged(SensorEvent? e)
    {
        MessagingCenter.Send<object,int>(this, "StepUpdated", _sessionSteps);
        if (e?.Sensor is null || e.Values is null || e.Values.Count == 0) return;

        if (e.Sensor.Type == SensorType.StepCounter)
        {
            int totalSinceBoot = (int)e.Values[0];
            if (_baseCounter < 0) _baseCounter = totalSinceBoot;
            _sessionSteps = Math.Max(0, totalSinceBoot - _baseCounter);
        }
        else if (e.Sensor.Type == SensorType.StepDetector)
        {
            _sessionSteps += 1;
        }

        StepsChanged?.Invoke(this, _sessionSteps);
    }
}
#endif