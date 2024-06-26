using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.WPF
{
    public class ServiceRegistry
    {
        private static readonly Ioc s_ioc;

        static ServiceRegistry()
        {
            s_ioc = Ioc.Default;
            s_ioc.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<LocalConfigService>()
                .AddSingleton<DayItemInformationModel>()
                .AddSingleton<HolidayUpdaterService>()
                .AddSingleton<DayLabelStyleSettingViewModel>()
                .AddSingleton<DaysOfMonthModel>()
                .AddSingleton<MainWindowViewModel>()
                .AddTransient<CalendarMonthViewModel>()
                .AddSingleton<SettingsViewModel>()
                .BuildServiceProvider());
        }

        public static T? GetService<T>() where T : class => s_ioc.GetService<T>();

        public static object? GetService(Type type) => s_ioc.GetService(type);
    }
}
