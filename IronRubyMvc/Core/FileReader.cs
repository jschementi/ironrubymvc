#region Usings

using System.IO;

#endregion

namespace IronRubyMvcLibrary.Core
{
    public class FileReader : Reader
    {
        private readonly IPathProvider _pathProvider;

        public FileReader(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public FileReader() : this(new VirtualPathProvider())
        {
        }

        public override string Read(string filePath)
        {
            if (!_pathProvider.FileExists(filePath))
            {
                return string.Empty;
            }

            using (Stream stream = _pathProvider.Open(filePath))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}