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

        public VirtualPathStreamContentProvider(string fileName)
        {
            _fileName = fileName;
        }

        #region Overrides of StreamContentProvider

        public override Stream GetStream()
        {
            return HostingEnvironment.VirtualPathProvider.GetFile(_fileName).Open();
        }

        #endregion
    }
}