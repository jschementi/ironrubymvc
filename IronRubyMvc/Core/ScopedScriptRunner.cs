using System;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvc.Core
{
    internal class ScopedScriptRunner: IScriptRunner
    {
        private readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;

        

        public ScopedScriptRunner(ScriptEngine engine, ScriptScope scope) 
            : this(engine, scope, string.Empty, new FileReader())
        {
            
        }

        public ScopedScriptRunner(ScriptEngine engine, ScriptScope scope, ReaderType readerType) 
            : this(engine, scope, string.Empty, readerType == ReaderType.File ? new FileReader() : (IReader)new AssemblyResourceReader())
        {
            
        }

        public ScopedScriptRunner(ScriptEngine engine, ScriptScope scope, string scriptPath, IReader reader)
        {
            _engine = engine;
            _scope = scope;
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

            return ExecuteScript(Reader.Read(ScriptPath));
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
            return _engine.Execute(script, _scope);
        }

        public virtual T ExecuteScript<T>(string script)
        {
            return (T) ExecuteScript(script);
        }
    }
}