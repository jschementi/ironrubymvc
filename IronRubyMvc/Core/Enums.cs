namespace System.Web.Mvc.IronRuby.Core
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