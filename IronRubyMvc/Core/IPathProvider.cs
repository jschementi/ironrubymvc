#region Usings

using System.IO;

#endregion

namespace IronRubyMvcLibrary.Core
{
    public interface IPathProvider
    {
        bool FileExists(string filePath);
        Stream Open(string filePath);
        string ApplicationPhysicalPath { get; }
    }
}