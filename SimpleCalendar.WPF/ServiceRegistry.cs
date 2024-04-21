using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SimpleCalendar.WPF.Services;
using SimpleCalendar.WPF.ViewModels;

namespace SimpleCalendar.WPF
{
    public class ServiceRegistry
    {
        private static readonly Ioc ioc;

        static ServiceRegistry()
        {
            ioc = Ioc.Default;
            ioc.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<DayItemService>()
                .AddSingleton<DaysOfMonthService>()
                .AddSingleton<CurrentMonthViewModel>()
                .AddTransient<CalendarMonthViewModel>()
                .BuildServiceProvider());
        }

        public static T? GetService<T>() where T : class => ioc.GetService<T>();

        public static object? GetService(Type type) => ioc.GetService(type);
    }
}
