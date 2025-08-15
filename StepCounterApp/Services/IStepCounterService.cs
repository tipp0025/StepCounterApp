namespace StepCounterApp.Services;

public interface IStepCounterService
{
    event EventHandler<int> StepsChanged;
    void Start();
    void Stop();
}