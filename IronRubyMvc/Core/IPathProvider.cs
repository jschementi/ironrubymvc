#region Usings

using System.IO;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public interface IPathProvider
    {
        string ApplicationPhysicalPath { get; }
        bool FileExists(string filePath);
        Stream Open(string filePath);
        string MapPath(string filePath);
    }
}