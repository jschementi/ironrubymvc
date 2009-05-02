#region Usings

using System.IO;
using System.Web.Hosting;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class VirtualPathStreamContentProvider : StreamContentProvider
    {
        private readonly string _fileName;
        private readonly IPathProvider _pathProvider;

        public VirtualPathStreamContentProvider(string fileName, IPathProvider pathProvider)
        {
            _fileName = fileName;
            _pathProvider = pathProvider;
        }

        #region Overrides of StreamContentProvider

        public override Stream GetStream()
        {
            return _pathProvider.Open(_fileName);
        }

        #endregion
    }
}