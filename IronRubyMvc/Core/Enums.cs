namespace IronRubyMvcLibrary.Core
{
    public enum ReaderType
    {
        File,
        AssemblyResource
    }

    public enum OnExecuting
    {
        BeforeAction,
        AfterAction,
        BeforeResult,
        AfterResult
    }
}