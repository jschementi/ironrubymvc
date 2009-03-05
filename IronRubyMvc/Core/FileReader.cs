#region Usings

using System.IO;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class FileReader : Reader
    {
        private readonly IPathProvider _pathProvider;

        public FileReader(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public override string Read(string filePath)
        {
            if (!_pathProvider.FileExists(filePath))
            {
                return string.Empty;
            }

            using (var stream = _pathProvider.Open(filePath))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}