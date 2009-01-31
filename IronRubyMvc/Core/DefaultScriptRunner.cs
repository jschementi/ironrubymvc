using System;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvc.Core
{
    internal class DefaultScriptRunner: IScriptRunner
    {
        private readonly ScriptEngine _engine;

        public DefaultScriptRunner(ScriptEngine engine) 
            : this(engine, string.Empty, new FileReader())
        {
            
        }

        public DefaultScriptRunner(ScriptEngine engine, ReaderType readerType) 
            : this(engine, string.Empty, readerType == ReaderType.File ? new FileReader() : (IReader)new AssemblyResourceReader())
        {
            
        }

        public DefaultScriptRunner(ScriptEngine engine, string scriptPath, IReader reader)
        {
            _engine = engine;
            ScriptPath = scriptPath;
            Reader = reader;
        }

        public string ScriptPath { get; private set; }

        public IReader Reader { get; private set; }
        
        public virtual object Execute()
        {
            if (ScriptPath.IsNullOrBlank())
                throw new NullReferenceException("You need to specify a ScriptPath in order to execute a script.");

            if(Reader.IsNull())
                throw new NullReferenceException("You need to provice a Reader in order to execute a script");

            return _engine.CreateScriptSourceFromString(Reader.Read(ScriptPath)).Execute();
        }

        public virtual T Execute<T>()
        {
            return (T) Execute();
        }

        public virtual object ExecuteFile(string scriptPath)
        {
            ScriptPath = scriptPath;
            return Execute();
        }

        public virtual T ExecuteFile<T>(string scriptPath)
        {
            return (T) ExecuteFile(scriptPath);
        }

        public virtual object ExecuteScript(string script)
        {
            return _engine.CreateScriptSourceFromString(script).Execute();
        }

        public virtual T ExecuteScript<T>(string script)
        {
            return (T) ExecuteScript(script);
        }
    }
}