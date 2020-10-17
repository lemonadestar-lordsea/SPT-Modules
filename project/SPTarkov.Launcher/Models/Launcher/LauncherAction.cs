using SPTarkov.Launcher.Converters;
using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    [TypeConverter(typeof(EnumToLocaleStringConverter))]
    public enum LauncherAction
    {
        MinimizeAction,
        MinimizeToSystemTrayAction,
        DoNothingAction,
        ExitAction
    }
}
