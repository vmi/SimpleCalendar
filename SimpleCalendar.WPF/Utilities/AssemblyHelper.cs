using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace SimpleCalendar.WPF.Utilities
{
    public sealed class AssemblyHelper
    {
        public static readonly AssemblyHelper Instance = new();

        private readonly Assembly assembly = Assembly.GetEntryAssembly();

        private AssemblyHelper()
        {
            foreach (var attr in assembly.GetCustomAttributes())
            {
                Debug.WriteLine($"attr={attr}");
            }
        }

        public Icon? LoadIcon(string iconName)
        {
            var match = $".Resources.{iconName}";
            var resName = assembly.GetManifestResourceNames().Where(name => name.EndsWith(match)).First();
            if (resName == null) { return null; }
            using var stream = assembly.GetManifestResourceStream(resName);
            if (stream == null) { return null; }
            return new Icon(stream);
        }
    }
}
