#region Usings

using System.IO;
using System.Reflection;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class AssemblyStreamContentProvider : StreamContentProvider
    {
        private readonly Assembly _assembly;
        private readonly string _fileName;

        public AssemblyStreamContentProvider(string fileName, Assembly assembly)
        {
            _fileName = fileName;
            _assembly = assembly;
        }

        #region Overrides of StreamContentProvider

        public override Stream GetStream()
        {
            return _assembly.GetManifestResourceStream(_fileName);
        }

        #endregion
    }
}