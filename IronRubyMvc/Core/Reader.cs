namespace IronRubyMvc.Core
{
    internal abstract class Reader : IReader
    {
        public abstract string Read(string filePath);
    }
}