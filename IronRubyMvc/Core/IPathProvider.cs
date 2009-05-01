#region Usings

using System.IO;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public interface IPathProvider
    {
        string ApplicationPhysicalPath { get; }
        bool FileExists(string filePath);
        bool DirectoryExists(string dirPath);
        string[] GetDirectories(string path, string searchPattern);
        string[] GetFiles(string path, string searchPattern);
        Stream Open(string filePath);
        string MapPath(string filePath);
        bool IsAbsolutePath(string path);
        string GetFullPath(string path);

        Stream Open(string path, FileMode mode, FileAccess access, FileShare share);
        Stream Open(string path, FileMode mode, FileAccess access, FileShare share, int size);
    }
}