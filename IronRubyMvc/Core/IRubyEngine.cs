#region Usings

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
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        TTarget CreateInstance<TTarget>(RubyClass rubyClass, params object[] args);

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="rubyClass">The ruby class.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        TTarget CreateInstance<TTarget>(RubyClass rubyClass, bool throwError, params object[] args) where TTarget : class;

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
        /// Loads the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void LoadAssembly(Assembly assembly);

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