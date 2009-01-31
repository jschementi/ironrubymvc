namespace IronRubyMvc.Core
{
    internal interface IScriptRunner
    {
        string ScriptPath { get; }
        IReader Reader { get; }
        object Execute();
        T Execute<T>();
        object ExecuteFile(string scriptPath);
        T ExecuteFile<T>(string scriptPath);
        object ExecuteScript(string script);
        T ExecuteScript<T>(string script);
    }
}