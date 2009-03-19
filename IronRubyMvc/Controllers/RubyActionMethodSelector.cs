#region Usings

using System.Collections;
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
        public IEnumerable<KeyValuePair<string, PredicateList>> AliasedMethods { get; private set; }

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

        private KeyValuePair<string, PredicateList> KeyValuePairFor(KeyValuePair<object, object> pair)
        {
            return new KeyValuePair<string, PredicateList>(pair.Key.ToString(), new PredicateList(_rubyEngine, (RubyArray) pair.Value));
        }

        private static AmbiguousMatchException CreateAmbiguousMatchException(string actionName)
        {
            return new AmbiguousMatchException("Too many methods found when looking for: " + actionName);
        }

        public string FindActionMethod(ControllerContext controllerContext, string actionName)
        {
            PopulateLookupTables(controllerContext); // dynamic languages can add methods at runtime
            var methodsMatchingName = GetMatchingAliasedMethods(controllerContext, actionName);
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

        private List<string> GetMatchingAliasedMethods(ControllerContext controllerContext, string actionName)
        {
            return new List<string>(AliasedMethods.Where(pair => pair.Key == actionName && pair.Value.IsValidForName(controllerContext, actionName)).Map(pair => pair.Key));
        }

        private List<string> RunSelectionFilters(ControllerContext controllerContext, IEnumerable<string> matchingMethods)
        {
            var filtersDescriptions = (Hash) _rubyEngine.CallMethod(ControllerClass, "method_selectors");
            var filters = filtersDescriptions.Where(pair => matchingMethods.Contains(pair.Key.ToString())).Map(pair => KeyValuePairFor(pair));

            return filters.Count() == 0
                       ? new List<string>(matchingMethods)
                       : new List<string>(
                           matchingMethods.Where(
                                methodName => filters.All(filter => filter.Value.IsValidForName(controllerContext, methodName))
                           )
                        );
        }
    }

    public class PredicateList : IEnumerable<Func<ControllerContext, string, bool>>
    {
        private readonly RubyArray _items;
        private readonly List<Func<ControllerContext, string, bool>> _predicates = new List<Func<ControllerContext, string, bool>>();
        private readonly IRubyEngine _rubyEngine;

        public PredicateList(IRubyEngine rubyEngine, RubyArray items)
        {
            _rubyEngine = rubyEngine;
            _items = items;
            Populate();
        }

        private void Populate()
        {
            _items.ForEach(obj => Add((Proc) obj));
        }

        private void Add(Proc proc)
        {
            _predicates.Add(_rubyEngine.ConvertProcToFunc<bool>(proc));
        }

        public bool IsValidForName(ControllerContext context, string name)
        {
            return _predicates.All(predicate => predicate(context, name));
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Func<ControllerContext, string, bool>> GetEnumerator()
        {
            return _predicates.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}