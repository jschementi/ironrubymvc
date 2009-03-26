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
            var methodAliases = (Hash) _rubyEngine.CallMethod(ControllerClass, "name_selectors");
            AliasedMethods = methodAliases.Map(pair => KeyValuePairFor(pair));
            NonAliasedMethods =
                methodNames.Where(
                    method =>
                    AliasedMethods.DoesNotContain(
                        pair => String.Equals(pair.Key, method.Underscore(), StringComparison.OrdinalIgnoreCase) || String.Equals(pair.Key, method.Pascalize(), StringComparison.OrdinalIgnoreCase)));
        }

        private static KeyValuePair<string, PredicateList> KeyValuePairFor(KeyValuePair<object, object> pair)
        {
            return new KeyValuePair<string, PredicateList>(pair.Key.ToString(), new PredicateList((RubyArray) pair.Value));
        }

        private static AmbiguousMatchException CreateAmbiguousMatchException(string actionName)
        {
            return new AmbiguousMatchException("Too many methods found when looking for: " + actionName);
        }

        public string FindActionMethod(ControllerContext controllerContext, string actionName)
        {
            PopulateLookupTables(controllerContext); // dynamic languages can add methods at runtime
            var methodsMatchingName = GetMatchingAliasedMethods(controllerContext, actionName);
            methodsMatchingName.AddRange(
                NonAliasedMethods.Where(
                    name => String.Equals(name, actionName.Underscore(), StringComparison.OrdinalIgnoreCase) || String.Equals(name, actionName.Pascalize(), StringComparison.OrdinalIgnoreCase)));
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
            return new List<string>(AliasedMethods.Where(pair => pair.Value.IsValidForAction(controllerContext, actionName)).Map(pair => pair.Key));
        }

        private List<string> RunSelectionFilters(ControllerContext controllerContext, IEnumerable<string> matchingMethods)
        {
            var filtersDescriptions = (Hash) _rubyEngine.CallMethod(ControllerClass, "method_selectors");
            var filters =
                filtersDescriptions.Where(pair => matchingMethods.Contains(pair.Key.ToString().Underscore()) || matchingMethods.Contains(pair.Key.ToString().Pascalize())).Map(
                    pair => KeyValuePairFor(pair));

            return filters.Count() == 0
                       ? new List<string>(matchingMethods)
                       : new List<string>(
                             matchingMethods.Where(
                                 methodName => filters.All(filter => filter.Value.IsValid(controllerContext, methodName))
                                 )
                             );
        }
    }

    public class PredicateList : IEnumerable<Func<ControllerContext, string, bool>>
    {
        private readonly RubyArray _items;
        private readonly List<Func<ControllerContext, string, bool>> _predicates = new List<Func<ControllerContext, string, bool>>();

        public PredicateList(RubyArray items)
        {
            _items = items;
            Populate();
        }

        private void Populate()
        {
            _items.ForEach(obj => Add((Proc) obj));
        }

        private void Add(Proc proc)
        {
            _predicates.Add(ConvertProcToFunc<bool>(proc));
        }

        public bool IsValid(ControllerContext context, string name)
        {
            return _predicates.All(predicate => predicate(context, name));
        }

        public bool IsValidForAction(ControllerContext context, string name)
        {
            var result = false;
            foreach (var list in _predicates)
            {
                if (list(context, name))
                {
                    result = true;
                    break;
                }
            }
            return result;
//            var result = _predicates.Any(predicate =>
//                                             {
//                                                 var result1 = predicate(context, name.Underscore());
//                                                 var result2 = predicate(context, name.Pascalize());
//                                                 return result1 || result2;
//                                             });
//            return result;
        }

        public Func<ControllerContext, string, TResult> ConvertProcToFunc<TResult>(Proc proc)
        {
            return (context, name) => (TResult) proc.Call(context, name);
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