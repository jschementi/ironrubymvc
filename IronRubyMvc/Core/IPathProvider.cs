using System.IO;

namespace IronRubyMvc.Core
{
    public interface IPathProvider
    {
        bool FileExists(string filePath);
        Stream Open(string filePath); 
    }
}