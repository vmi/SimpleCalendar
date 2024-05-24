using System.Drawing;
using System.Linq;
using System.Reflection;

namespace SimpleCalendar.WinUI3.Utilities
{
    public sealed class AssemblyHelper
    {
        public static readonly AssemblyHelper Instance = new();

        private readonly Assembly assembly = Assembly.GetEntryAssembly();

        private AssemblyHelper() { }

        public Icon LoadIcon(string iconName)
        {
            if (assembly == null) { return null; }
            string match = $".Resources.{iconName}";
            string resName = assembly.GetManifestResourceNames().Where(name => name.EndsWith(match)).First();
            if (resName == null) { return null; }
            using System.IO.Stream stream = assembly.GetManifestResourceStream(resName);
            if (stream == null) { return null; }
            return new Icon(stream);
        }
    }
}
