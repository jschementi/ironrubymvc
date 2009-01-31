using System.IO;
using System.Web.Hosting;

namespace IronRubyMvc.Core
{
    internal class FileReader : Reader
    {
        public override string Read(string filePath)
        {
            if (!HostingEnvironment.VirtualPathProvider.FileExists(filePath))
            {
                return null;
            }

            var file = HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            using (var stream = file.Open())
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                    
                }
            }
        }
    }
}