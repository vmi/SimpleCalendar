using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleCalendar.WPF.Utilities
{
    public static class PropertyChangedEventHelper
    {
        private static readonly object s_exist = new();
        private static readonly ConditionalWeakTable<INotifyPropertyChanged, ConditionalWeakTable<PropertyChangedEventHandler, object>> s_registered = [];
        public static bool RegisterPropertyChangedEventHandler(this INotifyPropertyChanged target, PropertyChangedEventHandler handler)
        {
            lock (s_registered)
            {
                if (s_registered.TryGetValue(target, out ConditionalWeakTable<PropertyChangedEventHandler, object> handlers))
                {
                    if (handlers.TryGetValue(handler, out _))
                    {
                        return false;
                    }
                    handlers.Add(handler, s_exist);
                }
                else
                {
                    handlers = new()
                    {
                        { handler, s_exist }
                    };
                    s_registered.Add(target, handlers);
                }
            }
            target.PropertyChanged += handler;
            return true;
        }
    }
}
