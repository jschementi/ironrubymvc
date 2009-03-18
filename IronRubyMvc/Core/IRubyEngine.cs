#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public interface IRubyEngine
    {
        /// <summary>
        /// Loads the controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        RubyController LoadController(RequestContext requestContext, string controllerName);

        /// <summary>
        /// Configures the controller.
        /// </summary>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        RubyController ConfigureController(RubyClass rubyClass, RequestContext requestContext);

        /// <summary>
        /// Converts a proc object to a .NET Func.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="proc">The proc.</param>
        /// <returns></returns>
        Func<string, TResult> ConvertProcToFunc<TResult>(Proc proc);

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        object CallMethod(object receiver, string message, params object[] args);

        /// <summary>
        /// Determines whether the specified controller as the action.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>
        /// 	<c>true</c> if the specified controller has the action; otherwise, <c>false</c>.
        /// </returns>
        bool HasControllerAction(RubyController controller, string actionName);

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
        /// Defines the read only global variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        void DefineReadOnlyGlobalVariable(string variableName, object value);

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
    }
}