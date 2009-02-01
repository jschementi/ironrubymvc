using System.IO;
using System.Web.Hosting;

namespace IronRubyMvc.Core
{
    //Wraps the VirtualPathProvider so that testing is a bit easier
    public class VirtualPathProvider : IPathProvider
    {
        public bool FileExists(string filePath)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(filePath);
        }

        public Stream Open(string filePath)
        {
            var file = HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            return file.Open();
        }
    }
}