namespace IronRubyMvcLibrary.Core
{
    public enum ReaderType
    {
        File,
        AssemblyResource
    }

    public enum When
    {
        BeforeAction,
        AfterAction,
        BeforeResult,
        AfterResult
    }
}