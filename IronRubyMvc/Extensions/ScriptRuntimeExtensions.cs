#region Usings

using System.Web;
using Microsoft.Scripting.Hosting;

#endregion

namespace IronRubyMvcLibrary
{
    public static class ScriptRuntimeExtensions
    {
        private const string APPKEY_SCRIPTRUNTIME = "__ScriptRuntime__";

        public static ScriptRuntime GetScriptRuntime(this HttpApplicationState app)
        {
            return (ScriptRuntime) app[APPKEY_SCRIPTRUNTIME];
        }

        public static ScriptRuntime GetScriptRuntime(this HttpApplicationStateBase app)
        {
            return (ScriptRuntime) app[APPKEY_SCRIPTRUNTIME];
        }

        public static void SetScriptRuntime(this HttpApplicationState app, ScriptRuntime scriptRuntime)
        {
            app[APPKEY_SCRIPTRUNTIME] = scriptRuntime;
        }

        public static void SetScriptRuntime(this HttpApplicationStateBase app, ScriptRuntime scriptRuntime)
        {
            app[APPKEY_SCRIPTRUNTIME] = scriptRuntime;
        }
    }
}