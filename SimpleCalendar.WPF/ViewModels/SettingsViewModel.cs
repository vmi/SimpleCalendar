using CommunityToolkit.Mvvm.ComponentModel;
using SimpleCalendar.WPF.Models;

namespace SimpleCalendar.WPF.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public SettingsLogger Logger { get; }

        public SettingsViewModel(SettingsLogger logger)
        {
            Logger = logger;
        }
    }
}
