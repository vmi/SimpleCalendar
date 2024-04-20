using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
                .AddSingleton<CurrentMonthViewModel>()
                .AddTransient(provider => new CalendarMonthViewModel(provider.GetService<CurrentMonthViewModel>()!))
                .BuildServiceProvider());
        }

        public static T? GetService<T>() where T : class => ioc.GetService<T>();

        public static object? GetService(Type type) => ioc.GetService(type);
    }
}
