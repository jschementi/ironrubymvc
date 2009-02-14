#region Usings

using System.IO;
using System.Web.Hosting;

#endregion

namespace IronRubyMvcLibrary.Core
{
    //Wraps the VirtualPathProvider so that testing is a bit easier
    public class VirtualPathProvider : IPathProvider
    {
        #region IPathProvider Members

        public bool FileExists(string filePath)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(filePath);
        }

        public Stream Open(string filePath)
        {
            var file = HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            return file.Open();
        }

        public string ApplicationPhysicalPath
        {
            get { return HostingEnvironment.ApplicationPhysicalPath; }
        }

        #endregion
    }
}