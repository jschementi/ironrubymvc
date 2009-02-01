namespace IronRubyMvcLibrary.Core
{
    public abstract class Reader : IReader
    {
        #region IReader Members

        public abstract string Read(string filePath);

        #endregion
    }
}