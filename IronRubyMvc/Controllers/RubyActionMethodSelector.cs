#region Usings

using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    /// <summary>
    /// Encapsulates selecting action methods on a ruby controller
    /// </summary>
    public class RubyActionMethodSelector
    {
        private readonly IRubyEngine _rubyEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyActionMethodSelector"/> class.
        /// </summary>
        /// <param name="rubyEngine">The ruby engine.</param>
        /// <param name="rubyClass">The ruby class.</param>
        public RubyActionMethodSelector(IRubyEngine rubyEngine, RubyClass rubyClass)
        {
            ControllerClass = rubyClass;
            _rubyEngine = rubyEngine;
            
        }

        public RubyClass ControllerClass { get; set; }

        /// <summary>
        /// Gets or sets the aliased methods.
        /// </summary>
        /// <value>The aliased methods.</value>
        public IEnumerable<KeyValuePair<string, Func<string, bool>>> AliasedMethods { get; private set; }

        /// <summary>
        /// Gets or sets the non aliased method names.
        /// </summary>
        /// <value>The non aliased method names.</value>
        public IEnumerable<string> NonAliasedMethods { get; private set; }

        private void PopulateLookupTables(ControllerContext controllerContext)
        {
            var methodNames = _rubyEngine.MethodNames(controllerContext.Controller);
            PopulateLookupTables(methodNames);
        }

        private void PopulateLookupTables(IEnumerable<string> methodNames)
        {
            var methodAliases = (Hash)_rubyEngine.CallMethod(ControllerClass, "name_selectors");
            AliasedMethods = methodAliases.Map(pair => KeyValuePairFor(pair));
            NonAliasedMethods = methodNames.Where(method => AliasedMethods.DoesNotContain(pair => String.Equals(pair.Key, method, StringComparison.OrdinalIgnoreCase)));
        }

        private KeyValuePair<string, Func<string, bool>> KeyValuePairFor(KeyValuePair<object, object> pair)
        {
            return new KeyValuePair<string, Func<string, bool>>(pair.Key.ToString(), _rubyEngine.ConvertProcToFunc<bool>((Proc) pair.Value));
        }

        private static AmbiguousMatchException CreateAmbiguousMatchException(string actionName)
        {
            return new AmbiguousMatchException("Too many methods found when looking for: " + actionName);
        }

        public string FindActionMethod(ControllerContext controllerContext, string actionName)
        {
            PopulateLookupTables(controllerContext); // dynamic languages can add methods at runtime
            var methodsMatchingName = GetMatchingAliasedMethods(actionName);
            methodsMatchingName.AddRange(NonAliasedMethods.Where(name => String.Equals(name, actionName, StringComparison.OrdinalIgnoreCase)));
            var finalMethods = RunSelectionFilters(controllerContext, methodsMatchingName);

            switch (finalMethods.Count)
            {
                case 0:
                    return null;

                case 1:
                    return finalMethods[0];

                default:
                    throw CreateAmbiguousMatchException(actionName);
            }
        }

        public IEnumerable<string> GetAllActionMethods()
        {
            PopulateLookupTables(_rubyEngine.MethodNames(ControllerClass));

            var result = new List<string>();
            result.AddRange(AliasedMethods.Map(method => method.Key));
            result.AddRange(NonAliasedMethods);
            return result;
        }

        private List<string> GetMatchingAliasedMethods(string actionName)
        {
            return new List<string>(AliasedMethods.Where(pair => pair.Value(actionName)).Map(pair => pair.Key));
        }

        private List<string> RunSelectionFilters(ControllerContext controllerContext, IEnumerable<string> matchingMethods)
        {
            var filtersDescriptions = (Hash) _rubyEngine.CallMethod(controllerContext.Controller, "method_selectors");
            var filters = filtersDescriptions.Map(pair => _rubyEngine.ConvertProcToFunc<bool>((Proc) pair.Value));
            
            return filters.Count() == 0 
                ? new List<string>(matchingMethods) 
                : new List<string>(matchingMethods.Where(methodName => filters.All(filter => filter(methodName))));
        }
    }
}