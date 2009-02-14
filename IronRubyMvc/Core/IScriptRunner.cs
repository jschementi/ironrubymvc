namespace IronRubyMvcLibrary.Core
{
    public interface IScriptRunner
    {
        string ScriptPath { get; }
        IReader Reader { get; }
        object Execute();
        object ExecuteFile(string scriptPath);
        object ExecuteScript(string script);
    }
}