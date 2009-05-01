#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IronRuby.Builtins;
using Microsoft.Scripting.Hosting;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    /// <summary>
    /// A facade over the classes for interacting with the IronRuby runtime
    /// </summary>
    public interface IRubyEngine
    {
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        object CallMethod(object receiver, string message, params object[] args);

        /// <summary>
        /// Gets the method names for the controller class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        IEnumerable<string> MethodNames(IController controller);

        /// <summary>
        /// Methods the names.
        /// </summary>
        /// <param name="controllerClass">The controller class.</param>
        /// <returns></returns>
        IEnumerable<string> MethodNames(RubyClass controllerClass);

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void LoadAssembly(Assembly assembly);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        object ExecuteScript(string script);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        object ExecuteScript(string script, ScriptScope scope);

        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="throwIfNotExist">if set to <c>true</c> [throw if not exist].</param>
        /// <returns></returns>
        object ExecuteFile(string path, ScriptScope scope, bool throwIfNotExist);

        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="throwIfNotExist">if set to <c>true</c> [throw if not exist].</param>
        /// <returns></returns>
        T ExecuteFile<T>(string path, bool throwIfNotExist);


        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        T ExecuteFile<T>(string path);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        T ExecuteScript<T>(string script);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script">The script.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        T ExecuteScript<T>(string script, ScriptScope scope);

        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="throwIfNotExist">if set to <c>true</c> [throw if not exist].</param>
        /// <returns></returns>
        T ExecuteFile<T>(string path, ScriptScope scope, bool throwIfNotExist);


        /// <summary>
        /// Executes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        object ExecuteFile(string path);

        /// <summary>
        /// Defines the read only global variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        void DefineReadOnlyGlobalVariable(string variableName, object value);


        /// <summary>
        /// Removes the class from globals.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        void RemoveClassFromGlobals(string className);

        /// <summary>
        /// Creates an instance of a ruby object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rubyClass">The ruby class.</param>
        /// <returns></returns>
        T CreateInstance<T>(RubyClass rubyClass);

        /// <summary>
        /// Gets the ruby class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        RubyClass GetRubyClass(string className);

        /// <summary>
        /// Gets the global variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T GetGlobalVariable<T>(string name);

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        void LoadAssemblies(params Type[] assemblies);

        /// <summary>
        /// Executes the block in scope.
        /// </summary>
        /// <param name="block">The block.</param>
        void ExecuteInScope(Action<ScriptScope> block);

        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        void RequireRubyFile(string path);

        /// <summary>
        /// Requires the ruby file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="readerType">Type of the reader.</param>
        void RequireRubyFile(string path, ReaderType readerType);
    }
}