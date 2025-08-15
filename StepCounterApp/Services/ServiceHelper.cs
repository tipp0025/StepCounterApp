namespace StepCounterApp.Services;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceHelper
{
    public static T GetRequiredService<T>() where T : notnull =>
        Current.GetRequiredService<T>();

    public static IServiceProvider Current =>
        Application.Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("Services not available");
}