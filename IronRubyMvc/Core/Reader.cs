namespace IronRubyMvc.Core
{
    public abstract class Reader : IReader
    {
        public abstract string Read(string filePath);
    }
}