#region Usings

using System.IO;
using System.Reflection;
using System.Web.Mvc.IronRuby.Extensions;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class PathProviderPal : PlatformAdaptationLayer
    {
        internal static PathProviderPal PAL = new PathProviderPal(new VirtualPathProvider());
        private readonly IPathProvider _pathProvider;

        protected PathProviderPal(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public override string CurrentDirectory
        {
            get { return _pathProvider.ApplicationPhysicalPath; }
        }

        internal static PathProviderPal Create(IPathProvider pathProvider)
        {
            pathProvider.EnsureArgumentNotNull("pathProvider");
            PAL = new PathProviderPal(pathProvider);

            return PAL;
        }

        public override bool DirectoryExists(string path)
        {
            return _pathProvider.DirectoryExists(path);
        }

        public override bool FileExists(string path)
        {
            return _pathProvider.FileExists(path);
        }

        public override string[] GetDirectories(string path, string searchPattern)
        {
            return _pathProvider.GetDirectories(path, searchPattern);
        }

        public override string[] GetFiles(string path, string searchPattern)
        {
            return _pathProvider.GetFiles(path, searchPattern);
        }

        public override string GetFullPath(string path)
        {
            return _pathProvider.GetFullPath(path);
        }

        public override bool IsAbsolutePath(string path)
        {
            return _pathProvider.IsAbsolutePath(path);
        }

        public override Stream OpenInputFileStream(string path)
        {
            return _pathProvider.Open(path);
        }

        public override Stream OpenInputFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return _pathProvider.Open(path, mode, access, share);
        }

        public override Stream OpenInputFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        {
            return _pathProvider.Open(path, mode, access, share, bufferSize);
        }

        public override Assembly LoadAssemblyFromPath(string path)
        {
            return base.LoadAssemblyFromPath(_pathProvider.MapPath(path));
        }
    }
}