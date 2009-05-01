#region Usings

using System.IO;
using System.Web.Hosting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    //Wraps the VirtualPathProvider so that testing is a bit easier
    public class VirtualPathProvider : IPathProvider
    {
        #region IPathProvider Members

        public bool FileExists(string filePath)
        {
            
            return HostingEnvironment.VirtualPathProvider.FileExists(filePath);
        }

        public bool DirectoryExists(string dirPath)
        {
            return HostingEnvironment.VirtualPathProvider.DirectoryExists(dirPath);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(HostingEnvironment.MapPath(path) ?? ApplicationPhysicalPath, searchPattern);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(HostingEnvironment.MapPath(path) ?? ApplicationPhysicalPath, searchPattern);
        }

        public Stream Open(string filePath)
        {
            var file = HostingEnvironment.VirtualPathProvider.GetFile(filePath);
            return file.Open();
        }

        public string MapPath(string filePath)
        {
            return HostingEnvironment.MapPath(filePath);
        }

        public bool IsAbsolutePath(string path)
        {
            return new Uri(GetFullPath(path), UriKind.RelativeOrAbsolute).IsAbsoluteUri;
        }

        public string GetFullPath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(MapPath(path), mode, access, share);
        }

        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share, int size)
        {
            return new FileStream(MapPath(path), mode, access, share, size);
        }

        public string ApplicationPhysicalPath
        {
            get { return HostingEnvironment.ApplicationPhysicalPath; }
        }

        #endregion
    }
}