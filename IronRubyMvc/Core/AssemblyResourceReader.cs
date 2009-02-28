#region Usings

using System.IO;
using System.Reflection;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Core
{
    public class AssemblyResourceReader : Reader
    {
        private readonly Assembly _assembly;

        public AssemblyResourceReader(Assembly assembly)
        {
            _assembly = assembly;
        }

//        public AssemblyResourceReader() : this(typeof (AssemblyResourceReader).Assembly)
//        {
//        }

        public override string Read(string filePath)
        {
            using (var stream = _assembly.GetManifestResourceStream(filePath))
                if (stream.IsNotNull())
                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd();
            return string.Empty;
        }
    }
}