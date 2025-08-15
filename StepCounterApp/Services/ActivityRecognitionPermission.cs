namespace StepCounterApp.Services;

// ActivityRecognitionPermission.cs
using Microsoft.Maui.ApplicationModel;

#if ANDROID
public class ActivityRecognitionPermission : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new[] { (global::Android.Manifest.Permission.ActivityRecognition, true) };
}
#endif